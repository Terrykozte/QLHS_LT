using System;
using System.Security.Cryptography;
using System.Text;
using QLTN_LT.DAL;
using QLTN_LT.DTO;
using QLTN_LT.DAL.Interfaces;
using System.Diagnostics;
using System.Linq;
using System.Collections.Concurrent;

namespace QLTN_LT.BLL
{
    public class AuthBLL
    {
        private readonly IUserRepository _userRepo;
        private static readonly string LogSource = "AuthBLL";

        // Lockout policy (in-memory)
        private const int MAX_ATTEMPTS = 5;
        private static readonly TimeSpan WINDOW = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan LOCKOUT = TimeSpan.FromMinutes(15);
        private static readonly ConcurrentDictionary<string, LoginAttempt> _attempts = new ConcurrentDictionary<string, LoginAttempt>(System.StringComparer.OrdinalIgnoreCase);

        public AuthBLL() : this(new UserDAL()) { }

        public AuthBLL(IUserRepository userRepo)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        }

        /// <summary>
        /// Authenticates a user with the provided username and password.
        /// </summary>
        public UserDTO Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return null;

                username = username.Trim();

                // Check lockout status
                var att = _attempts.GetOrAdd(username, _ => new LoginAttempt());
                lock (att)
                {
                    if (att.LockedUntil.HasValue && DateTime.Now < att.LockedUntil.Value)
                    {
                        var remaining = att.LockedUntil.Value - DateTime.Now;
                        throw new InvalidOperationException($"Tài khoản tạm bị khóa. Vui lòng thử lại sau {Math.Ceiling(remaining.TotalMinutes)} phút.");
                    }
                    // Reset window if expired
                    if (att.FirstAttemptAt.HasValue && DateTime.Now - att.FirstAttemptAt.Value > WINDOW)
                    {
                        att.ResetWindow();
                    }
                }

                // Retrieve user from database
                UserDTO user = null;
                try
                {
                    user = _userRepo.GetUserWithRoles(username);
                }
                catch (Exception ex)
                {
                    LogMessage($"Database error retrieving user '{username}': {ex.Message}", EventLogEntryType.Error);
                    throw new InvalidOperationException("Không thể kết nối đến cơ sở dữ liệu.");
                }

                // Demo accounts (no lockout applied to built-in demo to ease testing)
                if (user == null)
                {
                    var u = username.ToLower();
                    if (u == "admin" && password == "123456")
                    {
                        _attempts.TryRemove(username, out _);
                        return new UserDTO
                        {
                            UserID = -1,
                            Username = "admin",
                            FullName = "Quản trị hệ thống",
                            IsActive = true,
                            Roles = new System.Collections.Generic.List<string> { "Admin" }
                        };
                    }
                    if (u == "staff" && password == "123456")
                    {
                        _attempts.TryRemove(username, out _);
                        return new UserDTO
                        {
                            UserID = -2,
                            Username = "staff",
                            FullName = "Nhân viên",
                            IsActive = true,
                            Roles = new System.Collections.Generic.List<string> { "Staff" }
                        };
                    }

                    // fail: user not found
                    RegisterFailedAttempt(username);
                    return null;
                }

                if (!user.IsActive)
                {
                    RegisterFailedAttempt(username);
                    return null;
                }

                // Verify password (PBKDF2 preferred, fallback legacy HMACSHA512)
                bool legacy;
                if (!VerifyPassword( password, user.PasswordSalt, user.PasswordHash, out legacy))
                {
                    RegisterFailedAttempt(username);
                    return null;
                }

                // Auto-migrate legacy hashes to PBKDF2 on successful login
                if (legacy)
                {
                    try { _userRepo.ChangePassword(user.UserID, password); } catch { /* ignore migrate errors */ }
                }

                // Successful login → clear attempts
                _attempts.TryRemove(username, out _);
                return user;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogMessage($"Unexpected error during login: {ex.Message}", EventLogEntryType.Error);
                throw new InvalidOperationException("Lỗi không mong muốn trong quá trình đăng nhập.");
            }
        }

        private void RegisterFailedAttempt(string username)
        {
            var att = _attempts.GetOrAdd(username, _ => new LoginAttempt());
            lock (att)
            {
                var now = DateTime.Now;
                if (!att.FirstAttemptAt.HasValue || (now - att.FirstAttemptAt.Value) > WINDOW)
                {
                    att.FirstAttemptAt = now;
                    att.Count = 1;
                }
                else
                {
                    att.Count++;
                }

                if (att.Count >= MAX_ATTEMPTS)
                {
                    att.LockedUntil = now.Add(LOCKOUT);
                    att.Count = 0;
                    att.FirstAttemptAt = null;
                    throw new InvalidOperationException($"Tài khoản tạm bị khóa {LOCKOUT.TotalMinutes} phút do đăng nhập sai nhiều lần.");
                }
            }
        }

        private class LoginAttempt
        {
            public int Count { get; set; }
            public DateTime? FirstAttemptAt { get; set; }
            public DateTime? LockedUntil { get; set; }
            public void ResetWindow()
            {
                Count = 0;
                FirstAttemptAt = null;
            }
        }

        /// <summary>
        /// Verifies if the provided password matches the stored hash.
        /// </summary>
        private bool VerifyPassword(string plainPassword, byte[] salt, byte[] hashFromDb, out bool legacy)
        {
            legacy = false;
            try
            {
                if (salt == null || salt.Length == 0) return false;
                if (hashFromDb == null || hashFromDb.Length == 0) return false;

                // Detect algorithm by salt/hash size (legacy HMACSHA512 uses 128-byte salt; PBKDF2 we set salt=16, hash=32)
                if (salt.Length == 16 && (hashFromDb.Length == 32 || hashFromDb.Length == 64))
                {
                    // PBKDF2-SHA1 default in .NET Framework's Rfc2898DeriveBytes
                    const int Iterations = 150000;
                    using (var pbkdf2 = new Rfc2898DeriveBytes(plainPassword, salt, Iterations))
                    {
                        var computed = pbkdf2.GetBytes(hashFromDb.Length);
                        int diff = 0;
                        for (int i = 0; i < computed.Length; i++) diff |= computed[i] ^ hashFromDb[i];
                        return diff == 0;
                    }
                }
                else
                {
                    // Legacy HMACSHA512
                    legacy = true;
                    using (var hmac = new HMACSHA512(salt))
                    {
                        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
                        if (computedHash.Length != hashFromDb.Length) return false;
                        int diff = 0;
                        for (int i = 0; i < computedHash.Length; i++) diff |= computedHash[i] ^ hashFromDb[i];
                        return diff == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error verifying password: {ex.Message}", EventLogEntryType.Error);
                return false;
            }
        }

        private void LogMessage(string message, EventLogEntryType type)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logMessage = $"[{timestamp}] {LogSource}: {message}";
                Debug.WriteLine(logMessage);
            }
            catch { }
        }
    }
}


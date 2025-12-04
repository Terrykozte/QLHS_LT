using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using QLTN_LT.DTO;
using QLTN_LT.DAL.Interfaces;

namespace QLTN_LT.DAL
{
    public class UserDAL : IUserRepository
    {
        public UserDTO GetUserWithRoles(string username)
        {
            UserDTO user = null;
            const string userSql = @"SELECT UserID, Username, PasswordHash, PasswordSalt, FullName, Email, Phone, IsActive, Roles 
                                     FROM dbo.Users WHERE Username = @username AND IsActive = 1";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(userSql, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        user = new UserDTO
                        {
                            UserID = (int)rd["UserID"],
                            Username = rd["Username"].ToString(),
                            PasswordHash = (byte[])rd["PasswordHash"],
                            PasswordSalt = (byte[])rd["PasswordSalt"],
                            FullName = rd["FullName"] as string,
                            Email = rd["Email"] as string,
                            Phone = rd["Phone"] as string,
                            IsActive = (bool)rd["IsActive"],
                        };

                        // Parse roles from comma-separated string if available
                        if (!rd.IsDBNull(rd.GetOrdinal("Roles")))
                        {
                            var roleStr = rd["Roles"].ToString();
                            if (!string.IsNullOrWhiteSpace(roleStr))
                            {
                                user.Roles = roleStr
                                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(r => r.Trim())
                                    .Where(r => r.Length > 0)
                                    .ToList();
                            }
                        }
                    }
                }
            }
            return user;
        }

        public List<UserDTO> GetAll()
        {
            var users = new List<UserDTO>();
            const string sql = "SELECT UserID, Username, FullName, Email, Phone, IsActive, Roles FROM dbo.Users";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var user = new UserDTO
                        {
                            UserID = (int)rd["UserID"],
                            Username = rd["Username"].ToString(),
                            FullName = rd["FullName"] as string,
                            Email = rd["Email"] as string,
                            Phone = rd["Phone"] as string,
                            IsActive = (bool)rd["IsActive"],
                        };
                        if (!rd.IsDBNull(rd.GetOrdinal("Roles")))
                        {
                            var roleStr = rd["Roles"].ToString();
                            if (!string.IsNullOrWhiteSpace(roleStr))
                            {
                                user.Roles = roleStr
                                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(r => r.Trim())
                                    .Where(r => r.Length > 0)
                                    .ToList();
                            }
                        }
                        users.Add(user);
                    }
                }
            }
            return users;
        }

        public bool Insert(UserDTO user, string password)
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            const string sql = @"INSERT INTO dbo.Users (Username, PasswordHash, PasswordSalt, FullName, Email, Phone, IsActive, Roles) 
                               VALUES (@Username, @PasswordHash, @PasswordSalt, @FullName, @Email, @Phone, @IsActive, @Roles)";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, passwordHash.Length).Value = passwordHash;
                cmd.Parameters.Add("@PasswordSalt", SqlDbType.VarBinary, passwordSalt.Length).Value = passwordSalt;
                cmd.Parameters.AddWithValue("@FullName", (object)user.FullName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                var roles = (user.Roles != null && user.Roles.Count > 0) ? string.Join(",", user.Roles) : (string)null;
                cmd.Parameters.AddWithValue("@Roles", (object)roles ?? DBNull.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(UserDTO user)
        {
            const string sql = @"UPDATE dbo.Users 
                               SET FullName = @FullName, Email = @Email, Phone = @Phone, IsActive = @IsActive, Roles = @Roles 
                               WHERE UserID = @UserID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@FullName", (object)user.FullName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                var roles = (user.Roles != null && user.Roles.Count > 0) ? string.Join(",", user.Roles) : (string)null;
                cmd.Parameters.AddWithValue("@Roles", (object)roles ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UserID", user.UserID);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int userId)
        {
            const string sql = "DELETE FROM dbo.Users WHERE UserID = @UserID";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ChangePassword(int userId, string newPassword)
        {
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            const string sql = "UPDATE dbo.Users SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt WHERE UserID = @UserID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, passwordHash.Length).Value = passwordHash;
                cmd.Parameters.Add("@PasswordSalt", SqlDbType.VarBinary, passwordSalt.Length).Value = passwordSalt;
                cmd.Parameters.AddWithValue("@UserID", userId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // PBKDF2 (Rfc2898DeriveBytes) - HMACSHA1 in .NET Framework, with high iterations
            const int Iterations = 150000; // adjust higher if acceptable
            const int SaltSize = 16; // 128-bit
            const int HashSize = 32; // 256-bit

            using (var rng = RandomNumberGenerator.Create())
            {
                passwordSalt = new byte[SaltSize];
                rng.GetBytes(passwordSalt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, passwordSalt, Iterations))
            {
                passwordHash = pbkdf2.GetBytes(HashSize);
            }
        }
    }
}

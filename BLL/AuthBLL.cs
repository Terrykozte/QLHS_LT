using System;
using System.Security.Cryptography;
using System.Text;
using QLTN_LT.DAL;
using QLTN_LT.DTO;
using QLTN_LT.DAL.Interfaces;

namespace QLTN_LT.BLL
{
    public class AuthBLL
    {
        private readonly IUserRepository _userRepo;

        public AuthBLL() : this(new UserDAL()) { }

        public AuthBLL(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public UserDTO Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

                        var user = _userRepo.GetUserWithRoles(username);

            if (user == null || !user.IsActive)
            {
                return null; // User not found or inactive
            }

            // Verify password
            if (VerifyPassword(password, user.PasswordSalt, user.PasswordHash))
            {
                return user; // Login successful
            }

            return null; // Password incorrect
        }

        private bool VerifyPassword(string plainPassword, byte[] salt, byte[] hashFromDb)
        {
            // Recreate the hash using the provided password and the salt from the database
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(plainPassword);
                var saltString = new Guid(salt).ToString(); // Convert salt bytes back to the same string format used in the DB script
                var combinedBytes = Encoding.UTF8.GetBytes(plainPassword + saltString);

                var computedHash = sha256.ComputeHash(combinedBytes);

                // Compare the computed hash with the hash from the database
                if (computedHash.Length != hashFromDb.Length)
                {
                    return false;
                }

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != hashFromDb[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}


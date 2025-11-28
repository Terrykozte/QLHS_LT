using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            const string userSql = "SELECT UserID, Username, PasswordHash, PasswordSalt, FullName, Email, Phone, IsActive FROM hs.Users WHERE Username = @username AND IsActive = 1";
            const string rolesSql = "SELECT r.RoleName FROM hs.Roles r JOIN hs.UserRoles ur ON r.RoleID = ur.RoleID WHERE ur.UserID = @userid";

            using (var conn = DatabaseHelper.CreateConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(userSql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
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
                                FullName = rd["FullName"].ToString(),
                                Email = rd["Email"].ToString(),
                                Phone = rd["Phone"].ToString(),
                                IsActive = (bool)rd["IsActive"]
                            };
                        }
                    }
                }

                if (user != null)
                {
                    using (var cmd = new SqlCommand(rolesSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@userid", user.UserID);
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                user.Roles.Add(rd["RoleName"].ToString());
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
            const string sql = "SELECT UserID, Username, FullName, Email, Phone, IsActive FROM hs.Users";

            using (var conn = DatabaseHelper.CreateConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            users.Add(new UserDTO
                            {
                                UserID = (int)rd["UserID"],
                                Username = rd["Username"].ToString(),
                                FullName = rd["FullName"].ToString(),
                                Email = rd["Email"].ToString(),
                                Phone = rd["Phone"].ToString(),
                                IsActive = (bool)rd["IsActive"]
                            });
                        }
                    }
                }
            }
            return users;
        }

        public bool Insert(UserDTO user, string password)
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            const string sql = @"INSERT INTO hs.Users (Username, PasswordHash, PasswordSalt, FullName, Email, Phone, IsActive) 
                               VALUES (@Username, @PasswordHash, @PasswordSalt, @FullName, @Email, @Phone, @IsActive)";

            using (var conn = DatabaseHelper.CreateConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@PasswordSalt", passwordSalt);
                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Phone", user.Phone);
                    cmd.Parameters.AddWithValue("@IsActive", user.IsActive);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Update(UserDTO user)
        {
            const string sql = @"UPDATE hs.Users 
                               SET FullName = @FullName, Email = @Email, Phone = @Phone, IsActive = @IsActive 
                               WHERE UserID = @UserID";

            using (var conn = DatabaseHelper.CreateConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Phone", user.Phone);
                    cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                    cmd.Parameters.AddWithValue("@UserID", user.UserID);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int userId)
        {
            const string sql = "DELETE FROM hs.Users WHERE UserID = @UserID";
            using (var conn = DatabaseHelper.CreateConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ChangePassword(int userId, string newPassword)
        {
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            const string sql = "UPDATE hs.Users SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt WHERE UserID = @UserID";

            using (var conn = DatabaseHelper.CreateConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@PasswordSalt", passwordSalt);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using QLTN_LT.DAL;
using QLTN_LT.DTO;
using QLTN_LT.DAL.Interfaces;

namespace QLTN_LT.BLL
{
    public class UserBLL
    {
        private readonly IUserRepository _userRepo;

        public UserBLL()
        {
            _userRepo = new UserDAL();
        }

        public List<UserDTO> GetAll()
        {
            return _userRepo.GetAll();
        }

        public bool Insert(UserDTO user, string password)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Username and Password are required.");

            return _userRepo.Insert(user, password);
        }

        public bool Update(UserDTO user)
        {
            return _userRepo.Update(user);
        }

        public bool Delete(int userId)
        {
            return _userRepo.Delete(userId);
        }

        public bool ChangePassword(int userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("New password is required.");
            
            return _userRepo.ChangePassword(userId, newPassword);
        }
    }
}

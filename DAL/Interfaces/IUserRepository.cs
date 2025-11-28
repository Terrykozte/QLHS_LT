using System.Collections.Generic;
using QLTN_LT.DTO;

namespace QLTN_LT.DAL.Interfaces
{
    public interface IUserRepository
    {
        UserDTO GetUserWithRoles(string username);
        List<UserDTO> GetAll();
        bool Insert(UserDTO user, string password);
        bool Update(UserDTO user);
        bool Delete(int userId);
        bool ChangePassword(int userId, string newPassword);
    }
}

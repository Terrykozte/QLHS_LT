using System.Collections.Generic;

namespace QLTN_LT.DTO
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; }

        public UserDTO()
        {
            Roles = new List<string>();
        }
    }
}


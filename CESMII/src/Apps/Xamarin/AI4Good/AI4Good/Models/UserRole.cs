using System;
namespace AI4Good.Models
{
    public class UserRole
    {
        public UserRole()
        {
        }

        public Guid UserRoleID { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public Guid ConcurrencyStamp { get; set; }
    }
}

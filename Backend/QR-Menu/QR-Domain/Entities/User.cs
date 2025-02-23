using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace QR_Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        public int RoleId { get; set; }
        public int? RefreshTokenId { get; set; }
        public Role Role { get; set; }

        public bool IsActive { get; set; } = true;

        public RefreshToken? RefreshToken { get; set; }

        public User(string userName, string name, int roleId)
        {
            UserName = userName;
            Name = name;
            RoleId = roleId;
        }

        public User()
        {
        }
    }
}

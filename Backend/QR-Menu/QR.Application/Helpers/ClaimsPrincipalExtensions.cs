using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using QR_Domain.Enums;

namespace QR.Application.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static Claims GetClams(this ClaimsPrincipal user)
        {
            return new Claims
            {
                UserId = int.Parse(user.FindFirst("userId")?.Value),
                Role = (Roles)Enum.Parse(typeof(Roles), user.FindFirst(ClaimTypes.Role)?.Value),
                Username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            };
        }
    }

    public class Claims
    {
        public int UserId { get; set; }
        public Roles Role { get; set; }
        public string Username { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QR_Domain.Enums;

namespace QR.Application.Helpers
{
    public static class RoleConstants
    {
        public const string SuperAdmin = nameof(Roles.SuperAdmin);
        public const string Admin = nameof(Roles.Admin);

        public static string GetRoleName(int roleId)
        {
            return Enum.GetName(typeof(Roles), roleId);
        }
    }
}

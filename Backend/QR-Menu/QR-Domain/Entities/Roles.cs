using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace QR_Domain.Entities
{
    public class Role : IdentityRole<int>
    {
        public ICollection<User> Users { get; set; } = new List<User>();
        public Role(int id, string name)
        {
            Id = id;
            Name = name;
            NormalizedName = name.ToUpper();
        }
        public Role()
        {
            
        }
    }

}

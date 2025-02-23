using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QR_Domain.Entities;

namespace QR_Domain.Interfaces
{
  public interface IUnitOfWork
    {
        public IBaseRepository<User> UserRepository { get; set;} 
        public IBaseRepository<Role> RoleRepository { get; set;}
        public IBaseRepository<RefreshToken> RefreshTokenRepository { get; set; }

        public Task<IDbTransaction> BeginTransaction();
        public Task SaveChangesAsync();
    }
}

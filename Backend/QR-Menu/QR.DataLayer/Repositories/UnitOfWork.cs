using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QR_Domain.Entities;
using QR_Domain.Interfaces;

namespace QR.DataLayer.Repositories
{
  public class UnitOfWork : IUnitOfWork
    {
        private readonly QRContext _context;

        public IBaseRepository<User> UserRepository { get ; set ; }
        public IBaseRepository<Role> RoleRepository { get ; set ; }
        public IBaseRepository<RefreshToken> RefreshTokenRepository { get; set; }

        public UnitOfWork(
            QRContext context,
            IBaseRepository<User> userRepository,
            IBaseRepository<Role> roleRepository,
            IBaseRepository<RefreshToken> refreshTokenRepository
            )
        {
            RefreshTokenRepository = refreshTokenRepository;
            UserRepository = userRepository;
            RoleRepository = roleRepository;
            _context = context; 
        }

        public Task SaveChangesAsync() =>
            _context.SaveChangesAsync();

        public async Task<IDbTransaction> BeginTransaction()
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            return transaction.GetDbTransaction();
        }


    }
}

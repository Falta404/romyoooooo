using Microsoft.EntityFrameworkCore;
using QR_Domain.Interfaces;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HR.Datalayer.Repositories
{
  public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly QRContext _context;

        public BaseRepository(QRContext context) =>
            _context = context;

        public async Task<bool> isExistAsync(Expression<Func<T, bool>> expression) =>
            await _context.Set<T>().AsNoTracking().AnyAsync(expression);
        public async Task AddAsync(T entity) =>
            await _context.Set<T>().AddAsync(entity);
        public async Task AddRangeAsync(List<T> entities) =>
            await _context.Set<T>().AddRangeAsync(entities);

        public async Task DeleteAsync(T entity) =>
            _context.Set<T>().Remove(entity);

        public async Task<T> FindAsync(Expression<Func<T, bool>> expression, string[] inludes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (inludes != null)
                foreach (var inlude in inludes)
                    query = query.Include(inlude);

            return await query.SingleOrDefaultAsync(expression);
        }


        private IQueryable<T> ApplyIncludes<T>(IQueryable<T> query, string[] includes) where T : class
        {
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query;
        }

        public async Task<IEnumerable<T>> GetAllAsync(string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            query = ApplyIncludes(query, includes);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllByAsync(Expression<Func<T, bool>> expression, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>().Where(expression);

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.AsSplitQuery().ToListAsync();
        }

        public int GetCounts() =>
             _context.Set<T>().Count();

        public async Task UpdateAsync(T entity) =>
            _context.Set<T>().Update(entity);

        public async Task UpdateAllAsync(IEnumerable<T> entities) =>
            _context.Set<T>().UpdateRange(entities);

        public async Task DeleteRangeAsync(IEnumerable<T> entites) =>
            _context.Set<T>().RemoveRange(entites);

    }
}

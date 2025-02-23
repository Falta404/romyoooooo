using System.Linq.Expressions;

namespace QR_Domain.Interfaces
{
  public interface IBaseRepository<T> where T : class
  {
    Task<T> FindAsync(Expression<Func<T, bool>> expression, string[] inludes = null);
    Task<bool> isExistAsync(Expression<Func<T, bool>> expression);
    Task<IEnumerable<T>> GetAllByAsync(Expression<Func<T, bool>> expression, string[] includes = null);
    Task<IEnumerable<T>> GetAllAsync(string[] includes = null);
    Task AddAsync(T entity);
    Task AddRangeAsync(List<T> entities);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(IEnumerable<T> entities);
    Task UpdateAllAsync(IEnumerable<T> entities);
    int GetCounts();
  }
}

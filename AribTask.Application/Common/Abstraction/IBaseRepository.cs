


using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace AribTask.Application.Common.Abstraction
{
    public interface IBaseRepository <T> where T : class
    {
      
        void Save();
        Task SaveAsync();
        T Remove(T entity);
        T Update(T entity);


        void RemoveRange(IEnumerable<T> entities);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, Expression<Func<T, IQueryable<T>>> select = null, Expression<Func<T, T>> selector = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Expression<Func<T, bool>> includeFilter = null, string includeProperties = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null, int? skip = null, int? take = null);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null, string includeProperties = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        Task<int> CountAsync(Expression<Func<T, bool>> filter = null, string includeProperties = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter = null);
        Task<bool> ExistAsync(Expression<Func<T, bool>> filter = null, string includeProperties = null);
        Task<IEnumerable<TType>> GetSpecificSelectAsync<TType>(Expression<Func<T, bool>> filter, Expression<Func<T, TType>> select, string includeProperties = null, int? skip = null, int? take = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null) where TType : class;
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(string id);

    }
}

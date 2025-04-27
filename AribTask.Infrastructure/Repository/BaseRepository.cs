
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using AribTask.Application.Common.Abstraction;
using AribTask.Domain.Data;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace AribTask.Infrastructure.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public readonly AribTaskDbContext context;
        public readonly DbSet<T> dbSet;
        //private readonly DbContext _ActiveContext;
        public BaseRepository(AribTaskDbContext _context)
        {
            context = _context;
            dbSet = context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null!,
            Expression<Func<T, IQueryable<T>>> select = null!,
            Expression<Func<T, T>> selector = null!,
            Func<IQueryable<T>,
            IOrderedQueryable<T>> orderBy = null!,
            Expression<Func<T, bool>> includeFilter = null!,
            string includeProperties = null!,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null!,
            int? skip = null,
            int? take = null)
        {
            IQueryable<T> query = dbSet;

            if (includeFilter is not null)
                query = query.Include(includeFilter);

            if (select != null)
                query = (IQueryable<T>)query.Select(select);
            if (filter != null)
                query = query.Where(filter);

            if (includeProperties != null)
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty).AsSplitQuery().AsNoTracking();
            if (includes != null)
                query = includes(query);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<TType>> GetSpecificSelectAsync<TType>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TType>> select,
            string includeProperties = null!,
            int? skip = null,
            int? take = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null!

    ) where TType : class
        {
            IQueryable<T> query = dbSet.AsNoTracking();

            if (includeProperties != null)
            {
                query.AsSplitQuery();
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty).IgnoreQueryFilters();
            }

            if (filter != null)
                query = query.Where(filter);
            if (orderBy != null)
                query = orderBy(query);

            if (skip.HasValue)
                query = query.Skip(skip.Value);
            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.Select(select).ToListAsync();
        }
        public async Task<T> GetFirstOrDefaultAsync(
                Expression<Func<T, bool>> filter = null!,
                string includeProperties = null!,
                Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null!,
                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null!)

        {
            IQueryable<T> query = dbSet;

            if (filter != null)
                query = query.Where(filter);


            if (includes != null)
                query = includes(query);

            if (includeProperties != null)
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty).AsSplitQuery().AsNoTracking();
            if (orderBy != null)
                return (await orderBy(query).FirstOrDefaultAsync())!;

            return (await query.FirstOrDefaultAsync())!;
        }

        public async Task<T> GetByIdAsync(int id) =>
                (await dbSet.FindAsync(id))!;
        public async Task<T> GetByIdAsync(string id) =>
                (await dbSet.FindAsync(id))!;

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter = null!, string includeProperties = null!)
        {
            IQueryable<T> query = dbSet.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);
            if (includeProperties != null)
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty);

            return await query.CountAsync();
        }
        public async Task<T> AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
            return entities;
        }
       
        public T Update(T entity)
        {
            dbSet.Update(entity);
            return entity;
        }
        public void UpdateRange(IEnumerable<T> entities) =>
        dbSet.UpdateRange(entities);

        public T Remove(T entity)
        {
            dbSet.Remove(entity);
            return entity;
        }
        public void RemoveRange(IEnumerable<T> entities) =>
            dbSet.RemoveRange(entities);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter = null!)
        {
            IQueryable<T> query = dbSet.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            return await query.AnyAsync();
        }
        public async Task<bool> ExistAsync(Expression<Func<T, bool>> filter = null!, string includeProperties = null!
  )
        {
            IQueryable<T> query = dbSet.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (includeProperties != null)
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty);

            return await query.FirstOrDefaultAsync() is not null;
        }

     
        public void Save()
        {
          
                context.SaveChangesAsync();
        }

        public  Task SaveAsync()
        {
         
                return context.SaveChangesAsync();
        }
    }
}

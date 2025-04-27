using AribTask.Application.Common.Abstraction;
using AribTask.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribTask.Infrastructure.Repository
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly AribTaskDbContext _context;

        public RepositoryFactory(AribTaskDbContext context)
        {
            _context = context;
        }

        public IBaseRepository<T> GetRepository<T>() where T : class
        {
            return new BaseRepository<T>(_context);
        }
    }
}

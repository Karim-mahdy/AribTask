using Microsoft.EntityFrameworkCore.Storage;
using AribTask.Application.Common.Abstraction;
using AribTask.Domain.Data;
using System.Net.Sockets;
using Task = System.Threading.Tasks.Task;
using AribTask.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AribTask.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AribTaskDbContext _context;
        private readonly IRepositoryFactory _repositoryFactory;
        private IDbContextTransaction _transaction;
        private bool _disposed;
        
        private IBaseRepository<Department> _departments;
        private IBaseRepository<Employee> _employees;
        private IBaseRepository<EmployeeTask> _employeeTasks;

        public UnitOfWork(AribTaskDbContext context, IRepositoryFactory repositoryFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
        }

        public IBaseRepository<Department> Departments => _departments ??= _repositoryFactory.GetRepository<Department>();
        
        public IBaseRepository<Employee> Employees => _employees ??= _repositoryFactory.GetRepository<Employee>();
        
        public IBaseRepository<EmployeeTask> EmployeeTasks => _employeeTasks ??= _repositoryFactory.GetRepository<EmployeeTask>();

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }

                _disposed = true;
            }
        }
    }
}



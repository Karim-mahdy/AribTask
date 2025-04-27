using Microsoft.EntityFrameworkCore.Storage;
using System.Net.Sockets;
using Task = System.Threading.Tasks.Task;
using AribTask.Domain.Models;

namespace AribTask.Application.Common.Abstraction
{
    public interface IUnitOfWork
    {
        IBaseRepository<Department> Departments { get; }
        IBaseRepository<Employee> Employees { get; }
        IBaseRepository<EmployeeTask> EmployeeTasks { get; }
        
        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

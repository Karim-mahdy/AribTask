using AribTask.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AribTask.Domain.Data
{
    public class AribTaskDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor accessor;
        public IConfiguration configuration { get; }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeTask> EmployeeTasks { get; set; }

        public AribTaskDbContext(DbContextOptions<AribTaskDbContext> options, IConfiguration configuration, IHttpContextAccessor accessor)
            : base(options)
        {
            this.configuration = configuration;
            this.accessor = accessor;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.SeedData();
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            modelBuilder.AddQueryFilterToAllEntitiesAssignableFrom<BaseEntity>(x => x.IsDeleted == false);
            modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()).ToList().ForEach(x => x.DeleteBehavior = DeleteBehavior.NoAction);

            // Configure the Employee-EmployeeTask relationship
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.AssignedTasks)
                .WithOne(t => t.Employee)
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            DateTime dateNow = DateTime.Now;
            string userId = accessor!.HttpContext == null
                ? string.Empty
                : accessor!.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified
                        || e.State == EntityState.Deleted)
            );

            foreach (var entityEntry in entries)
            {
                var entity = (BaseEntity)entityEntry.Entity;
                entity.UpdateDate = dateNow;
                entity.UpdateBy = userId;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.InsertDate = dateNow;

                    if (string.IsNullOrEmpty(entity.InsertBy))
                    {
                        entity.InsertBy = userId;
                    }
                }

                if (entityEntry.State == EntityState.Deleted)
                {
                    entityEntry.State = EntityState.Modified;
                    entity.DeleteDate = dateNow;
                    entity.DeleteBy = userId;
                    entity.IsDeleted = true;
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
    }

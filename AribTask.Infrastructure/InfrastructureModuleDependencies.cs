using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AribTask.Application.Common.Abstraction;
using AribTask.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AribTask.Infrastructure.Data;


namespace AribTask.Infrastructure
{
    public static class InfrastructureModuleDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            services.AddScoped<IDbInitializer, DbInitializer>();
            return services;
        }
    }
}

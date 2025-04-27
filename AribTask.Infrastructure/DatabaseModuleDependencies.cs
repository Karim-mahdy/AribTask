using AribTask.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AribTask.Domain.Models;

namespace AribTask.Infrastructure
{
    public static class DatabaseModuleDependencies
    {
        public static IServiceCollection AddDatabaseDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var connectioString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AribTaskDbContext>(options => options.UseSqlServer(connectioString));
            
            // Add Identity services
            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AribTaskDbContext>()
            .AddDefaultTokenProviders();
         
            return services;
        }
    }
}

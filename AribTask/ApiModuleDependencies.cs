using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using AribTask.Domain.Data;
using AribTask.Domain.Models;
using System.Drawing.Printing;
using System.Reflection;
using System.Text;

namespace OLX
{
    public static class ApiModuleDependencies
    {
        public static IServiceCollection AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // Assembly reference
            var assembly = Assembly.GetExecutingAssembly();

            // Add HttpContextAccessor first
            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddRazorPages(); // Add this line for Identity UI
            services.AddEndpointsApiExplorer();
                    

            // Register application services
            
            // Configure AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
  

            //services.AddControllers(options =>
            //{
            //    options.Filters.Add<CustomExceptionFilterAttribute>();
            //});

            return services;
        }
    }
}

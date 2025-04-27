using AribTask.Application;
using AribTask.Domain;
using AribTask.Infrastructure;
using OLX;
using Serilog;
using System.Configuration;
using AribTask.Application.Common.Extentions;
using AribTask.Domain.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AribTask.Domain.Models;
using AribTask.Application.Common.Abstraction;

namespace AribTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
           
          
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSignalR(); // Registers SignalR services
                                           // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            builder.Host.UseSerilog();

            // Add services to the container.
            var configuration = builder.Configuration;
            // Add services to the container.
            builder.Services
                .AddApiDependencies(configuration)
                .AddInfrastructureDependencies(configuration)
                .AddDatabaseDependencies(configuration)
                .AddDomainDependencies(configuration)
                .AddApplicationDependencies(configuration);
                
            var app = builder.Build();
            
          
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // Initialize the database
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    initializer.InitializeAsync().Wait();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the database.");
                }
            }
            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapRazorPages();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

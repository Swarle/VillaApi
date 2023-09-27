using DataLayer.Context;
using DataLayer.Models;
using DataLayer.Repository;
using DataLayer.Repository.Interfaces;
using DataLayer.UnitOfWork;
using DataLayer.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using PLL.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace PLL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
                .WriteTo
                .Console(outputTemplate: "{Timestamp:HH:mm:ss}  [{Level:}] {SourceContext}  \n{Message:l}\n{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog(logger);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IRepository<Villa>, Repository<Villa>>();
            builder.Services.AddScoped<IRepository<Orders>, OrderRepository>();
            builder.Services.AddScoped<IRepository<OrderStatus>, Repository<OrderStatus>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
            builder.Services.AddScoped<IRepository<Users>, UserRepository>();
            builder.Services.AddScoped<IRepository<VillaDetails>,VillaDetailsRepository>();
            builder.Services.AddScoped<IRepository<VillaStatus>, Repository<VillaStatus>>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            builder.Services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

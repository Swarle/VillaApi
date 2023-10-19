using System.Collections.Concurrent;
using System.Net;
using System.Security.Claims;
using System.Text;
using BusinessLogicLayer;
using BusinessLogicLayer.Infastructure;
using BusinessLogicLayer.Infrastructure;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;
using DataLayer.Context;
using DataLayer.Models;
using DataLayer.Repository;
using DataLayer.Repository.Interfaces;
using DataLayer.UnitOfWork;
using DataLayer.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using PLL.Controllers;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Utility;

namespace PLL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
                .MinimumLevel.Warning()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore",LogEventLevel.Information)
                .WriteTo
                .Console(outputTemplate: "{Timestamp:HH:mm:ss}  [{Level:}] {SourceContext}  \n{Message:l}\n{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog(logger);

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .SelectMany(m => m.Value.Errors)
                            .Select(m => m.ErrorMessage)
                            .ToList();

                        var response = new ApiResponse
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ErrorMessage = errors
                        };

                        return new BadRequestObjectResult(response);
                    };
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IRepository<Villa>, VillaRepository>();
            builder.Services.AddScoped<IRepository<Orders>, OrderRepository>();
            builder.Services.AddScoped<IRepository<OrderStatus>, Repository<OrderStatus>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
            builder.Services.AddScoped<IRepository<Users>, UserRepository>();
            builder.Services.AddScoped<IRepository<VillaDetails>,Repository<VillaDetails>>();
            builder.Services.AddScoped<IRepository<VillaStatus>, Repository<VillaStatus>>();
            

            builder.Services.AddSingleton<JwtTokenHandler>();

            builder.Services.AddAutoMapper(typeof(MappingConfig));

            builder.Services.AddScoped<IVillaService,VillaService>();
            builder.Services.AddScoped<IUserService,UserService>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            builder.Services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
            });

            var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                        ValidateIssuer = false,
                        ValidateAudience = false

                    };
                });

            builder.Services.AddAuthorization(c =>
            {
                c.AddPolicy("AdminPolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.Role, RolesSD.Admin);
                });
            });
            
            builder.Services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Villa_API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new string[] {}
                    }
                });
            });


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapControllers().AllowAnonymous();
            }
            else
            {
                app.MapControllers();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

           
            
            await app.RunAsync();
        }
    }
}

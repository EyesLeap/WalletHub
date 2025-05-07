using System.Text;
using api.BackgroundServices;
using api.Caching;
using api.Data;
using api.Interfaces;
using api.Models;
using api.Repository;
using api.Service;
using api.Validators.Portfolio;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPortfolioRepository, PortfolioRepository>();
            services.AddScoped<IAssetRepository, AssetRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IPortfolioSnapshotRepository, PortfolioSnapshotRepository>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
           
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPortfolioService, PortfolioService>();
            services.AddScoped<IAssetService, AssetService>();
            services.AddScoped<ICoinMarketCapService, CoinMarketCapService>();
            services.AddScoped<IPortfolioSnapshotService, PortfolioSnapshotService>();
            services.AddScoped<ICoinMarketCapService, CoinMarketCapService>();
            services.AddHttpClient<ICoinMarketCapService, CoinMarketCapService>();

            return services;
        }

        public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddHangfireServer();

            return services;
           
        }

        public static IServiceCollection RegisterRecurringJobs(this IServiceCollection services)
        {
            
            var optionsUtc = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            };

            RecurringJob.AddOrUpdate<PortfolioSnapshotJob>(
                "every-3-hours-snapshot",
                job => job.CreateSnapshotsAsync(),
                "0 */3 * * *",
                optionsUtc
            );


            return services;
           
        }

        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
           
            var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6379"; 
            
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;  
                options.InstanceName = "WalletHub:";      
            });
            services.AddSingleton<ICacheService, RedisCacheService>();

            return services;
        }
        public static IServiceCollection AddDecorators(this IServiceCollection services)
        {
            services.Decorate<ICoinMarketCapService, CachedCoinMarketCapService>();
            services.Decorate<IPortfolioService, CachedPortfolioService>();

            return services;
        }

        public static IServiceCollection AddSwaggerSetup(this IServiceCollection services)
        {
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"])
                    )
                };
            });

            return services;
        }

        public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CreatePortfolioDtoValidator>(); //Registers all validators from the assembly

            return services;

        }

        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDBContext>();

            return services;
        }
    }
}
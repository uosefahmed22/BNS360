using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Text;
using System;
using BNS360.Apis.Helpers;
using BNS360.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json;
using BNS360.Core.Models.Auth;
using BNS360.Core.IServices.Auth;
using BNS360.Repository.Services;
using BNS360.Core.IServices;
using BNS360.Core.IRepository;
using BNS360.Repository.Repository;

namespace BNS360.Apis.Extentions
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            var key = Encoding.ASCII.GetBytes(configuration["JwtConfig:Secret"]);

            var tokenvalidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                RequireExpirationTime = false,
                ClockSkew = TimeSpan.Zero
            };

            services.AddSingleton(tokenvalidationParameters);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(jwt =>
           {
               jwt.SaveToken = true;
               jwt.TokenValidationParameters = tokenvalidationParameters;
               jwt.Events = new JwtBearerEvents
               {
                   OnChallenge = context =>
                   {
                       context.HandleResponse();
                       context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                       context.Response.ContentType = "application/json";
                       var result = JsonSerializer.Serialize(new
                       {
                           StatusCode = StatusCodes.Status401Unauthorized,
                           Message = "You are not authorized to access this resource."
                       });
                       return context.Response.WriteAsync(result);
                   },
                   OnForbidden = context =>
                   {
                       context.Response.StatusCode = StatusCodes.Status403Forbidden;
                       context.Response.ContentType = "application/json";
                       var result = JsonSerializer.Serialize(new
                       {
                           StatusCode = StatusCodes.Status403Forbidden,
                           Message = "You do not have permission to access this resource."
                       });
                       return context.Response.WriteAsync(result);
                   }
               };
           });


            services.AddIdentity<AppUser, IdentityRole>(options =>
                options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<AppDbContext>();

            //services.AddControllers().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            //    options.JsonSerializerOptions.WriteIndented = true;
            //});

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
            });


            services.AddEndpointsApiExplorer();

            // Configure Swagger using the extension method
            services.AddSwaggerDocumentation();
            // Add Memory Cache
            services.AddMemoryCache();
            //configure Auto Mapper
            services.AddAutoMapper(typeof(MappingProfile));
            //Cloudinary Configuration
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySetting"));

            services.AddSingleton(cloudinary =>
            {
                var config = configuration.GetSection("CloudinarySetting").Get<CloudinarySettings>();
                var account = new CloudinaryDotNet.Account(config.CloudName, config.ApiKey, config.ApiSecret);
                return new CloudinaryDotNet.Cloudinary(account);
            });

            // Add custom services
            services.AddScoped<IAuthService,AuthService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IBusinessRepository, BusinessRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICraftRepository, CraftRepository>();
            services.AddScoped<ICraftsMenRepository, CraftsMenRepository>();
            services.AddScoped<IFavoriteBusinessRepository, FavoriteBusinessRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<ISavedJobsRepository, SavedJobsRepository>();

            // Configure CORS using the extension method
            services.ConfigureCors();

            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

            // Add custom error handling for model validation
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new
                        {
                            Field = e.Key,
                            ErrorMessages = e.Value.Errors.Select(x => x.ErrorMessage).ToArray()
                        }).ToArray();

                    var result = new
                    {
                        Message = "Validation failed",
                        Errors = errors
                    };

                    return new BadRequestObjectResult(result);
                };
            });
        }
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Open", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }
        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            });
            });
        }
    }
}

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
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using BNS360.Apis.Filters;

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

            var jwtConfig = configuration.GetRequiredSection("JwtConfig").Get<JwtConfig>()
                ?? throw new InvalidOperationException("JwtConfig is missing.");
            if (Encoding.UTF8.GetByteCount(jwtConfig.Secret) < 32
                || string.IsNullOrWhiteSpace(jwtConfig.Issuer)
                || string.IsNullOrWhiteSpace(jwtConfig.Audience))
            {
                throw new InvalidOperationException("JwtConfig must contain a 32-byte secret, issuer, and audience.");
            }
            var key = Encoding.UTF8.GetBytes(jwtConfig.Secret);

            var tokenvalidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtConfig.Audience,
                ValidateLifetime = true,
                RequireExpirationTime = true,
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
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.User.RequireUniqueEmail = true;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<AppDbContext>();

            //services.AddControllers().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            //    options.JsonSerializerOptions.WriteIndented = true;
            //});

            services.AddControllers(options =>
                {
                    options.Filters.Add<ApiResponseStatusCodeFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.WriteIndented = true;
                });


            services.AddEndpointsApiExplorer();

            // Configure Swagger using the extension method
            services.AddSwaggerDocumentation();
            // Add Memory Cache
            services.AddMemoryCache();
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsJsonAsync(
                        new
                        {
                            StatusCode = StatusCodes.Status429TooManyRequests,
                            Message = "Too many requests. Please try again later."
                        },
                        cancellationToken);
                };

                options.AddPolicy("auth", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

                options.AddPolicy("email", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(10),
                            QueueLimit = 0
                        }));
            });
            //Cloudinary Configuration
            services.AddOptions<CloudinarySettings>()
                .Bind(configuration.GetSection("CloudinarySetting"))
                .Validate(settings =>
                    !string.IsNullOrWhiteSpace(settings.CloudName)
                    && !string.IsNullOrWhiteSpace(settings.ApiKey)
                    && !string.IsNullOrWhiteSpace(settings.ApiSecret),
                    "CloudinarySetting is incomplete.")
                .ValidateOnStart();

            services.AddSingleton(serviceProvider =>
            {
                var config = serviceProvider
                    .GetRequiredService<Microsoft.Extensions.Options.IOptions<CloudinarySettings>>()
                    .Value;
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
            services.AddScoped<IPropertyRepository, PropertyRepository>();

            // Configure CORS using the extension method
            services.ConfigureCors(configuration);

            services.AddOptions<MailSettings>()
                .Bind(configuration.GetSection(nameof(MailSettings)))
                .Validate(settings =>
                    !string.IsNullOrWhiteSpace(settings.Email)
                    && !string.IsNullOrWhiteSpace(settings.Password)
                    && !string.IsNullOrWhiteSpace(settings.SmtpServer)
                    && settings.Port is > 0 and <= 65535,
                    "MailSettings is incomplete.")
                .ValidateOnStart();

            // Add custom error handling for model validation
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .Select(e => new
                        {
                            Field = e.Key,
                            ErrorMessages = e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
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
        public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

            services.AddCors(options =>
            {
                options.AddPolicy("Open", builder =>
                {
                    if (allowedOrigins.Length > 0)
                    {
                        builder.WithOrigins(allowedOrigins);
                    }
                    else
                    {
                        builder.SetIsOriginAllowed(_ => false);
                    }

                    builder.AllowAnyMethod().AllowAnyHeader();
                });
            });
        }
        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter the JWT access token only. Scalar adds the Bearer prefix automatically.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
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

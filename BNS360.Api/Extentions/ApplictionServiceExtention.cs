using BNS360.Core.Abstractions;
using BNS360.Core.Errors;
using BNS360.Core.Helpers.Settings;
using BNS360.Core.Services.AppBusniss;
using BNS360.Core.Services.Authentication;
using BNS360.Core.Services.Shared;
using BNS360.Reposatory.Data.AppBusniss;
using BNS360.Reposatory.Repositories.Authentication;
using BNS360.Reposatory.Repositories.Repositories;
using BNS360.Reposatory.Repositories.Shared;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BNS360.Api.Extentions
{
    public static class ApplictionServiceExtention
    {
        public static IServiceCollection AddAplictionService(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();  
            var config = provider.GetService<IConfiguration>();

            services.AddControllers().ConfigureApiBehaviorOptions(options =>

            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Where(e => e.Value is not null && e.Value.Errors.Any())
                .ToDictionary
                (
                    e => e.Key,
                    e => e.Value!.Errors.Select(error => error.ErrorMessage).ToList()
                );
                return new BadRequestObjectResult(new ApiValidationResponse(errors).Errors);
            }
            );





            services.AddDbContext<AppBusnissDbContext>(options =>
              options.UseSqlServer(
                  config?.GetConnectionString("BusnissDbConnection"),
                  sqlServerOptionsAction: sqlOptions =>
                  {
                      sqlOptions.EnableRetryOnFailure();
                  })
              );
            services.AddScoped<IReviewService,ReviewService>();
            services.AddScoped<IBusnissRepository,BusnissRepository>();
            services.AddSingleton<IJwtGenerator, JwtGenerator>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;

        }
    }
}

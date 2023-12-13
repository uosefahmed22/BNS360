using BNS360.Core.Errors;
using BNS360.Core.Services;
using BNS360.Reposatory.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BNS360.Api.Extentions
{
    public static class ApplictionServiceExtention
    {
        public static IServiceCollection AddAplictionService(this IServiceCollection service)
        {
            service.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(entry => entry.Value.Errors.Any())
                            .ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                            );

                        return new BadRequestObjectResult(new ApiValidationErrorResponse(400, null, errors));
                    };
                });
            service.AddScoped<IOtpService, OtpService>();
            service.AddScoped<IEmailService, EmailService>();
            service.AddScoped<IAuthService, AuthService>();
        
            return service;

        }
    }
}

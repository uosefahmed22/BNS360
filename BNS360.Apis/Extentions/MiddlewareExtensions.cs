namespace BNS360.Apis.Extentions
{
    using Scalar.AspNetCore;

    public static class MiddlewareExtensions
    {
        public static void ConfigureMiddleware(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(options =>
                    options.RouteTemplate = "openapi/{documentName}.json");
                app.MapScalarApiReference(options => options
                    .WithTitle("BNS360 API")
                    .WithOpenApiRoutePattern("/openapi/{documentName}.json"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseRateLimiter();
            app.UseCors("Open");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}

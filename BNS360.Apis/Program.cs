using BNS360.Apis.Extentions;
using BNS360.Apis.Exceptions;
using Serilog;

namespace BNS360.Apis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((context, services, configuration) => configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext());
            builder.Services.ConfigureServices(builder.Configuration);


            var app = builder.Build();

            app.UseSerilogRequestLogging();
            app.UseMiddleware<GlobalExceptionHandler>();
            app.ConfigureMiddleware();

            app.Run();
        }
    }
}

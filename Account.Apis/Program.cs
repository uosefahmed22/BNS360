using Account.Apis.Errors;
using Account.Apis.Extentions;
using Account.Apis.Helpers;
using Account.Core.Models.Account;
using Account.Reposatory.Data.Context;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Win32;
using System.Text.Json.Serialization;

namespace Account.Apis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region configure service

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));

            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySetting"));

            builder.Services.AddSingleton(cloudinary =>
            {
                var config = builder.Configuration.GetSection("CloudinarySetting").Get<CloudinarySettings>();
                var account = new CloudinaryDotNet.Account(config.CloudName, config.ApiKey, config.ApiSecret);
                return new CloudinaryDotNet.Cloudinary(account);
            });


            builder.Services.AddIdentityServices(builder.Configuration);

            builder.Services.AddSwaggerService();
            builder.Services.AddAplictionService();
            builder.Services.AddMemoryCache();
            builder.Services.AddCors(Options =>
            {
                Options.AddPolicy("MyPolicy", Options =>
                {
                    Options.AllowAnyHeader().
                    AllowAnyMethod()
                    .AllowAnyOrigin();
                });
            });

            #endregion
            var app = builder.Build();

            #region Update automatically
            //// Create a service scope to resolve services


            //using var scope = app.Services.CreateScope();
            //var Services = scope.ServiceProvider;

            //// Obtain logger factory to create loggers
            //var loggerfactory = Services.GetRequiredService<ILoggerFactory>();
            //try
            //{
            //    // Get the database context for Identity
            //    var identityDbContext = Services.GetRequiredService<AppDBContext>();

            //    // Apply database migration asynchronously
            //    await identityDbContext.Database.MigrateAsync();

            //    // Get the UserManager service to manage users
            //    var usermanager = Services.GetRequiredService<UserManager<AppUser>>();

            //    // Seed initial user data for the Identity context
            //    //await AppIdentityDbContextSeed.SeedUserAsync(usermanager);
            //}
            //catch (Exception ex)
            //{
            //    // If an exception occurs during migration or seeding, log the error
            //    var logger = loggerfactory.CreateLogger<Program>();
            //    logger.LogError(ex, "An Error Occurred During Applying The Migrations");
            //}
            #endregion


            #region configure middlewares
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseMiddleware<ExeptionMiddleWares>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddlewares();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("MyPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            #endregion
            app.Run();
        }
    }
}

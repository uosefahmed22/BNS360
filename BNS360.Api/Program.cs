using BNS360.Api.Extentions;
using BNS360.Api.Midlewares;
using BNS360.Core.Helpers.Settings;
using BNS360.Reposatory.Data.AppBusniss;
using BNS360.Reposatory.Data.AppBusniss.DataSeeding;
using BNS360.Reposatory.Data.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

#region configure service
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
builder.Services.Configure<FileUploadSettings>(builder.Configuration.GetSection(nameof(FileUploadSettings))); 
builder.Services.AddAplictionService();
builder.Services.AddSwaggerService();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<BNS360IdentityDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Defualt")));
#endregion

var app = builder.Build();

#region update udmatecaly

using var scope = app.Services.CreateScope();
var Services = scope.ServiceProvider;

var loggerfactory = Services.GetRequiredService<ILoggerFactory>();
try
{
    var identityContext = Services.GetRequiredService<BNS360IdentityDbContext>();
    await identityContext.Database.MigrateAsync();
    var context = Services.GetRequiredService<AppBusnissDbContext>();
    //await context.Database.EnsureDeletedAsync();
    await context.Database.MigrateAsync();
    await SeedData.Seed(context);
}
catch (Exception ex)
{
    var logger = loggerfactory.CreateLogger<Program>();
    logger.LogError(ex, "An Error Occured During Appling The Migrations");
}

#endregion
#region configure middlewares


if (app.Environment.IsDevelopment())
{
    app.UseSwaggerMiddlewares();
}
app.UseStatusCodePagesWithReExecute("/errors/{0}");
app.UseMiddleware<ExeptionMiddleWares>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseResponseCompression();
app.UseMiddleware<DecompressionMiddleware>();
app.UseStaticFiles();
#endregion
app.Run();
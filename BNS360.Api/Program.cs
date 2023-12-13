using BNS360.Api.Extentions;
using BNS360.Api.Midlewares;
using BNS360.Core.Entities.Identity;
using BNS360.Core.Errors;
using BNS360.Core.Services;
using BNS360.Reposatory.Data.Identity;
using BNS360.Reposatory.Repositories;
using MailKit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region configure service

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
    var identityDbContext = Services.GetRequiredService<BNS360IdentityDbContext>();
    await identityDbContext.Database.MigrateAsync();

}
catch (Exception ex)
{
    var logger = loggerfactory.CreateLogger<Program>();
    logger.LogError(ex, "An Error Occured During Appling The Migrations");
}
#endregion
#region configure middlewares

app.UseMiddleware<ExeptionMiddleWares>();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerMiddlewares();
}

app.UseStatusCodePagesWithReExecute("/errors/{0}");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
#endregion
app.Run();
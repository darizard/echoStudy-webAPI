using echoStudy_webAPI.Areas.Identity.Data;
using echoStudy_webAPI.Data;
using echoStudy_webAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace echoStudy_webAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await CreateDbIfNotExists(host);

            host.Run();
        }

        private static async Task CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var echoContext = services.GetRequiredService<EchoStudyDB>();
                    var identityContext = services.GetRequiredService<EchoStudyUsersRolesDB>();
                    var userManager = services.GetService<UserManager<EchoUser>>();

                    DbInitializer.CreateEchoStudyDB(echoContext, identityContext);
                    await IdentityInitializer.Initialize(host);
                    await DbInitializer.InitializeEchoStudyDb(echoContext, userManager);

                    await echoContext.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

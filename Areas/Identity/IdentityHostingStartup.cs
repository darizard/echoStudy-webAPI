using System;
using echoStudy_webAPI.Areas.Identity.Data;
using echoStudy_webAPI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(echoStudy_webAPI.Areas.Identity.IdentityHostingStartup))]
namespace echoStudy_webAPI.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<EchoStudyUsersRolesDB>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DefaultConnection")));

                services.AddDefaultIdentity<EchoUser>(options => { })
                .AddRoles<IdentityRole>()
                .AddSignInManager<SignInManager<EchoUser>>()
                .AddEntityFrameworkStores<EchoStudyUsersRolesDB>();

                services.AddMvc();
            });
        }
    }
}
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Gladiator.Areas.Identity.IdentityHostingStartup))]
namespace Gladiator.Areas.Identity
{
	public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                /*
                services.AddDbContext<GladiatorContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("GladiatorContextConnection")));
                /*
                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<GladiatorContext>();
                */
            });
        }
    }
}
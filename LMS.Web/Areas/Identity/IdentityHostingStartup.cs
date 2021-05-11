using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(LMS.Web.Areas.Identity.IdentityHostingStartup))]
namespace LMS.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}

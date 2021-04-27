using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Data.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LMS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<LMSWebContext>();
                //context.Database.EnsureDeleted();
                context.Database.Migrate();

                //dotnet user-sercrets set "AdminPW" "Group3!"
                var confiq = services.GetRequiredService<IConfiguration>();

                var adminPW = confiq["AdminPW"];

                try
                {
                    SeedData.InitAsync(services, adminPW).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex.Message, "Seed Has Failed!");
                    throw;
                }
            }


            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

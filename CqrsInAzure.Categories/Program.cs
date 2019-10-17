using System.Threading.Tasks;
using CqrsInAzure.Categories.Storage;
using CqrsInAzure.Categories.Telemetry;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CqrsInAzure.Categories
{
    public class Program
    {
        private static readonly string ApplicationInsightsInstrumentationKey = "4253ea6a-d8dc-4685-9791-410439ac1379";

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ApplicationInsights(ApplicationInsightsInstrumentationKey, new CustomTelemetryConverter())
                .CreateLogger();

            var host = CreateHostBuilder(args).Build();
            Log.Information("Starting web host");

            using (var scope = host.Services.CreateScope())
            {
                var storage = scope.ServiceProvider.GetService<ICategoriesStorage>();
                await DataSeeder.Seed(storage);
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
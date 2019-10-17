using CqrsInAzure.Search.Telemetry;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CqrsInAzure.Search
{
    public class Program
    {
        private static readonly string ApplicationInsightsInstrumentationKey = "1f6af6df-3e13-4415-a73f-15129aef1b21";

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ApplicationInsights(ApplicationInsightsInstrumentationKey, new CustomTelemetryConverter())
                .CreateLogger();

            var host = CreateHostBuilder(args).Build();
            Log.Information("Starting web host");
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
using CqrsInAzure.Candidates.Telemetry;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace CqrsInAzure.Candidates
{
    public class Program
    {
        private static readonly string ApplicationInsightsInstrumentationKey = "c25fe893-1f0b-4872-9be7-db2c1fe30964";

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
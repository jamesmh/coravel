using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Coravel;

namespace WorkerScheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Changed to return the IHost
            // builder before running it.
            IHost host = CreateHostBuilder(args).Build();
            host.Services.UseScheduler(scheduler => {
                // Easy peasy 👇
                scheduler
                    .Schedule<MyFirstInvocable>()
                    .EveryFiveSeconds()
                    .Weekday();
            });
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddScheduler();
                    // Add this 👇
                    services.AddTransient<MyFirstInvocable>();
                });
        };
}

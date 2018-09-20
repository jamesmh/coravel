using System;
using Microsoft.Extensions.DependencyInjection;
using Coravel.Queuing.Interfaces;
using Coravel;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace HostBuilderWithQueue
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddEnvironmentVariables(prefix: "PREFIX_");
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddQueue();
                    services.AddScheduler();
                })
                .UseConsoleLifetime()
                .Build();

            host.Services.GetRequiredService<IQueue>().QueueAsyncTask(async () =>
            {
                await Task.Delay(1000);
                Console.WriteLine("This was queued.");
            });

            host.Services.UseScheduler(s =>
                s.Schedule(() => Console.WriteLine("This was scheduled.")).EveryMinute()
            );

            using (var disposableHost = host)
            {

                await disposableHost.StartAsync();

                Console.WriteLine("This runs for five minutes then shuts down.");
                await Task.Delay(60000 * 5);

                await disposableHost.StopAsync();
            }
        }
    }
}

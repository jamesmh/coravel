﻿using System;
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
        static void Main(string[] args)
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
                    // Add Coravel's Scheduling...
                    services.AddScheduler();
                })
                .Build();

            // Configure the scheduled tasks....
            host.Services.UseScheduler(scheduler =>
                scheduler
                    .Schedule(() => Console.WriteLine("This was scheduled every minute."))
                    .EveryMinute()
            );

            // Run it!
            host.Run();
        }
    }
}

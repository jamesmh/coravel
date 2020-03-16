using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.ObjectPool;

namespace Coravel.Mailer.Mail.Renderers
{
    /// <summary>
    /// Provides methods to generate an instance of the RazorRenderer.
    /// 
    /// Note: Coravel uses this vs building an instance of RazorRenderer from the outer app's
    /// container. Coravel needs the viewer renderer to be a singleton so that we can queue mail properly.
    /// Dependencies of the RazorRenderer are scoped etc. so an instance of RazorRenderer will be null when running
    /// a queued Mail item (since the scope no longer exists an dependencies were Disposed).
    /// </summary>
    public class RazorRendererFactory
    {
        /// <summary>
        /// Get an instance of RazorRenderer. 
        /// The returned instance should be cached by the caller as this is a costly operation.
        /// </summary>
        /// <returns></returns>
        public static RazorRenderer MakeInstance(IConfiguration config)
        {
            var services = new ServiceCollection();
            string appDirectoryPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string appDirectory2 = Directory.GetCurrentDirectory();

            Console.WriteLine(appDirectoryPath);
            Console.WriteLine(appDirectory2);

            var webhostBuilder = new WebHostBuilder().ConfigureServices(services =>
            {
                services.AddSingleton(config);

                services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
                {
                    options.FileProviders.Clear();
                    options.FileProviders.Add(new PhysicalFileProvider(appDirectoryPath));
                });

                services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

                var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
                services.AddSingleton(diagnosticSource);
                services.AddSingleton<DiagnosticSource>(diagnosticSource);

                services.AddLogging();
                services.AddMvc();
                services.AddSingleton<RazorRenderer>();
            }).UseStartup<DummyStartup>().Build();

            return webhostBuilder.Services.GetRequiredService<RazorRenderer>();
        }

        public class DummyStartup
        {
            public void Configure()
            {

            }
        }
    }
}
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;

namespace Coravel.Mail.Renderers
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
            services.AddSingleton<IConfiguration>(config);
            var applicationEnvironment = PlatformServices.Default.Application;
            services.AddSingleton(applicationEnvironment);

            var appDirectory = Directory.GetCurrentDirectory();

            var environment = new HostingEnvironment
            {
                ApplicationName = Assembly.GetEntryAssembly().GetName().Name
            };
            services.AddSingleton<IHostingEnvironment>(environment);

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(new PhysicalFileProvider(appDirectory));
            });

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);

            services.AddLogging();
            services.AddMvc();
            services.AddSingleton<RazorRenderer>();
            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<RazorRenderer>();
        }
    }
}
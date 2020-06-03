using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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
            services.AddSingleton(config);

            var appDirectory = Directory.GetCurrentDirectory();
            var webRootDirectory = GetWebRootPath(appDirectory);
            var baseFileProvider = new PhysicalFileProvider(appDirectory);
            var webrootFileProvider = new PhysicalFileProvider(webRootDirectory);

            var viewAssemblyFiles = Directory.GetFiles(appDirectory, "*.Views.dll");
            var viewAssemblies = viewAssemblyFiles.Select(file => Assembly.LoadFile(file));

            var environment = new DummyWebHostEnvironment
            {
                ApplicationName = Assembly.GetEntryAssembly().GetName().Name,
                ContentRootPath = appDirectory,
                EnvironmentName = "CoravelMailer",
                ContentRootFileProvider = baseFileProvider,
                WebRootFileProvider = webrootFileProvider,
                WebRootPath = webRootDirectory
            };

            services.AddSingleton<IWebHostEnvironment>(environment);
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            var diagnosticListener = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticListener);
            services.AddSingleton(diagnosticListener);
            services.AddLogging();
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            var builder = services.AddMvcCore().AddRazorViewEngine();

            foreach (var viewAssembly in viewAssemblies)
            {
                builder.PartManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(viewAssembly));
            }

            services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            {
                options.FileProviders.Add(baseFileProvider);
            });

            services.AddSingleton<RazorRenderer>();

            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<RazorRenderer>();
        }

        private static string GetWebRootPath(string appDirectory)
        {
            var webroot = Path.Combine(appDirectory, "wwwroot");
            return Directory.Exists(webroot)
                ? webroot
                : appDirectory;
        }

        public class DummyWebHostEnvironment : IWebHostEnvironment
        {
            public IFileProvider WebRootFileProvider { get; set; }
            public string WebRootPath { get; set; }
            public string ApplicationName { get; set; }
            public IFileProvider ContentRootFileProvider { get; set; }
            public string ContentRootPath { get; set; }
            public string EnvironmentName { get; set; }
        }
    }
}
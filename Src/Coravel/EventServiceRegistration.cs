using System;
using System.Linq;
using System.Reflection;
using Coravel.Events;
using Coravel.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel
{
    public static class EventServiceRegistration
    {
        public static IServiceCollection AddEvents(this IServiceCollection services)
        {
            services.AddSingleton<IDispatcher>(p =>
                new Dispatcher(p.GetRequiredService<IServiceScopeFactory>()));
            return services;
        }

        public static IEventRegistration ConfigureEvents(this IServiceProvider provider)
        {
            var dispatcher = provider.GetRequiredService<IDispatcher>();
            if(dispatcher is Dispatcher unboxed) {
                return unboxed;
            }
            throw new Exception("Coravel.ConfigureEvents has been mis-configured.");
        }
    }
}
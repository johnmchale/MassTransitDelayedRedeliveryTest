using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace MassTransitDelayedRedeliveryTest
{
    public static class ServiceRegistrations
    {
        public static IServiceCollection AddMessageBusServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind MassTransit settings from appsettings.json
            var massTransitOptions = new MassTransitOptions();
            configuration.GetSection("MassTransit").Bind(massTransitOptions);

            services.AddMassTransit(x =>
            {
                var entryAssembly = Assembly.GetExecutingAssembly();
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumers(entryAssembly);

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    // Match the main application's queue setup
                    cfg.ConfigureEndpoints(context);

                    // Use delayed redelivery with intervals from appsettings.json
                    cfg.UseDelayedRedelivery(r =>
                    {
                        r.Intervals(
                            TimeSpan.FromMinutes(massTransitOptions.DelayedRetryPeriod1),
                            TimeSpan.FromMinutes(massTransitOptions.DelayedRetryPeriod2),
                            TimeSpan.FromMinutes(massTransitOptions.DelayedRetryPeriod3)
                        );
                    });

                    //// Use message retry to match the main application
                    //cfg.UseMessageRetry(r => r.Incremental(
                    //    massTransitOptions.IntervalLimit,
                    //    TimeSpan.FromSeconds(massTransitOptions.InitialInterval),
                    //    TimeSpan.FromSeconds(massTransitOptions.IntervalIncrement)
                    //));
                });
            });

            return services;
        }
    }
}

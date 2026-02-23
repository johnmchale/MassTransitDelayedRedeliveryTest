using MassTransit;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("MassTransit", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "MassTransitDelayedRedeliveryTest")
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341")
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((context, services) =>
    {

        services.AddMassTransit(x =>
        {
            // 1) Register all consumers in the assembly (no 40+ consumer lines)
            x.AddConsumers(typeof(TestMessageConsumer).Assembly);

            // 2) Apply to EVERY endpoint created by cfg.ConfigureEndpoints(context)
            x.AddConfigureEndpointsCallback((context, name, endpointCfg) =>
            {

                // PER-ENDPOINT cap (prevents a single hot queue hogging everything)
                endpointCfg.ConcurrentMessageLimit = 1; // start with 1 (or 2)
                endpointCfg.PrefetchCount = 4;

                // IMPORTANT: delayed redelivery FIRST
                endpointCfg.UseDelayedRedelivery(r =>
                {
                    r.Ignore<HttpRequestException>(ex =>
                        HttpRetryHelper.TryGetStatus(ex, out var status) &&
                        status >= 400 && status < 500 &&
                        status != 429);


                    r.Intervals(
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(20),
                        TimeSpan.FromSeconds(30));
                });

                // THEN immediate retry
                endpointCfg.UseMessageRetry(r =>
                {
                    r.Ignore<HttpRequestException>(ex =>
                       HttpRetryHelper.TryGetStatus(ex, out var status) &&
                       status >= 400 && status < 500);

                    r.Interval(3, TimeSpan.FromSeconds(2));
                });
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // GLOBAL cap (across ALL endpoints in this container)
                cfg.UseConcurrencyLimit(10);


                // 3) This is what creates the endpoints and triggers the callback above
                cfg.ConfigureEndpoints(context);
            });
        });


        services.AddHostedService<TestMessagePublisherService>();

    })
    .Build();

await host.RunAsync();

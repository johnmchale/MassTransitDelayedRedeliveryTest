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
        // Configure MassTransit with RabbitMQ
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TestMessageConsumer>(); 

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq", h =>
                {
                    h.Username("guest");  // Use guest/guest
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("worker-queue", e =>
                {
                    // n.b ensuring that both UseMessageRetry and UseDelayedRedelivery both work 
                    // comment out one of the two to see the other in action

                    // UseMessageRetry is MassTransits built in retry policy which retries the message immediately after a failure, with an optional delay between retries.
                    // In this case we are configuring it to retry 3 times with a 10 second delay between each retry.
                    // If the message still fails after the configured number of retries, it will be moved to the _skipped queue.
                    // It uses Polly under the hood to handle the retry logic, which allows for more advanced retry policies such as
                    // exponential backoff, circuit breakers, etc. Polly also provides detailed logging and metrics for retries, which can be useful for monitoring and troubleshooting.
                    
                    //e.UseMessageRetry(r =>
                    //{
                    //    r.Interval(3, TimeSpan.FromSeconds(10)); // Retry 3 times with 10s delay
                    //});

                    e.UseDelayedRedelivery(r =>
                    {
                        r.Intervals(TimeSpan.FromSeconds(10),
                                    TimeSpan.FromSeconds(20),
                                    TimeSpan.FromSeconds(30));
                    });


                    e.ConfigureConsumer<TestMessageConsumer>(context);
                });
            });
        });

        services.AddHostedService<TestMessagePublisherService>();
    })
    .Build();

await host.RunAsync();

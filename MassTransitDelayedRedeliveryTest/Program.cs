using MassTransit;
using MassTransitDelayedRedeliveryTest; 
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341")
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((context, services) =>
    {

        services.AddMessageBusServices(context.Configuration); // 

        // Configure MassTransit with RabbitMQ
        //services.AddMassTransit(x =>
        //{
        //    x.UsingRabbitMq((context, cfg) =>
        //    {
        //        cfg.Host("rabbitmq", h =>
        //        {
        //            h.Username("guest");  // Use guest/guest
        //            h.Password("guest");
        //        });

        //        // -------------------------------------------------------------------------
        //        // Block 1 
        //        // code added just to make sure the message goes to the queue but is not consumed
        //        // this is useful to see the headers and payload content as used by MassTransit 
        //        // n.b. you'll have to uncomment this block and comment out Block 2 to see the message sitting in the queue
        //        // in the RabbitMQ Management Plugin UI 

        //        //cfg.ReceiveEndpoint("worker-queue", e =>
        //        //{
        //        //    e.ConfigureConsumeTopology = false; //Prevents auto-consuming
        //        //    e.SetQueueArgument("x-queue-mode", "lazy");
        //        //    e.SetQueueArgument("durable", true);

        //        //    // Manually bind the queue to the correct exchange to ensure messages arrive
        //        //    e.Bind<TestMessage>();
        //        //});

        //        //-------------------------------------------------------------------------
        //        // Block 2 
        //        // code added to consume the message from the queue
        //        // Block 1 should be commented out for this block to work 
        //        cfg.ReceiveEndpoint("worker-queue", e =>
        //        {
        //            // n.b ensuring that both UseMessageRetry and UseDelayedRedelivery both work 
        //            // comment out one of the two to see the other in action

        //            //e.UseMessageRetry(r =>
        //            //{
        //            //    r.Interval(3, TimeSpan.FromSeconds(10)); // Retry 3 times with 10s delay
        //            //});

        //            e.UseDelayedRedelivery(r =>
        //            {
        //                r.Intervals(TimeSpan.FromSeconds(10),
        //                            TimeSpan.FromSeconds(20),
        //                            TimeSpan.FromSeconds(30));
        //            });


        //            e.Consumer<TestMessageConsumer>();
        //        });
        //    });
        //});

        services.AddHostedService<TestMessagePublisherService>();
    })
    .Build();

await host.RunAsync();

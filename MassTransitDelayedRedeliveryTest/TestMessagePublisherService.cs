using MassTransit;
using MassTransitDelayedRedeliveryTest;

public class TestMessagePublisherService : IHostedService
{
    private readonly IBus _bus;
    private readonly ILogger<TestMessagePublisherService> _logger;

    public TestMessagePublisherService(IBus bus, ILogger<TestMessagePublisherService> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing TestMessage to RabbitMQ...");

        await _bus.Publish(new TestMessage { Text = "Hello from MassTransit!" }, cancellationToken);

        _logger.LogInformation("Message published.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

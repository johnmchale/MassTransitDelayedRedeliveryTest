using MassTransit;

namespace MassTransitDelayedRedeliveryTest
{
    public class TestMessagePublisherService : IHostedService
    {
        private readonly IBus _bus;

        public TestMessagePublisherService(IBus bus)
        {
            _bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Publishing message to RabbitMQ...");

            await _bus.Publish(new TestMessage { Text = "Hello from MassTransit!" }, cancellationToken);

            Console.WriteLine("Message published! Exiting...");

        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}


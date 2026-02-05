using MassTransit;
using MassTransitDelayedRedeliveryTest;
using Polly;

public class TestMessageConsumer : IConsumer<TestMessage>
{
    private readonly ILogger<TestMessageConsumer> _logger;

    public TestMessageConsumer(ILogger<TestMessageConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TestMessage> context)
    {
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(500),
                (exception, delay, retryCount, _) =>
                {
                    _logger.LogWarning(exception,
                        "Polly retry {RetryCount}: retrying in {DelayMs}ms",
                        retryCount, delay.TotalMilliseconds);
                });

        await policy.ExecuteAsync(async () =>
        {
            var dbActive = Environment.GetEnvironmentVariable("DB_ACTIVE") ?? "false";
            var isDbActive = bool.TryParse(dbActive, out var result) && result;

            if (!isDbActive)
            {
                _logger.LogWarning(
                    "DB is inactive. MessageId={MessageId} Text={Text}",
                    context.MessageId, context.Message.Text);

                throw new Exception("DB inactive - triggering retry");
            }

            _logger.LogInformation(
                "Message processed. MessageId={MessageId} Text={Text}",
                context.MessageId, context.Message.Text);
        });
    }
}

using MassTransit;
using Polly;
using Polly.Retry;
using System;
using System.Threading.Tasks;
using Serilog;
using MassTransitDelayedRedeliveryTest;

public class TestMessageConsumer : IConsumer<TestMessage>
{
    public async Task Consume(ConsumeContext<TestMessage> context)
    {
        var policy = Policy
            .Handle<Exception>()  // Handle any exception
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(500), // Polly retries 3 times, 500ms apart
                (exception, timeSpan, retryCount, _) =>
                {
                    Log.Warning($"Polly retry {retryCount}: Retrying in {timeSpan.TotalMilliseconds}ms due to {exception.Message}");
                });

        await policy.ExecuteAsync(async () =>
        {
            var dbActive = Environment.GetEnvironmentVariable("DB_ACTIVE") ?? "false";
            bool isDbActive = bool.TryParse(dbActive, out var result) && result;

            if (!isDbActive)
            {
                Log.Warning("DB is inactive. Message will be retried.");
                throw new Exception("DB inactive - triggering retry");
            }

            Log.Information($"Message processed: {context.Message.Text}");
            Console.WriteLine($"Processed: {context.Message.Text}");

            await Task.CompletedTask;
        });
    }
}

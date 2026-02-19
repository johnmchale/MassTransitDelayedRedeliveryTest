using MassTransit;
using MassTransitDelayedRedeliveryTest;

public class TestMessageConsumer : IConsumer<TestMessage>
{
    private readonly ILogger<TestMessageConsumer> _logger;

    public TestMessageConsumer(ILogger<TestMessageConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TestMessage> context)
    {
        var redeliveryCount = context.GetRedeliveryCount();
        var retryAttempt = context.GetRetryAttempt();

        _logger.LogWarning(
            "ENTER Consume: RetryAttempt={RetryAttempt} RedeliveryCount={RedeliveryCount}",
            retryAttempt, redeliveryCount);

        // ============================================================
        // SIMULATION BLOCK – Uncomment ONE section at a time
        // ============================================================

        #region DB_SIMULATION

        //var dbActiveRaw = Environment.GetEnvironmentVariable("DB_ACTIVE") ?? "true";

        //if (!bool.TryParse(dbActiveRaw, out var isDbActive) || !isDbActive)
        //{
        //    _logger.LogWarning("SIMULATION: DB is inactive");
        //    throw new Exception("Simulated DB failure");
        //}

        #endregion


        #region HTTP_STATUS_SIMULATION

        var statusRaw = Environment.GetEnvironmentVariable("SIMULATED_HTTP_STATUS");

        if (!string.IsNullOrWhiteSpace(statusRaw) &&
            int.TryParse(statusRaw, out var status))
        {
            if (status == 200)
            {
                _logger.LogInformation("SIMULATION: HTTP 200 - success");
            }
            else
            {
                var ex = new HttpRequestException($"Simulated HTTP {status}");
                ex.Data["StatusCode"] = status;

                _logger.LogWarning("SIMULATION: Throwing HTTP {StatusCode}", status);
                throw ex;
            }
        }

        #endregion

        // ============================================================
        // NORMAL SUCCESS PATH
        // ============================================================

        _logger.LogInformation(
            "Message processed successfully. MessageId={MessageId} Text={Text}",
            context.MessageId,
            context.Message.Text);

        await Task.CompletedTask;
    }
}

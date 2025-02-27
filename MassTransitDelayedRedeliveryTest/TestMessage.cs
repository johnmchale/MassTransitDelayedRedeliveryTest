namespace MassTransitDelayedRedeliveryTest
{
    public record TestMessage
    {
        public string Text { get; init; } = string.Empty;

        // Ensure a parameterless constructor exists for deserialization
        public TestMessage() { }
    }
}


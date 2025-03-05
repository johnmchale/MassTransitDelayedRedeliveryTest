namespace MassTransitDelayedRedeliveryTest
{
    public class MassTransitOptions
    {
        public int IntervalLimit { get; set; }
        public int InitialInterval { get; set; }
        public int IntervalIncrement { get; set; }
        public int DelayedRetryPeriod1 { get; set; }
        public int DelayedRetryPeriod2 { get; set; }
        public int DelayedRetryPeriod3 { get; set; }
    }
}

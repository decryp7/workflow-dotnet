using System.Threading;

namespace SimpleWorkflow.Test
{
    public sealed class TestSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object state)
        {
            d(state);
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            d(state);
        }
    }
}
using SimpleWorkflow.Events;
using SimpleWorkflow.SanityCheck;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow engine context
    /// </summary>
    public class WorkflowEngineContext
    {
        /// <summary>
        /// Workflow engine event aggregator
        /// </summary>
        public IWorkflowEngineEventAggregator EventAggregator { get; private set; }

        /// <summary>
        /// Cancellation token
        /// </summary>
        /// readonly is used here so that CancellationToken can only be set by constructor
        /// and not anywhere else
        public CancellationToken CancellationToken { get; } = new CancellationToken();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventAggregator">Event aggregator</param>
        public WorkflowEngineContext(IWorkflowEngineEventAggregator eventAggregator)
        {
            Guard.Ensure(eventAggregator, nameof(eventAggregator)).IsNotNull();
            EventAggregator = eventAggregator;
        }
    }
}
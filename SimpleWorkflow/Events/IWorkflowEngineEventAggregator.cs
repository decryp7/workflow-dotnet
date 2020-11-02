namespace SimpleWorkflow.Events
{
    /// <summary>
    /// WorkflowEngine event aggregator interface
    /// </summary>
    public interface IWorkflowEngineEventAggregator
    {
        /// <summary>
        /// Get the workflow engine event
        /// </summary>
        /// <typeparam name="TMessage">Workflow engine event type</typeparam>
        /// <returns>Workflow engine event</returns>
        TMessage GetMessage<TMessage>() where TMessage : WorkflowEngineEventBase;
    }
}
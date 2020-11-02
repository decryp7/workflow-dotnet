namespace SimpleWorkflow
{
    /// <summary>
    /// Implement this interface for WorkflowContext to be injected.
    /// </summary>
    /// <typeparam name="TWorkflowContext"></typeparam>
    internal interface IRequireWorkflowContext<in TWorkflowContext>
    {
        /// <summary>
        /// The workflow context that is to be injected.
        /// </summary>
        TWorkflowContext WorkflowContext { set; }
    }
}
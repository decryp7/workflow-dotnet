namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow execution result
    /// </summary>
    public class WorkflowExecutionResult
    {
        /// <summary>
        /// Get the kind of workflow execution result
        /// </summary>
        public WorkflowExecutionResultKind ResultKind { get; private set; }

        /// <summary>
        /// Set workflow execution result to failed
        /// </summary>
        /// <returns>Workflow execution result</returns>
        public WorkflowExecutionResult SetFailed()
        {
            ResultKind = WorkflowExecutionResultKind.Failed;
            return this;
        }

        /// <summary>
        /// Set workflow execution result to successful
        /// </summary>
        /// <returns>Workflow execution result</returns>
        public WorkflowExecutionResult SetSuccessful()
        {
            ResultKind = WorkflowExecutionResultKind.Successful;
            return this;
        }

        /// <summary>
        /// Set workflow execution result to canceled
        /// </summary>
        /// <returns>Workflow execution result</returns>
        public WorkflowExecutionResult SetCanceled()
        {
            ResultKind = WorkflowExecutionResultKind.Canceled;
            return this;
        }
    }
}
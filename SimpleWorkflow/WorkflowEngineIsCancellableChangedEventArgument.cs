using System;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow engine is cancellable event args
    /// </summary>
    public class WorkflowEngineIsCancellableChangedEventArgument : EventArgs
    {
        /// <summary>
        /// Get the new value of workflow engine's IsCancellable property
        /// </summary>
        public bool IsCancellable { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isCancellable">IsCancellable value</param>
        public WorkflowEngineIsCancellableChangedEventArgument(bool isCancellable)
        {
            IsCancellable = isCancellable;
        }
    }
}
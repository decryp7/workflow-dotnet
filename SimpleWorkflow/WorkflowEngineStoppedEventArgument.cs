using System;
using GuardLibrary;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow engine stopped event args
    /// </summary>
    public class WorkflowEngineStoppedEventArgument : EventArgs
    {
        /// <summary>
        /// Get the workflow engine execution result
        /// </summary>
        public WorkflowEngineExecutionResult Result { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result">Workflow engine execution result</param>
        public WorkflowEngineStoppedEventArgument(WorkflowEngineExecutionResult result)
        {
            Guard.Ensure(result, nameof(result)).IsNotNull();
            Result = result;
        }
    }
}
using System;
using GuardLibrary;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow engine started event args
    /// </summary>
    public class WorkflowEngineStartedEventArgument : EventArgs
    {
        /// <summary>
        /// Get the total number of workflows queue for execution
        /// </summary>
        public int TotalNumberOfWorkflows { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="totalNumberOfWorkflows">Total number of workflows queue for execution</param>
        public WorkflowEngineStartedEventArgument(int totalNumberOfWorkflows)
        {
            Guard.Ensure(totalNumberOfWorkflows, nameof(totalNumberOfWorkflows)).IsMoreThenZero();
            
            TotalNumberOfWorkflows = totalNumberOfWorkflows;
        }
    }
}
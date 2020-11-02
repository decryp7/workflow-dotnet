using System;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow engine progress changed event args
    /// </summary>
    public class WorkflowEngineProgressChangedEventArgument : EventArgs
    {
        /// <summary>
        /// Get the current workflow engine progress percentage
        /// </summary>
        public double CompletionPercentage { get;}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="completionPercentage">Completion percentage</param>
        public WorkflowEngineProgressChangedEventArgument(double completionPercentage)
        {
            CompletionPercentage = completionPercentage;
        }
    }
}
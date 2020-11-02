using System;
using SimpleWorkflow.SanityCheck;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow engine error occured event args
    /// </summary>
    public class WorkflowEngineErrorOccurredEventArgument : EventArgs
    {
        /// <summary>
        /// Error source
        /// </summary>
        public object Source { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Error source</param>
        /// <param name="errorDescription">Error message</param>
        public WorkflowEngineErrorOccurredEventArgument(object source, string errorDescription)
        {
            Guard.Ensure(errorDescription, nameof(errorDescription)).IsNotNullOrEmpty();
            Error = errorDescription;
            //source can be null
            Source = source;
        }
    }
}
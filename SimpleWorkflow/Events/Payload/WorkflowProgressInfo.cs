using System;

namespace SimpleWorkflow.Events.Payload
{
    /// <summary>
    /// Workflow progress payload
    /// </summary>
    public class WorkflowProgressInfo : WorkflowInfo
    {
        /// <summary>
        /// Get the completion percentage
        /// </summary>
        public double CompletionPercentage { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workflowType">Workflow Type</param>
        /// <param name="id">Workflow id</param>
        /// <param name="name">Workflow name</param>
        /// <param name="workflowContext">Workflow context</param>
        /// <param name="completionPercentage">Completion percentage</param>
        public WorkflowProgressInfo(Type workflowType, Guid id, string name, object workflowContext,
            double completionPercentage)
            : base(workflowType, id, name, workflowContext)
        {
            CompletionPercentage = completionPercentage;
        }
    }
}
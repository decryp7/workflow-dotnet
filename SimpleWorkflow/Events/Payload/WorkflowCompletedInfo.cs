using System;
using GuardLibrary;

namespace SimpleWorkflow.Events.Payload
{
    /// <summary>
    /// Workflow completed payload
    /// </summary>
    public class WorkflowCompletedInfo : WorkflowInfo
    {
        /// <summary>
        /// Get the workflow execution result
        /// </summary>
        public WorkflowExecutionResult Result { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workflowType">Workflow Type</param>
        /// <param name="id">Workflow id</param>
        /// <param name="name">Workflow name</param>
        /// <param name="workflowContext">Workflow context</param>
        /// <param name="result">Workflow execution result</param>
        public WorkflowCompletedInfo(Type workflowType, Guid id, string name, object workflowContext,
            WorkflowExecutionResult result)
            : base(workflowType, id, name, workflowContext)
        {
            Guard.Ensure(result, nameof(result)).IsNotNull();
            Result = result;
        }
    }
}
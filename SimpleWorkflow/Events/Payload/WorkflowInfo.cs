using System;
using SimpleWorkflow.SanityCheck;

namespace SimpleWorkflow.Events.Payload
{
    /// <summary>
    /// Workflow payload
    /// </summary>
    public class WorkflowInfo
    {
        /// <summary>
        /// Get the workflow id
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Get the workflow name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Get the workflow context
        /// </summary>
        public object WorkflowContext { get; private set; }

        /// <summary>
        /// Get the workflow type
        /// </summary>
        public Type WorkflowType { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workflowType">Workflow Type</param>
        /// <param name="id">Workflow id</param>
        /// <param name="name">Workflow name</param>
        /// <param name="workflowContext">WorkflowContext</param>
        public WorkflowInfo(Type workflowType, Guid id, string name, object workflowContext)
        {
            Guard.Ensure(workflowType, nameof(workflowType)).IsNotNull();
            Guard.Ensure(name, nameof(name)).IsNotNullOrEmpty();
            Guard.Ensure(workflowContext, nameof(workflowContext)).IsNotNull();

            WorkflowType = workflowType;
            Id = id;
            Name = name;
            WorkflowContext = workflowContext;
        }
    }
}
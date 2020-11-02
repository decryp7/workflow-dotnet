using System;
using System.Globalization;
using SimpleWorkflow.SanityCheck;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow rollback started event args
    /// </summary>
    public class WorkflowRollbackStartedEventArgument : EventArgs
    {
        private object workflowContext;

        /// <summary>
        /// Get the workflow id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Workflow name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Get the workflow type
        /// </summary>
        public Type WorkflowType { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workflowType">Workflow type</param>
        /// <param name="id">Workflow id</param>
        /// <param name="name">Workflow name</param>
        /// <param name="workflowContext">Workflow context</param>
        public WorkflowRollbackStartedEventArgument(Type workflowType, Guid id, string name, object workflowContext)
        {
            Guard.Ensure(workflowType, nameof(workflowType)).IsNotNull();
            Guard.Ensure(id, nameof(id)).IsNotNull();
            Guard.Ensure(name, nameof(name)).IsNotNull();
            Guard.Ensure(workflowContext, nameof(workflowContext)).IsNotNull();

            Id = id;
            WorkflowType = workflowType;
            Name = name;
            this.workflowContext = workflowContext;
        }

        /// <summary>
        /// Get the workflow context
        /// </summary>
        /// <typeparam name="TWorkflowContext">WorkflowContext type</typeparam>
        /// <returns>WorkflowContext</returns>
        public TWorkflowContext GetWorkflowContext<TWorkflowContext>()
        {
            Guard.EnsureThisConditionIsMet(() => workflowContext is TWorkflowContext)
                .OrThrowException(
                    new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                        "WorkflowContext is not of type {0}.", typeof(TWorkflowContext))));

            return (TWorkflowContext)workflowContext;
        }
    }
}
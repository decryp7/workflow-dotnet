using System;
using System.Globalization;
using GuardLibrary;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow started event args
    /// </summary>
    public class WorkflowStartedEventArgument : EventArgs
    {
        private object workflowContext;

        /// <summary>
        /// Get the workflow name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Get the position of the current executing workflow
        /// </summary>
        public int Index { get; }
        
        /// <summary>
        /// Get the total number of workflow queued for execution
        /// </summary>
        public int TotalWorkflowsCount { get; }

        /// <summary>
        /// Get the workflow id
        /// </summary>
        public Guid Id { get; }

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
        /// <param name="workflowContext">The workflow context</param>
        /// <param name="index">Position of the current executing workflow</param>
        /// <param name="totalWorkflowsCount">Total number for workflow queued for execution</param>
        public WorkflowStartedEventArgument(Type workflowType, Guid id, string name, object workflowContext, int index, int totalWorkflowsCount)
        {
            Guard.Ensure(workflowType, nameof(workflowType)).IsNotNull();
            Guard.Ensure(id, nameof(id)).IsNotNull();
            Guard.Ensure(name, nameof(name)).IsNotNullOrEmpty();
            Guard.Ensure(workflowContext, nameof(workflowContext)).IsNotNull();
            Guard.Ensure(index, nameof(index)).IsNotNull().IsMoreThenZero();
            Guard.Ensure(totalWorkflowsCount, nameof(totalWorkflowsCount)).IsNotNull().IsMoreThenZero();

            WorkflowType = workflowType;
            Name = name;
            Id = id;
            Index = index;
            TotalWorkflowsCount = totalWorkflowsCount;
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
                        "WorkflowContext is not of type {0}.", typeof (TWorkflowContext))));

            return (TWorkflowContext)workflowContext;
        }
    }
}
using System;
using System.Globalization;
using SimpleWorkflow.SanityCheck;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow completed event args
    /// </summary>
    public class WorkflowCompletedEventArgument : EventArgs
    {
        private object workflowContext;

        /// <summary>
        /// Workflow name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Workflow execution result
        /// </summary>
        public WorkflowExecutionResult Result { get; private set; }

        /// <summary>
        /// Get the workflow id
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Get the workflow type
        /// </summary>
        public Type WorkflowType { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workflowType">Workflow type</param>
        /// <param name="id">Workflow id</param>
        /// <param name="name">Workflow name</param>
        /// <param name="workflowContext">The workflow context</param>
        /// <param name="result">Workflow execution result</param>
        public WorkflowCompletedEventArgument(Type workflowType, Guid id, string name, object workflowContext,
            WorkflowExecutionResult result)
        {
            Guard.Ensure(workflowType, nameof(workflowType)).IsNotNull();
            Guard.Ensure(id, nameof(id)).IsNotNull();
            Guard.Ensure(name, nameof(name)).IsNotNull();
            Guard.Ensure(workflowContext, nameof(workflowContext)).IsNotNull();
            Guard.Ensure(result, nameof(result)).IsNotNull();

            WorkflowType = workflowType;
            Name = name;
            Id = id;
            Result = result;
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

            return (TWorkflowContext) workflowContext;
        }
    }
}
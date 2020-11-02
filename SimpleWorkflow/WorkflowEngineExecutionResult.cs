using System;
using System.Collections.Generic;
using System.Linq;
using SimpleWorkflow.SanityCheck;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow engine execution result
    /// </summary>
    public class WorkflowEngineExecutionResult
    {
        private readonly IList<IWorkflow> workflows; 

        /// <summary>
        /// Get the kind of workflow engine execution result
        /// </summary>
        public WorkflowEngineExecutionResultKind ResultKind { get; private set; }

        public WorkflowEngineExecutionResult(IList<IWorkflow> workflows)
        {
            Guard.Ensure(workflows, nameof(workflows)).IsNotNull();
            this.workflows = workflows;
        }

        /// <summary>
        /// Set workflow engine execution result to completed
        /// </summary>
        /// <returns>Workflow engine execution result</returns>
        public WorkflowEngineExecutionResult SetCompleted()
        {
            ResultKind = WorkflowEngineExecutionResultKind.Completed;
            return this;
        }

        /// <summary>
        /// Set workflow engine execution result to completed with errors
        /// </summary>
        /// <returns>Workflow engine execution result</returns>
        public WorkflowEngineExecutionResult SetCompletedWithErrors()
        {
            ResultKind = WorkflowEngineExecutionResultKind.CompletedWithErrors;
            return this;
        }

        /// <summary>
        /// Set workflow engine execution result to canceled
        /// </summary>
        /// <returns>Workflow engine execution result</returns>
        public WorkflowEngineExecutionResult SetCanceled()
        {
            ResultKind = WorkflowEngineExecutionResultKind.Canceled;
            return this;
        }

        /// <summary>
        /// Get the count of workflows that matches the type
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int Count(Func<IWorkflow, bool> query)
        {
            Guard.Ensure(query, nameof(query)).IsNotNull();
            return
                workflows
                    .Count(
                        query.Invoke);
        }
    }
}
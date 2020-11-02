using System;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow interface
    /// </summary>
    public interface IWorkflow
    {
        /// <summary>
        /// Get the workflow execution result
        /// </summary>
        WorkflowExecutionResult ExecutionResult { get; }

        /// <summary>
        /// Get the workflow state
        /// </summary>
        WorkflowState State { get; }

        /// <summary>
        /// Unique identifier for the workflow
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Name of the workflow
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Execute the workflow
        /// </summary>
        /// <returns>Workflow execution result</returns>
        WorkflowExecutionResult Execute();

        /// <summary>
        /// Reset the workflow
        /// </summary>
        void Reset();

        /// <summary>
        /// Queue a activity on a workflow
        /// </summary>
        /// <param name="activity">Activity</param>
        /// <returns>Workflow</returns>
        IWorkflow Do(IActivity activity);

        /// <summary>
        /// Set the completion percentage for the workflow after this particular activity has finished execution successfully.
        /// </summary>
        /// <example>workflow.Queue(activity).SetCompletionActivity(10);</example>
        /// <param name="percentage">Target percentage</param>
        /// <returns>Workflow</returns>
        IWorkflow SetCompletionPercentage(double percentage);

        /// <summary>
        /// Continue workflow execution even though the particular activity has failed.
        /// This basically tell the workflow to ignore the failure of the activity.
        /// If this is not used, workflow automatically fails and rollbacks.
        /// </summary>
        /// <example>workflow.Queue(activity).IgnoreExecutionFailure();</example>
        /// <returns>Workflow</returns>
        IWorkflow IgnoreExecutionFailure();

        /// <summary>
        /// Set the activity to be executed if the workflow fails.
        /// </summary>
        /// <example>workflow.Queue(activity).SetRollbackActivity(rollbackActivity);</example>
        /// <param name="rollbackActivity">Rollback activity</param>
        /// <returns>Workflow</returns>
        IWorkflow SetRollbackActivity(IActivity rollbackActivity);
    }
}
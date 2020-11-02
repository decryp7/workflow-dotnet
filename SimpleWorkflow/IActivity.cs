using System;

namespace SimpleWorkflow
{
    /// <summary>
    /// Activity interface
    /// </summary>
    public interface IActivity
    {
        /// <summary>
        /// Get the activity execution result
        /// </summary>
        ActivityExecutionResult ExecutionResult { get; }

        /// <summary>
        /// Get the activity state
        /// </summary>
        ActivityState State { get; }

        /// <summary>
        /// Unique Id of the activity
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Description of the activity's purpose
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Return true if activity is cancellable, otherwise false
        /// </summary>
        bool IsCancellable { get; }

        /// <summary>
        /// Execute the activity
        /// </summary>
        /// <returns>The execution result</returns>
        ActivityExecutionResult Execute();

        /// <summary>
        /// Reset the activity state
        /// </summary>
        void Reset();
    }
}
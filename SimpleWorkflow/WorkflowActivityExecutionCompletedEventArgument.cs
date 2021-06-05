using System;
using GuardLibrary;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow activity execution completed event args
    /// </summary>
    public class WorkflowActivityExecutionCompletedEventArgument : EventArgs
    {
        /// <summary>
        /// Activity id
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Activity's description
        /// </summary>
        public string Description { get; private set; }
        
        /// <summary>
        /// Activity execution result
        /// </summary>
        public ActivityExecutionResult Result { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Activity id</param>
        /// <param name="description">Activity description</param>
        /// <param name="result">Activity execution result</param>
        public WorkflowActivityExecutionCompletedEventArgument(Guid id, string description, ActivityExecutionResult result)
        {
            Guard.Ensure(id, nameof(id)).IsNotEmpty();
            Guard.Ensure(description, nameof(description)).IsNotNullOrEmpty();
            Guard.Ensure(result, nameof(result)).IsNotNull();
            Id = id;
            Description = description;
            Result = result;
        }
    }
}
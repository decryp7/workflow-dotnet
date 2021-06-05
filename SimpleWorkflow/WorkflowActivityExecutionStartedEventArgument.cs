using System;
using GuardLibrary;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow activity execution started event args
    /// </summary>
    public class WorkflowActivityExecutionStartedEventArgument : EventArgs
    {
        /// <summary>
        /// Activity id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Activity description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Checks if the activity is cancellable
        /// </summary>
        public bool IsCancellable { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Activity id</param>
        /// <param name="description">Activity description</param>
        /// <param name="isCancellable">IsCancellable</param>
        public WorkflowActivityExecutionStartedEventArgument(Guid id, string description, bool isCancellable)
        {
            Guard.Ensure(id, nameof(id)).IsNotEmpty();
            Guard.Ensure(description, nameof(description)).IsNotNull();
            Id = id;
            Description = description;
            IsCancellable = isCancellable;
        }
    }
}
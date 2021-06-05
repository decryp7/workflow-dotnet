using System;
using GuardLibrary;

namespace SimpleWorkflow.Events.Payload
{
    /// <summary>
    /// Activity execution completed info payload
    /// </summary>
    public class ActivityExecutionCompletedInfo : ActivityInfo
    {
        /// <summary>
        /// Get the activity execution result
        /// </summary>
        public ActivityExecutionResult Result { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Activity id</param>
        /// <param name="description">Activity description</param>
        /// <param name="result">Activity execution result</param>
        public ActivityExecutionCompletedInfo(Guid id, string description, ActivityExecutionResult result)
            : base(id, description)
        {
            Guard.Ensure(result, nameof(result)).IsNotNull();
            Result = result;
        }
    }
}
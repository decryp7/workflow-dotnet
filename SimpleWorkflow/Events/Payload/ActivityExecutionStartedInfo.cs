using System;

namespace SimpleWorkflow.Events.Payload
{
    /// <summary>
    /// Activity execution started payload
    /// </summary>
    public class ActivityExecutionStartedInfo : ActivityInfo
    {
        /// <summary>
        /// Checks if the activity is cancellable
        /// </summary>
        public bool IsCancellable { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description">Activity description</param>
        /// <param name="isCancellable">IsCancellable</param>
        public ActivityExecutionStartedInfo(Guid id, string description, bool isCancellable)
            : base(id, description)
        {
            IsCancellable = isCancellable;
        }
    }
}
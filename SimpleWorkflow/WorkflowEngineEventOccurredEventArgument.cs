using System;
using GuardLibrary;

namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow engine event occured event args
    /// </summary>
    public class WorkflowEngineEventOccurredEventArgument : EventArgs
    {
        /// <summary>
        /// Event Source
        /// </summary>
        public object Source { get; private set; }

        /// <summary>
        /// Event message
        /// </summary>
        public string Event { get; private set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="eventDescription">Event message</param>
        /// <param name="messageDateTimeFormatType">Message date time format type.</param>
        public WorkflowEngineEventOccurredEventArgument(
            object source,
            string eventDescription)
        {
            Guard.Ensure(eventDescription, nameof(eventDescription)).IsNotNullOrEmpty();
            Event = eventDescription;
            //source can be null
            Source = source;
        }
    }
}
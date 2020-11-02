using SimpleWorkflow.SanityCheck;

namespace SimpleWorkflow.Events.Payload
{
    /// <summary>
    /// Event Information
    /// </summary>
    public class EventMessage : Message
    {
        /// <summary>
        /// Event message
        /// </summary>
        public string Event { get; private set; }


        /// <summary>
        /// Event Information
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="event">Event message</param>
        /// <param name="messageDateTimeFormatType">Message date time format type.</param>
        public EventMessage(
            object source,
            string @event)
            : base(source)
        {
            Guard.Ensure(@event, nameof(@event)).IsNotNullOrEmpty();
            Event = @event;
        }
    }
}
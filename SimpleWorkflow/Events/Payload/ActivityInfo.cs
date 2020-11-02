using System;
using SimpleWorkflow.SanityCheck;

namespace SimpleWorkflow.Events.Payload
{
    /// <summary>
    /// Activity payload
    /// </summary>
    public class ActivityInfo
    {
        /// <summary>
        /// Get the activity id
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Get the activity description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Activity id</param>
        /// <param name="description">Activity description</param>
        public ActivityInfo(Guid id, string description)
        {
            Guard.Ensure(id, nameof(id)).IsNotEmpty();
            Guard.Ensure(description, nameof(description)).IsNotNull();

            Id = id;
            Description = description;
        }
    }
}
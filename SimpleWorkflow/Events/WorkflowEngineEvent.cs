using System;
using GuardLibrary;

namespace SimpleWorkflow.Events
{
    /// <summary>
    /// Workflow engine event
    /// </summary>
    /// <typeparam name="TPayload">Payload type</typeparam>
    public class WorkflowEngineEvent<TPayload> : WorkflowEngineEventBase
    {
        /// <summary>
        /// Subscribe to the workflow engine event
        /// </summary>
        /// <param name="action">Event Handler</param>
        public void Subscribe(Action<TPayload> action)
        {
            Guard.Ensure(action, nameof(action)).IsNotNull();
            InternalSubscribe(action);
        }

        /// <summary>
        /// Publish the workflow engine event
        /// </summary>
        /// <param name="payload">Payload</param>
        public void Publish(TPayload payload)
        {
            Guard.Ensure(payload, nameof(payload)).IsNotNull();
            InternalPublish(payload);
        }

        /// <summary>
        /// Unsubscribe to the workflow engine event
        /// </summary>
        /// <param name="action">Event Handler</param>
        public void Unsubscribe(Action<TPayload> action)
        {
            Guard.Ensure(action, nameof(action)).IsNotNull();
            InternalUnsubscribe(action);
        }
    }
}
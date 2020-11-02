using System;
using System.Collections.Generic;
using System.Linq;
using SimpleWorkflow.SanityCheck;

namespace SimpleWorkflow.Events
{
    /// <summary>
    /// Workflow engine event base
    /// </summary>
    public abstract class WorkflowEngineEventBase
    {
        private readonly IList<Delegate> actions = new List<Delegate>();

        /// <summary>
        /// Subscribe to the workflow engine event
        /// </summary>
        /// <param name="action">Workflow engine event handler</param>
        protected void InternalSubscribe(Delegate action)
        {
            Guard.Ensure(action, nameof(action)).IsNotNull();
            actions.Add(action);
        }

        /// <summary>
        /// Unsubscribe to the workflow engine event
        /// </summary>
        /// <param name="action">Workflow engine event handler</param>
        protected void InternalUnsubscribe(Delegate action)
        {
            Guard.Ensure(action, nameof(action)).IsNotNull();
            if (actions.Contains(action))
            {
                actions.Remove(action);
            }
        }

        /// <summary>
        /// Publish the workflow engine event
        /// </summary>
        /// <typeparam name="TPayload">Payload type</typeparam>
        /// <param name="payload">Payload</param>
        protected void InternalPublish<TPayload>(TPayload payload)
        {
            Guard.Ensure(payload, nameof(payload)).IsNotNull();
            foreach (Action<TPayload> action in actions.OfType<Action<TPayload>>())
            {
                action(payload);
            }
        }
    }
}
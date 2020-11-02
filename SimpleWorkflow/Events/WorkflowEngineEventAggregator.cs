using System;
using System.Collections.Generic;

namespace SimpleWorkflow.Events
{
    /// <summary>
    /// Workflow engine event aggregator
    /// </summary>
    public class WorkflowEngineEventAggregator : IWorkflowEngineEventAggregator
    {
        private readonly IDictionary<Type, WorkflowEngineEventBase> messages = new Dictionary<Type, WorkflowEngineEventBase>();

        /// <summary>
        /// Get the message
        /// </summary>
        /// <typeparam name="TMessage">Message type</typeparam>
        /// <returns>Message object</returns>
        public TMessage GetMessage<TMessage>() where TMessage : WorkflowEngineEventBase
        {
            if (messages.TryGetValue(typeof(TMessage), out var eventBase))
            {
                return (TMessage)eventBase;
            }

            TMessage message = Activator.CreateInstance<TMessage>();
            messages[typeof(TMessage)] = message;

            return message;
        }
    }
}
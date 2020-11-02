using System;
using SimpleWorkflow.Events;
using SimpleWorkflow.Events.Payload;

namespace SimpleWorkflow.Test
{
/// <summary>
    /// Dummy activity for unit testing
    /// </summary>
    public class DummyActivity : Activity<DummyWorkflowContext>
    {
        private readonly Func<ActivityExecutionResult> executionFunc;
        private readonly string description;
        private readonly bool isCancellable;
        private readonly bool publishEvent;
        private readonly bool publishError;

        public DummyActivity(string description = "Dummy Activity", bool isCancellable = true,
            Func<ActivityExecutionResult> executionFunc = null, bool publishEvent = false, bool publishError = false)
        {
            this.description = description;
            this.isCancellable = isCancellable;
            this.executionFunc = executionFunc;
            this.publishError = publishError;
            this.publishEvent = publishEvent;
        }

        #region Overrides of Activity<object>

        /// <summary>
        /// Description of the activity's purpose
        /// </summary>
        public override string Description
        {
            get { return this.description; }
        }

        /// <summary>
        /// Return true if activity is cancellable, otherwise false
        /// </summary>
        public override bool IsCancellable
        {
            get { return this.isCancellable; }
        }

        /// <summary>
        /// Activity execution implementation
        /// </summary>
        /// <returns></returns>
        protected override ActivityExecutionResult ExecuteImpl()
        {
            if (publishError)
            {
                WorkflowEngineContext.EventAggregator.GetMessage<ErrorOccured>()
                    .Publish(new ErrorMessage(this, "Error Occured!"));
            }

            if (publishEvent)
            {
                WorkflowEngineContext.EventAggregator.GetMessage<EventOccurred>()
                    .Publish(new EventMessage(this, "Event Occured!"));
            }

            return executionFunc != null ? executionFunc.Invoke() : base.ExecuteImpl();
        }

        #endregion
    }
}
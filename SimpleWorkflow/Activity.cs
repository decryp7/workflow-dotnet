using System;
using System.Diagnostics;
using System.Globalization;
using GuardLibrary;
using SimpleWorkflow.Events;
using SimpleWorkflow.Events.Payload;

namespace SimpleWorkflow
{
    /// <summary>
    /// Activity base
    /// </summary>
    /// <typeparam name="TWorkflowContext">Workflow context type</typeparam>
    public abstract class Activity<TWorkflowContext> : IActivity, IRequireWorkflowContext<TWorkflowContext>,
        IRequireWorkflowEngineContext
    {
        private TWorkflowContext workflowContext;
        private WorkflowEngineContext workflowEngineContext;
        private ActivityExecutionResult activityExecutionResult;

        /// <summary>
        /// Get the activity execution result
        /// </summary>
        public ActivityExecutionResult ExecutionResult
        {
            get
            {
                Guard.EnsureThisConditionIsMet(() => State == ActivityState.Stopped)
                    .OrThrowException(new InvalidOperationException("There is no result because Activity is not executed!"));
                return activityExecutionResult;
            }
        }

        /// <summary>
        /// Get the activity state
        /// </summary>
        public ActivityState State { get; private set; }

        /// <summary>
        /// Unique Id of the activity
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Get the injected workflow context
        /// </summary>
        protected TWorkflowContext WorkflowContext => workflowContext;

        /// <summary>
        /// The workflow context that is to be injected.
        /// </summary>
        TWorkflowContext IRequireWorkflowContext<TWorkflowContext>.WorkflowContext
        {
            set
            {
                Guard.Ensure(value, "WorkflowContext").IsNotNull();
                workflowContext = value;
            }
        }

        /// <summary>
        /// Get the injected workflow engine context
        /// </summary>
        protected WorkflowEngineContext WorkflowEngineContext => workflowEngineContext;

        /// <summary>
        /// The workflow engine context that is be injected.
        /// </summary>
        WorkflowEngineContext IRequireWorkflowEngineContext.WorkflowEngineContext
        {
            set
            {
                Guard.Ensure(value, "WorkflowEngineContext").IsNotNull();
                workflowEngineContext = value;
            }
        }

        /// <summary>
        /// Return true if activity is cancellable, otherwise false
        /// </summary>
        public virtual bool IsCancellable => true;

        /// <summary>
        /// Description of the activity's purpose
        /// </summary>
        public virtual string Description => Id.ToString();

        protected Activity()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Execute the activity
        /// </summary>
        /// <returns>The execution result</returns>
        public ActivityExecutionResult Execute()
        {
            Guard.EnsureThisConditionIsMet(() => State != ActivityState.Running)
                .OrThrowException(new InvalidOperationException("Activity is already running!"));

            try
            {
                State = ActivityState.Running;
                WorkflowEngineContext.EventAggregator.GetMessage<ActivityExecutionStarted>()
                    .Publish(new ActivityExecutionStartedInfo(Id, Description, IsCancellable));

                WriteDebug("Activity is running...");

                activityExecutionResult = ExecuteImpl();

                WorkflowEngineContext.EventAggregator.GetMessage<ActivityExecutionCompleted>()
                    .Publish(new ActivityExecutionCompletedInfo(Id, Description, activityExecutionResult));

                WriteDebug("Activity has completed execution. Result: {0}", activityExecutionResult.ResultKind);
            }
            finally
            {
                State = ActivityState.Stopped;
            }

            return activityExecutionResult;
        }

        /// <summary>
        /// Activity execution implementation
        /// </summary>
        /// <returns></returns>
        protected virtual ActivityExecutionResult ExecuteImpl()
        {
            return new ActivityExecutionResult().SetSuccessful();
        }

        /// <summary>
        /// Reset the activity state
        /// </summary>
        public virtual void Reset()
        {
            State = ActivityState.NotStarted;
            activityExecutionResult = null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        protected void WriteDebug(string format, params object[] args)
        {
            Guard.Ensure(format, "format").IsNotNullOrEmpty();

            string message = args.Length > 0 ? string.Format(CultureInfo.CurrentCulture, format, args) : format;

            FormattableString.Invariant( $"[Activity({GetType().Name},{Description})] {message}").WriteDebug();
        }
    }
}
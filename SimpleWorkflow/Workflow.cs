using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using SimpleWorkflow.Events;
using SimpleWorkflow.Events.Payload;
using SimpleWorkflow.SanityCheck;

namespace SimpleWorkflow
{
public abstract class Workflow<TWorkflowContext> : IWorkflow, IRequireWorkflowEngineContext
        where TWorkflowContext : WorkflowContext
    {
        private readonly IList<IActivity> activities = new List<IActivity>();
        private readonly IDictionary<Guid, double> activityCompletionPercentages = new Dictionary<Guid, double>();
        private readonly IList<Guid> ignoreExecutionFailureActivities = new List<Guid>();
        private readonly IDictionary<Guid, IActivity> rollbackActivities = new Dictionary<Guid, IActivity>();
        private readonly TWorkflowContext workflowContext;
        private WorkflowEngineContext workflowEngineContext;
        private WorkflowExecutionResult workflowExecutionResult;

        /// <summary>
        /// Get the workflow context
        /// </summary>
        protected TWorkflowContext WorkflowContext => workflowContext;

        /// <summary>
        /// Get the workflow execution result
        /// </summary>
        public WorkflowExecutionResult ExecutionResult
        {
            get
            {
                Guard.EnsureThisConditionIsMet(() => State == WorkflowState.Stopped)
                    .OrThrowException(new InvalidOperationException("There is no result because Workflow was not executed!"));
                return workflowExecutionResult;
            }
        }

        /// <summary>
        /// Get the workflow state
        /// </summary>
        public WorkflowState State { get; private set; }

        /// <summary>
        /// Unique identifier for the workflow
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Name of the workflow
        /// </summary>
        public virtual string Name => Id.ToString();

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
                Guard.Ensure(value, nameof(WorkflowEngineContext)).IsNotNull();
                workflowEngineContext = value;
            }
        }

        protected Workflow(TWorkflowContext workflowContext)
        {
            Guard.Ensure(workflowContext, nameof(workflowContext)).IsNotNull();

            Id = Guid.NewGuid();
            this.workflowContext = workflowContext;
        }

        /// <summary>
        /// Reset the workflow
        /// </summary>
        public void Reset()
        {
            State = WorkflowState.NotStarted;
            workflowExecutionResult = null;
            WorkflowContext.Cache.Clear();

            foreach (IActivity activity in activities)
            {
                activity.Reset();
            }
        }

        /// <summary>
        /// Queue a activity on a workflow
        /// </summary>
        /// <param name="activity">Activity</param>
        /// <returns>Workflow</returns>
        public IWorkflow Do(IActivity activity)
        {
            Guard.Ensure(activity, nameof(activity)).IsNotNull();
            activities.Add(activity);

            return this;
        }

        /// <summary>
        /// Set the completion percentage for the workflow after this particular activity has finished execution successfully.
        /// </summary>
        /// <example>workflow.Queue(activity).SetCompletionActivity(10);</example>
        /// <param name="percentage">Target percentage</param>
        /// <returns>Workflow</returns>
        public IWorkflow SetCompletionPercentage(double percentage)
        {
            Guard.EnsureThisConditionIsMet(() => activities.Count != 0)
                .OrThrowException(
                    new InvalidOperationException("Please add an Activity before setting the progress percentage."));
            Guard.EnsureThisConditionIsMet(() => !activityCompletionPercentages.ContainsKey(activities.Last().Id))
                .OrThrowException(new InvalidOperationException("Activity progress percentage is already set"));

            activityCompletionPercentages.Add(activities.Last().Id, percentage);

            return this;
        }

        /// <summary>
        /// Continue workflow execution even though the particular activity has failed.
        /// This basically tell the workflow to ignore the failure of the activity.
        /// If this is not used, workflow automatically fails and rollbacks.
        /// </summary>
        /// <example>workflow.Queue(activity).IgnoreExecutionFailure();</example>
        /// <returns>Workflow</returns>
        public IWorkflow IgnoreExecutionFailure()
        {
            Guard.EnsureThisConditionIsMet(() => activities.Count != 0)
                .OrThrowException(
                    new InvalidOperationException("Please add an Activity before setting the progress percentage."));
            Guard.EnsureThisConditionIsMet(() => !ignoreExecutionFailureActivities.Contains(activities.Last().Id))
                .OrThrowException(new InvalidOperationException("Activity is already set to halt workflow if it fail."));

            ignoreExecutionFailureActivities.Add(activities.Last().Id);

            return this;
        }

        /// <summary>
        /// Set the activity to be executed if the workflow fails.
        /// </summary>
        /// <example>workflow.Queue(activity).SetRollbackActivity(rollbackActivity);</example>
        /// <param name="rollbackActivity">Rollback activity</param>
        /// <returns>Workflow</returns>
        public IWorkflow SetRollbackActivity(IActivity rollbackActivity)
        {
            Guard.Ensure(rollbackActivity, "rollbackActivity").IsNotNull();
            Guard.EnsureThisConditionIsMet(() => activities.Count != 0)
                .OrThrowException(
                    new InvalidOperationException("Please add an Activity before setting the rollback activity."));
            Guard.EnsureThisConditionIsMet(() => !ignoreExecutionFailureActivities.Contains(activities.Last().Id))
                .OrThrowException(new InvalidOperationException("Rollback activity is already set."));

            rollbackActivities.Add(activities.Last().Id, rollbackActivity);

            return this;
        }

        /// <summary>
        /// Execute the workflow
        /// </summary>
        /// <returns>Workflow execution result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public virtual WorkflowExecutionResult Execute()
        {
            Guard.EnsureThisConditionIsMet(() => State != WorkflowState.Running)
                .OrThrowException(new InvalidOperationException("The workflow is already running!"));

            State = WorkflowState.Running;
            WriteDebug("Workflow is running...");

            WorkflowEngineContext.EventAggregator.GetMessage<WorkflowStarted>().Publish(new WorkflowInfo(GetType(), Id, Name, WorkflowContext));

            workflowExecutionResult = new WorkflowExecutionResult();

            foreach (IActivity activity in activities)
            {

                IActivity lastExecutedActivity = activities.LastOrDefault(a => a.State == ActivityState.Stopped);
                if (lastExecutedActivity != null && lastExecutedActivity.IsCancellable &&
                    WorkflowEngineContext.CancellationToken.CancellationPending)
                {
                    workflowExecutionResult.SetCanceled();
                    break;
                }

                ActivityExecutionResult activityExecutionResult = new ActivityExecutionResult();

                try
                {
                    InjectWorkflowContext(activity);
                    InjectWorkflowEngineContext(activity);

                    activityExecutionResult = activity.Execute();

                    if (activityExecutionResult.ResultKind == ActivityExecutionResultKind.Canceled)
                    {
                        workflowExecutionResult.SetCanceled();
                        break;
                    }

                    if (activityExecutionResult.ResultKind == ActivityExecutionResultKind.Failed)
                    {
                        if (!ignoreExecutionFailureActivities.Contains(activity.Id))
                        {
                            workflowExecutionResult.SetFailed();
                            break;
                        }
                        WriteDebug("Ignoring failure for {0}...", activity.Description);
                    }

                    if (activityExecutionResult.ResultKind == ActivityExecutionResultKind.Successful)
                    {
                        double activityCompletionPercentage = double.NaN;
                        if (activityCompletionPercentages.TryGetValue(activity.Id, out activityCompletionPercentage) &&
                            !double.IsNaN(activityCompletionPercentage))
                        {
                            WorkflowEngineContext.EventAggregator.GetMessage<WorkflowProgressChanged>()
                                .Publish(new WorkflowProgressInfo(GetType(), Id, Name, WorkflowContext,
                                    activityCompletionPercentage));
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteDebug($"Internal error occured during during {activity.Description}. {ex}");
                    WorkflowEngineContext.EventAggregator.GetMessage<ErrorOccured>()
                        .Publish(new ErrorMessage(this, "Internal error occured."));
                    workflowExecutionResult.SetFailed();
                    break;
                }
            }

            if (workflowExecutionResult.ResultKind != WorkflowExecutionResultKind.Successful)
            {
                Rollback();
            }

            State = WorkflowState.Stopped;
            WorkflowEngineContext.EventAggregator.GetMessage<WorkflowCompleted>()
                .Publish(new WorkflowCompletedInfo(GetType(), Id, Name, WorkflowContext, workflowExecutionResult));

            WriteDebug("Workflow has completed execution. Result: {0}", workflowExecutionResult.ResultKind);
             
            return workflowExecutionResult;
        }

        private void InjectWorkflowEngineContext(IActivity activity)
        {
            IRequireWorkflowEngineContext requireWorkflowEngineContext = activity as IRequireWorkflowEngineContext;
            if (requireWorkflowEngineContext != null)
            {
                requireWorkflowEngineContext.WorkflowEngineContext = WorkflowEngineContext;
            }
        }

        private void InjectWorkflowContext(IActivity activity)
        {
            IRequireWorkflowContext<TWorkflowContext> requireWorkflowContext =
                activity as IRequireWorkflowContext<TWorkflowContext>;
            if (requireWorkflowContext != null)
            {
                requireWorkflowContext.WorkflowContext = WorkflowContext;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void Rollback()
        {
            if (rollbackActivities.Count == 0)
            {
                WriteDebug("There are no activities to rollback...");
                return;
            }

            WriteDebug("Rolling back...");
            WorkflowEngineContext.EventAggregator.GetMessage<WorkflowRollbackStarted>()
                .Publish(new WorkflowInfo(GetType(), Id, Name, WorkflowContext));

            //Get the activities which needs rollback and is executed
            foreach (
                IActivity stoppedActivity in
                    activities.Where(
                        activity =>
                            activity.State == ActivityState.Stopped)
                        .Reverse())
            {
                IActivity rollbackActivity;
                if (rollbackActivities.TryGetValue(stoppedActivity.Id, out rollbackActivity))
                {
                    try
                    {
                        InjectWorkflowContext(rollbackActivity);
                        InjectWorkflowEngineContext(rollbackActivity);

                        rollbackActivity.Execute();
                    }
                    catch (Exception ex)
                    {
                        WriteDebug("Internal error occured during during {0}. {1}", rollbackActivity.Description,
                            ex);
                        WorkflowEngineContext.EventAggregator.GetMessage<ErrorOccured>()
                            .Publish(new ErrorMessage(this, "Internal error occured."));
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        protected void WriteDebug(string format, params object[] args)
        {
            Guard.Ensure(format, "format").IsNotNullOrEmpty();

            string message = args.Length > 0 ? string.Format(CultureInfo.CurrentCulture, format, args) : format;

            FormattableString.Invariant($"[Workflow({GetType().Name},{Name})] {message}");
        }
    }
}
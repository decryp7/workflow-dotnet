using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GuardLibrary;
using SimpleWorkflow.Events;
using SimpleWorkflow.Events.Payload;

namespace SimpleWorkflow
{
    public class WorkflowEngine : IWorkflowEngine
    {
        private readonly WorkflowEngineContext workflowEngineContext;
        private readonly IList<IWorkflow> workflows = new List<IWorkflow>();
        private bool detectedErrors;
        //Track engine execution progress
        private double progress;
        private double currentWorkflowProgress;
        private WorkflowEngineExecutionResult workflowEngineExecutionResult;

        #region Use for publishing events on the caller thread
        private readonly SynchronizationContext synchronizationContext;
        private SendOrPostCallback started;
        private SendOrPostCallback progressChanged;
        private SendOrPostCallback workflowRollbackStarted;
        private SendOrPostCallback activityExecutionStarted;
        private SendOrPostCallback activityExecutionCompleted;
        private SendOrPostCallback workflowStarted;
        private SendOrPostCallback workflowCompleted;
        private SendOrPostCallback isCancellableChanged;
        private SendOrPostCallback stopped;
        private SendOrPostCallback eventOccurred;
        private SendOrPostCallback errorOccurred;
        private SendOrPostCallback cancellationPending;
        #endregion

        /// <summary>
        /// Get the workflow engine execution result
        /// </summary>
        public WorkflowEngineExecutionResult ExecutionResult
        {
            get
            {
                Guard.EnsureThisConditionIsMet(() => State != WorkflowEngineState.NotStarted)
                    .OrThrowException(new InvalidOperationException("Workflow engine has not started yet!"));
                return workflowEngineExecutionResult;
            }
        }

        /// <summary>
        /// Get the workflow engine state
        /// </summary>
        public WorkflowEngineState State { get; protected set; }

        #region Workflow Engine Events
        /// <summary>
        /// Workflow engine has started event
        /// </summary>
        public event WorkflowEngineStartedEventHandler Started;

        /// <summary>
        /// Workflow engine progress has changed event
        /// </summary>
        public event WorkflowEngineProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Workflow rollback started event
        /// </summary>
        public event WorkflowRollbackStartedEventHandler WorkflowRollbackStarted;

        /// <summary>
        /// Workflow activity started execution event
        /// </summary>
        public event WorkflowActivityExecutionStartedEventHandler ActivityExecutionStarted;

        /// <summary>
        /// Workflow activity execution completed event
        /// </summary>
        public event WorkflowActivityExecutionCompletedEventHandler ActivityExecutionCompleted;

        /// <summary>
        /// Workflow started event
        /// </summary>
        public event WorkflowStartedEventHandler WorkflowStarted;

        /// <summary>
        /// Workflow completed event
        /// </summary>
        public event WorkflowCompletedEventHandler WorkflowCompleted;

        /// <summary>
        /// Workflow engine IsCancellable changed event
        /// </summary>
        public event WorkflowEngineIsCancellableChangedEventHandler IsCancellableChanged;

        /// <summary>
        /// Workflow engine stopped event
        /// </summary>
        public event WorkflowEngineStoppedEventHandler Stopped;

        /// <summary>
        /// Workflow engine event occured event
        /// </summary>
        public event WorkflowEngineEventOccuredEventHandler EventOccurred;

        /// <summary>
        /// Workflow engine error occured event
        /// </summary>
        public event WorkflowEngineErrorOccuredEventHandler ErrorOccurred;

        /// <summary>
        /// Workflow engine cancellation pending event
        /// </summary>
        public event WorkflowEngineCancellationPendingEventHandler CancellationPending;

        #endregion

        #region Protected Properties
        protected WorkflowEngineContext WorkflowEngineContext => workflowEngineContext;

        protected IList<IWorkflow> Workflows => workflows;

        protected bool DetectedErrors
        {
            get => detectedErrors;
            set => detectedErrors = value;
        }

        protected WorkflowEngineExecutionResult WorkflowEngineExecutionResult
        {
            get => workflowEngineExecutionResult;
            set => workflowEngineExecutionResult = value;
        }

        protected SynchronizationContext SynchronizationContext => synchronizationContext;

        protected SendOrPostCallback ProgressChangedCallBack => progressChanged;

        protected SendOrPostCallback WorkflowCompletedCallBack => workflowCompleted;

        #endregion Protected Properties

        public WorkflowEngine()
        {
            if (SynchronizationContext.Current == null)//Normally SynchronizationContext.Current will be null in non-UI thread
            {
                // set this context for this thread.
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            }
            synchronizationContext = SynchronizationContext.Current;
            workflowEngineContext = new WorkflowEngineContext(new WorkflowEngineEventAggregator());

            SetupSendOrPostCallback();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            workflowEngineContext.EventAggregator.GetMessage<WorkflowRollbackStarted>()
                .Subscribe(PostWorkflowRollbackStarted);
            workflowEngineContext.EventAggregator.GetMessage<ActivityExecutionStarted>()
                .Subscribe(PostActivityExecutionStarted);
            workflowEngineContext.EventAggregator.GetMessage<ActivityExecutionCompleted>()
                .Subscribe(PostActivityExecutionCompleted);
            workflowEngineContext.EventAggregator.GetMessage<WorkflowProgressChanged>()
                .Subscribe(PostProgressChanged);
            workflowEngineContext.EventAggregator.GetMessage<WorkflowStarted>().Subscribe(PostWorkflowStarted);
            workflowEngineContext.EventAggregator.GetMessage<WorkflowCompleted>()
                .Subscribe(PostWorkflowCompleted);
            workflowEngineContext.EventAggregator.GetMessage<IsCancellableChanged>()
                .Subscribe(PostIsCancellableChanged);
            workflowEngineContext.EventAggregator.GetMessage<EventOccurred>().Subscribe(PostEventOccurred);
            workflowEngineContext.EventAggregator.GetMessage<ErrorOccured>().Subscribe(PostErrorOccurred);
        }

        private void SetupSendOrPostCallback()
        {
            started = state => { PublishStarted((WorkflowEngineStartedEventArgument)state); };
            progressChanged =
                state => { PublishProgressChangedEvent((WorkflowEngineProgressChangedEventArgument)state); };
            workflowRollbackStarted =
                state => { PublishWorkflowRollbackStartedEvent((WorkflowRollbackStartedEventArgument)state); };
            activityExecutionStarted = state =>
            {
                PublishActivityExecutionStartedEvent((WorkflowActivityExecutionStartedEventArgument)state);
            };
            activityExecutionCompleted = state =>
            {
                PublishActivityExecutionCompletedEvent((WorkflowActivityExecutionCompletedEventArgument)state);
            };
            workflowStarted = state => { PublishWorkflowStartedEvent((WorkflowStartedEventArgument)state); };
            workflowCompleted = state => { PublishWorkflowCompletedEvent((WorkflowCompletedEventArgument)state); };
            isCancellableChanged =
                state => { PublishIsCancellableChanged((WorkflowEngineIsCancellableChangedEventArgument)state); };
            stopped = state => { PublishStopped((WorkflowEngineStoppedEventArgument)state); };
            eventOccurred = state =>
            {
                PublishEventOccuredEvent((WorkflowEngineEventOccurredEventArgument)state);
            };
            errorOccurred = state =>
            {
                PublishErrorOccuredEvent((WorkflowEngineErrorOccurredEventArgument)state);
            };
            cancellationPending = state =>
            {
                PublishCancellationPending((WorkflowEngineCancellationPendingEventArgument)state);
            };
        }

        public IWorkflowEngine OnStarted(WorkflowEngineStartedEventHandler workflowEngineStartedEventHandler)
        {
            Guard.Ensure(workflowEngineStartedEventHandler, nameof(workflowEngineStartedEventHandler)).IsNotNull();
            Started += workflowEngineStartedEventHandler;

            return this;
        }

        public IWorkflowEngine OnProgressChanged(
            WorkflowEngineProgressChangedEventHandler workflowEngineProgressChangedEventHandler)
        {
            Guard.Ensure(workflowEngineProgressChangedEventHandler, nameof(WorkflowEngineProgressChangedEventHandler)).IsNotNull();
            ProgressChanged += workflowEngineProgressChangedEventHandler;

            return this;
        }

        public IWorkflowEngine OnWorkflowRollbackStarted(
            WorkflowRollbackStartedEventHandler workflowRollbackStartedEventHandler)
        {
            Guard.Ensure(workflowRollbackStartedEventHandler, nameof(workflowRollbackStartedEventHandler)).IsNotNull();
            WorkflowRollbackStarted += workflowRollbackStartedEventHandler;

            return this;
        }

        public IWorkflowEngine OnActivityExecutionStarted(
            WorkflowActivityExecutionStartedEventHandler workflowActivityExecutionStartedEventHandler)
        {
            Guard.Ensure(workflowActivityExecutionStartedEventHandler,
                nameof(workflowActivityExecutionStartedEventHandler)).IsNotNull();
            ActivityExecutionStarted += workflowActivityExecutionStartedEventHandler;

            return this;
        }

        public IWorkflowEngine OnActivityExecutionCompleted(
            WorkflowActivityExecutionCompletedEventHandler workflowActivityExecutionCompletedEventHandler)
        {
            Guard.Ensure(workflowActivityExecutionCompletedEventHandler,
                nameof(workflowActivityExecutionCompletedEventHandler)).IsNotNull();
            ActivityExecutionCompleted += workflowActivityExecutionCompletedEventHandler;

            return this;
        }

        public IWorkflowEngine OnWorkflowStarted(WorkflowStartedEventHandler workflowStartedEventHandler)
        {
            Guard.Ensure(workflowStartedEventHandler, nameof(workflowStartedEventHandler)).IsNotNull();
            WorkflowStarted += workflowStartedEventHandler;

            return this;
        }

        public IWorkflowEngine OnWorkflowCompleted(WorkflowCompletedEventHandler workflowCompletedEventHandler)
        {
            Guard.Ensure(workflowCompletedEventHandler, nameof(workflowCompletedEventHandler)).IsNotNull();
            WorkflowCompleted += workflowCompletedEventHandler;

            return this;
        }

        public IWorkflowEngine OnIsCancellableChanged(
            WorkflowEngineIsCancellableChangedEventHandler workflowEngineIsCancellableChangedEventHandler)
        {
            Guard.Ensure(workflowEngineIsCancellableChangedEventHandler,
                nameof(WorkflowEngineIsCancellableChangedEventHandler)).IsNotNull();
            IsCancellableChanged += workflowEngineIsCancellableChangedEventHandler;

            return this;
        }

        public IWorkflowEngine OnStopped(WorkflowEngineStoppedEventHandler workflowEngineStoppedEventHandler)
        {
            Guard.Ensure(workflowEngineStoppedEventHandler, nameof(workflowEngineStoppedEventHandler)).IsNotNull();
            Stopped += workflowEngineStoppedEventHandler;

            return this;
        }

        public IWorkflowEngine OnEventOccured(WorkflowEngineEventOccuredEventHandler workflowEngineEventOccurredEventHandler)
        {
            Guard.Ensure(workflowEngineEventOccurredEventHandler, nameof(workflowEngineEventOccurredEventHandler)).IsNotNull();
            EventOccurred += workflowEngineEventOccurredEventHandler;

            return this;
        }

        public IWorkflowEngine OnErrorOccurred(WorkflowEngineErrorOccuredEventHandler workflowEngineErrorOccurredEventHandler)
        {
            Guard.Ensure(workflowEngineErrorOccurredEventHandler, nameof(workflowEngineErrorOccurredEventHandler)).IsNotNull();
            ErrorOccurred += workflowEngineErrorOccurredEventHandler;

            return this;
        }

        public IWorkflowEngine OnCancellationPending(
            WorkflowEngineCancellationPendingEventHandler workflowEngineCancellationPendingEventHandler)
        {
            Guard.Ensure(workflowEngineCancellationPendingEventHandler, nameof(workflowEngineCancellationPendingEventHandler))
                .IsNotNull();
            CancellationPending += workflowEngineCancellationPendingEventHandler;

            return this;
        }

        public virtual IWorkflowEngine Queue(IWorkflow workflow)
        {
            Guard.Ensure(workflow, nameof(workflow)).IsNotNull();
            workflows.Add(workflow);

            return this;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public virtual WorkflowEngineExecutionResult Run()
        {
            Guard.EnsureThisConditionIsMet(() => State != WorkflowEngineState.Running)
                .OrThrowException(new InvalidOperationException("Workflow engine is already running!"));
            Guard.EnsureThisConditionIsMet(() => workflows.Count > 0)
                .OrThrowException(new InvalidOperationException("There are no workflows queued for execution!"));

            Reset();
            PostStarted();
            State = WorkflowEngineState.Running;

            workflowEngineExecutionResult =
                new WorkflowEngineExecutionResult(workflows).SetCompleted();

            try
            {
                foreach (IWorkflow workflow in workflows)
                {
                    if (workflowEngineContext.CancellationToken.CancellationPending)
                    {
                        workflowEngineExecutionResult.SetCanceled();
                        break;
                    }

                    InjectWorkflowEngineContext(workflow);

                    WorkflowExecutionResult workflowExecutionResult = workflow.Execute();
                    if (workflowExecutionResult.ResultKind == WorkflowExecutionResultKind.Canceled)
                    {
                        workflowEngineExecutionResult.SetCanceled();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDebug($"Internal error occured. {ex}");
                //If unknown exception occurs, publish stop event and send result immediately.
                workflowEngineExecutionResult.SetCompletedWithErrors();
            }
            finally
            {
                State = WorkflowEngineState.Stopped;
            }

            //check number of failed workflows
            if (workflowEngineExecutionResult.ResultKind != WorkflowEngineExecutionResultKind.Canceled &&
                (workflows.Any(
                    workflow =>
                        workflow.State == WorkflowState.Stopped &&
                        workflow.ExecutionResult.ResultKind == WorkflowExecutionResultKind.Failed) || detectedErrors))
            {
                workflowEngineExecutionResult.SetCompletedWithErrors();
            }

            //Publish stop event
            PostStopped(workflowEngineExecutionResult);
            return workflowEngineExecutionResult;
        }

        public Task<WorkflowEngineExecutionResult> RunAsync()
        {
            Guard.EnsureThisConditionIsMet(() => State != WorkflowEngineState.Running)
                .OrThrowException(new InvalidOperationException("Workflow engine is already running!"));
            Guard.EnsureThisConditionIsMet(() => workflows.Count > 0)
                .OrThrowException(new InvalidOperationException("There are no workflows queued for execution!"));

            WriteDebug("Executing workflows on a new thread...");
            return Task.Run(() => Run());
        }

        public virtual void CancelAsync()
        {
            Guard.EnsureThisConditionIsMet(() => State == WorkflowEngineState.Running &&
                                                 !workflowEngineContext.CancellationToken.CancellationPending)
                .OrThrowException(new InvalidOperationException("Workflow engine is not running or already cancelled"));

            WriteDebug("Workflow engine received cancellation request.");
                PostIsCancellableChanged(false);
                PostCancellationPending();
                workflowEngineContext.CancellationToken.Cancel();
        }

        public void UnsubscribeAll<TTarget>(TTarget target)
        {
            EventInfo[] eventInfos = GetType().GetEvents();

            foreach (EventInfo eventInfo in eventInfos)
            {
                Type declaringType = eventInfo.DeclaringType;
                FieldInfo fieldInfo = declaringType.GetField(eventInfo.Name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                    BindingFlags.Static);
                if (fieldInfo != null)
                {
                    Delegate eventDelegate = fieldInfo.GetValue(this) as Delegate;
                    if (eventDelegate != null)
                    {
                        foreach (Delegate @delegate in eventDelegate.GetInvocationList())
                        {
                            if (@delegate.Target.Equals(target))
                            {
                                eventInfo.RemoveEventHandler(this, @delegate);
                            }
                        }
                    }
                }
            }

        }

        protected virtual void InjectWorkflowEngineContext(IWorkflow workflow)
        {
            if (workflow is IRequireWorkflowEngineContext requireWorkflowEngineContext)
            {
                requireWorkflowEngineContext.WorkflowEngineContext = workflowEngineContext;
            }
        }

        protected virtual void Reset()
        {
            State = WorkflowEngineState.NotStarted;
            progress = 0;
            currentWorkflowProgress = 0;
            workflowEngineContext.CancellationToken.Reset();
            workflowEngineExecutionResult = null;

            foreach (IWorkflow workflow in workflows)
            {
                workflow.Reset();
            }
        }

        protected virtual void PostStarted()
        {
            synchronizationContext?.Post(started, new WorkflowEngineStartedEventArgument(workflows.Count));
        }

        protected virtual void PostProgressChanged(WorkflowProgressInfo workflowProgressInfo)
        {
            if (synchronizationContext == null || 
                workflowProgressInfo == null ||
                double.IsNaN(workflowProgressInfo.CompletionPercentage))
            {
                return;
            }

            currentWorkflowProgress = (workflowProgressInfo.CompletionPercentage / 100) * (100 / (double)workflows.Count);
            synchronizationContext.Post(progressChanged,
                new WorkflowEngineProgressChangedEventArgument(progress + currentWorkflowProgress));
        }

        protected virtual void PostWorkflowRollbackStarted(WorkflowInfo workflowInfo)
        {
            if (synchronizationContext == null || workflowInfo == null)
            {
                return;
            }

            synchronizationContext.Post(workflowRollbackStarted,
                    new WorkflowRollbackStartedEventArgument(workflowInfo.WorkflowType, workflowInfo.Id, workflowInfo.Name,
                        workflowInfo.WorkflowContext));
        }

        protected virtual void PostActivityExecutionStarted(ActivityExecutionStartedInfo activityExecutionInfo)
        {
            if (synchronizationContext == null || activityExecutionInfo == null)
            {
                return;
            }

            synchronizationContext.Post(activityExecutionStarted,
                    new WorkflowActivityExecutionStartedEventArgument(activityExecutionInfo.Id,
                        activityExecutionInfo.Description,
                        activityExecutionInfo.IsCancellable));
        }

        protected virtual void PostActivityExecutionCompleted(ActivityExecutionCompletedInfo activityExecutionCompletedInfo)
        {
            if (synchronizationContext == null || activityExecutionCompletedInfo == null)
            {
                return;
            }

            synchronizationContext.Post(activityExecutionCompleted,
                new WorkflowActivityExecutionCompletedEventArgument(activityExecutionCompletedInfo.Id,
                    activityExecutionCompletedInfo.Description,
                    activityExecutionCompletedInfo.Result));
        }

        protected virtual void PostWorkflowStarted(WorkflowInfo workflowInfo)
        {
            if (synchronizationContext == null || workflowInfo == null)
            {
                return;
            }

            synchronizationContext.Post(workflowStarted,
                new WorkflowStartedEventArgument(workflowInfo.WorkflowType, workflowInfo.Id, workflowInfo.Name,
                    workflowInfo.WorkflowContext,
                    workflows.IndexOf(workflows.FirstOrDefault(w => w.Id.Equals(workflowInfo.Id))) + 1,
                    workflows.Count));
        }

        protected virtual void PostWorkflowCompleted(WorkflowCompletedInfo workflowCompletedInfo)
        {
            if (synchronizationContext == null || workflowCompletedInfo == null)
            {
                return;
            }

            progress += (double)100 / (double)workflows.Count;

            synchronizationContext.Post(progressChanged,
                new WorkflowEngineProgressChangedEventArgument(progress));

            synchronizationContext.Post(workflowCompleted,
                new WorkflowCompletedEventArgument(workflowCompletedInfo.WorkflowType, workflowCompletedInfo.Id,
                    workflowCompletedInfo.Name,
                    workflowCompletedInfo.WorkflowContext, workflowCompletedInfo.Result));
        }

        protected virtual void PostIsCancellableChanged(bool newValue)
        {
            synchronizationContext?.Post(isCancellableChanged, new WorkflowEngineIsCancellableChangedEventArgument(newValue));
        }

        protected virtual void PostStopped(WorkflowEngineExecutionResult result)
        {
            synchronizationContext?.Post(stopped, new WorkflowEngineStoppedEventArgument(result));
        }

        protected virtual void PostEventOccurred(EventMessage eventMessage)
        {
            if (synchronizationContext != null && eventMessage != null)
            {
                synchronizationContext.Post(eventOccurred,
                    new WorkflowEngineEventOccurredEventArgument(
                        eventMessage.Source,
                        eventMessage.Event));
            }
        }

        protected virtual void PostErrorOccurred(ErrorMessage errorMessage)
        {
            detectedErrors = true;

            if (synchronizationContext != null && errorMessage != null)
            {
                synchronizationContext.Post(errorOccurred,
                    new WorkflowEngineErrorOccurredEventArgument(errorMessage.Source, errorMessage.Error));
            }
        }

        protected virtual void PostCancellationPending()
        {
            synchronizationContext?.Post(cancellationPending, new WorkflowEngineCancellationPendingEventArgument());
        }

        private void PublishStarted(WorkflowEngineStartedEventArgument arg)
        {
            Started?.Invoke(this, arg);
        }

        private void PublishProgressChangedEvent(WorkflowEngineProgressChangedEventArgument arg)
        {
            ProgressChanged?.Invoke(this, arg);
        }

        private void PublishWorkflowRollbackStartedEvent(WorkflowRollbackStartedEventArgument arg)
        {
            WorkflowRollbackStarted?.Invoke(this, arg);
        }

        private void PublishActivityExecutionStartedEvent(WorkflowActivityExecutionStartedEventArgument arg)
        {
            ActivityExecutionStarted?.Invoke(this, arg);
        }

        private void PublishActivityExecutionCompletedEvent(WorkflowActivityExecutionCompletedEventArgument arg)
        {
            ActivityExecutionCompleted?.Invoke(this, arg);
        }

        private void PublishWorkflowStartedEvent(WorkflowStartedEventArgument arg)
        {
            WorkflowStarted?.Invoke(this, arg);
        }

        private void PublishWorkflowCompletedEvent(WorkflowCompletedEventArgument arg)
        {
            WorkflowCompleted?.Invoke(this, arg);
        }

        private void PublishIsCancellableChanged(WorkflowEngineIsCancellableChangedEventArgument arg)
        {
            IsCancellableChanged?.Invoke(this, arg);
        }

        private void PublishStopped(WorkflowEngineStoppedEventArgument arg)
        {
            Stopped?.Invoke(this, arg);
        }

        private void PublishEventOccuredEvent(WorkflowEngineEventOccurredEventArgument arg)
        {
            EventOccurred?.Invoke(this, arg);
        }

        private void PublishErrorOccuredEvent(WorkflowEngineErrorOccurredEventArgument arg)
        {
            ErrorOccurred?.Invoke(this, arg);
        }

        private void PublishCancellationPending(WorkflowEngineCancellationPendingEventArgument arg)
        {
            CancellationPending?.Invoke(this, arg);
        }

        protected virtual void WriteDebug(string format, params object[] args)
        {
            Guard.Ensure(format, "format").IsNotNullOrEmpty();

            if (args != null)
            {
                string message = args.Length > 0 ? string.Format(CultureInfo.CurrentCulture, format, args) : format;

                FormattableString.Invariant($"[WorkflowEngine] {message}").WriteDebug();
            }
        }
    }
}
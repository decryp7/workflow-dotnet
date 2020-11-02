namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow engine started event
    /// </summary>
    /// <param name="sender">Workflow engine</param>
    /// <param name="arg">Workflow engine started event argument</param>
    public delegate void WorkflowEngineStartedEventHandler(object sender, WorkflowEngineStartedEventArgument e);

    /// <summary>
    /// Workflow engine progress changed event
    /// </summary>
    /// <param name="sender">Workflow engine</param>
    /// <param name="arg">Workflow engine progress changed event argument</param>
    public delegate void WorkflowEngineProgressChangedEventHandler(object sender, WorkflowEngineProgressChangedEventArgument e);

    /// <summary>
    /// Workflow engine rollback started event
    /// </summary>
    /// <param name="sender">Workflow engine</param>
    /// <param name="arg">Workflow engine rollback started event argument</param>
    public delegate void WorkflowRollbackStartedEventHandler(object sender, WorkflowRollbackStartedEventArgument e);

    /// <summary>
    /// Workflow activity started event
    /// </summary>
    /// <param name="sender">Workflow activity</param>
    /// <param name="arg">Workflow activity started event argument</param>
    public delegate void WorkflowActivityExecutionStartedEventHandler(
        object sender, WorkflowActivityExecutionStartedEventArgument e);

    /// <summary>
    /// Workflow activity execution completed event
    /// </summary>
    /// <param name="sender">Workflow activity</param>
    /// <param name="arg">Workflow activity completed event argument</param>
    public delegate void WorkflowActivityExecutionCompletedEventHandler(
        object sender, WorkflowActivityExecutionCompletedEventArgument e);

    /// <summary>
    /// Workflow started event
    /// </summary>
    /// <param name="sender">Workflow</param>
    /// <param name="arg">Workflow started event argument</param>
    public delegate void WorkflowStartedEventHandler(object sender, WorkflowStartedEventArgument e);

    /// <summary>
    /// Workflow completed event
    /// </summary>
    /// <param name="sender">Workflow</param>
    /// <param name="arg">Workflow completed event argument</param>
    public delegate void WorkflowCompletedEventHandler(object sender, WorkflowCompletedEventArgument e);

    /// <summary>
    /// Workflow engine IsCancellable changed event
    /// </summary>
    /// <param name="sender">Workflow engine</param>
    /// <param name="arg">Workflow engine IsCancellable changed event argument</param>
    public delegate void WorkflowEngineIsCancellableChangedEventHandler(
        object sender, WorkflowEngineIsCancellableChangedEventArgument e);

    /// <summary>
    /// Workflow engine stopped event
    /// </summary>
    /// <param name="sender">Workflow engine</param>
    /// <param name="arg">Workflow engine stopped event argument</param>
    public delegate void WorkflowEngineStoppedEventHandler(
        object sender, WorkflowEngineStoppedEventArgument e);

    /// <summary>
    /// Workflow engine event occured event
    /// </summary>
    /// <param name="sender">Workflow engine</param>
    /// <param name="arg">Workflow engine event occured argument</param>
    public delegate void WorkflowEngineEventOccuredEventHandler(
        object sender, WorkflowEngineEventOccurredEventArgument e);

    /// <summary>
    /// Workflow engine error occured event
    /// </summary>
    /// <param name="sender">Workflow engine</param>
    /// <param name="arg">Workflow engine error occured argument</param>
    public delegate void WorkflowEngineErrorOccuredEventHandler(
        object sender, WorkflowEngineErrorOccurredEventArgument e);

    /// <summary>
    /// Workflow engine cancellation pending event
    /// </summary>
    /// <param name="sender">Workflow engine</param>
    /// <param name="arg">Workflow engine cancellation pending event argument</param>
    public delegate void WorkflowEngineCancellationPendingEventHandler(
        object sender, WorkflowEngineCancellationPendingEventArgument e);

    /// <summary>
    /// Workflow engine interface
    /// </summary>
    public interface IWorkflowEngine
    {
        /// <summary>
        /// Get the workflow engine execution result
        /// </summary>
        WorkflowEngineExecutionResult ExecutionResult { get; }

        /// <summary>
        /// Get the workflow engine state
        /// </summary>
        WorkflowEngineState State { get; }

        /// <summary>
        /// Workflow engine has started event
        /// </summary>
        event WorkflowEngineStartedEventHandler Started;

        /// <summary>
        /// Workflow engine progress has changed event
        /// </summary>
        event WorkflowEngineProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Workflow activity started execution event
        /// </summary>
        event WorkflowActivityExecutionStartedEventHandler ActivityExecutionStarted;

        /// <summary>
        /// Workflow activity execution completed event
        /// </summary>
        event WorkflowActivityExecutionCompletedEventHandler ActivityExecutionCompleted;

        /// <summary>
        /// Workflow rollback started event
        /// </summary>
        event WorkflowRollbackStartedEventHandler WorkflowRollbackStarted;

        /// <summary>
        /// Workflow started event
        /// </summary>
        event WorkflowStartedEventHandler WorkflowStarted;

        /// <summary>
        /// Workflow completed event
        /// </summary>
        event WorkflowCompletedEventHandler WorkflowCompleted;

        /// <summary>
        /// Workflow engine IsCancellable changed event
        /// </summary>
        event WorkflowEngineIsCancellableChangedEventHandler IsCancellableChanged;

        /// <summary>
        /// Workflow engine stopped event
        /// </summary>
        event WorkflowEngineStoppedEventHandler Stopped;

        /// <summary>
        /// Workflow engine event occured event
        /// </summary>
        event WorkflowEngineEventOccuredEventHandler EventOccurred;

        /// <summary>
        /// Workflow engine error occured event
        /// </summary>
        event WorkflowEngineErrorOccuredEventHandler ErrorOccurred;

        /// <summary>
        /// Workflow engine cancellation pending event
        /// </summary>
        event WorkflowEngineCancellationPendingEventHandler CancellationPending;

        /// <summary>
        /// Subscribe to workflow engine started event
        /// </summary>
        /// <example>workflowEngine.OnStarted(eventHandler);</example>
        /// <param name="workflowEngineStartedEventHandler">Event handler</param>
        /// <returns>Workflow engine</returns>
        IWorkflowEngine OnStarted(WorkflowEngineStartedEventHandler workflowEngineStartedEventHandler);

        /// <summary>
        /// Subscribe to workflow engine progress changed event
        /// </summary>
        /// <example>workflowEngine.OnProgressChanged(eventHandler);</example>
        /// <param name="workflowEngineProgressChangedEventHandler">Event handler</param>
        /// <returns>Workflow engine</returns>
        IWorkflowEngine OnProgressChanged(
            WorkflowEngineProgressChangedEventHandler workflowEngineProgressChangedEventHandler);

        /// <summary>
        /// Subscribe to workflow rollback started event
        /// </summary>
        /// <example>workflowEngine.OnRollbackStarted(eventHandler);</example>
        /// <param name="workflowRollbackStartedEventHandler"></param>
        /// <returns>Workflow engine</returns>
        IWorkflowEngine OnWorkflowRollbackStarted(
            WorkflowRollbackStartedEventHandler workflowRollbackStartedEventHandler);

        /// <summary>
        /// Subscribe to workflow activity execution started event
        /// </summary>
        /// <example>workflowEngine.OnActivityExecutionStarted(eventHandler);</example>
        /// <param name="workflowActivityExecutionStartedEventHandler">EventHandler</param>
        /// <returns>Workflow engine</returns>
        IWorkflowEngine OnActivityExecutionStarted(
            WorkflowActivityExecutionStartedEventHandler workflowActivityExecutionStartedEventHandler);

        /// <summary>
        /// Subscribe to workflow activity execution completed event
        /// </summary>
        /// <example>workflowEngine.OnActivityExecutionCompleted(eventHandler);</example>
        /// <param name="workflowActivityExecutionCompletedEventHandler">EventHandler</param>
        /// <returns>Workflow engine</returns>
        IWorkflowEngine OnActivityExecutionCompleted(
            WorkflowActivityExecutionCompletedEventHandler workflowActivityExecutionCompletedEventHandler);

        /// <summary>
        /// Subscribe to workflow started event
        /// </summary>
        /// <example>workflowEngine.OnWorkflowStarted(eventHandler);</example>
        /// <param name="workflowStartedEventHandler">EventHandler</param>
        /// <returns>Workflow engine</returns>
        IWorkflowEngine OnWorkflowStarted(WorkflowStartedEventHandler workflowStartedEventHandler);

        /// <summary>
        /// Subscribe to workflow completed event
        /// </summary>
        /// <example>workflowEngine.OnWorkflowCompleted(eventHandler);</example>
        /// <param name="workflowCompletedEventHandler">EventHandler</param>
        /// <returns>Workflow engine</returns>
        IWorkflowEngine OnWorkflowCompleted(WorkflowCompletedEventHandler workflowCompletedEventHandler);

        /// <summary>
        /// Subscribe to workflow engine IsCancellable change event
        /// </summary>
        /// <example>workflowEngine.OnIsCancellableChanged(eventHandler);</example>
        /// <param name="workflowEngineIsCancellableChangedEventHandler">EventHandler</param>
        /// <returns>Workflow engine</returns>
        IWorkflowEngine OnIsCancellableChanged(
            WorkflowEngineIsCancellableChangedEventHandler workflowEngineIsCancellableChangedEventHandler);

        /// <summary>
        /// Subscribe to workflow engine stopped event
        /// </summary>
        /// <example>workflowEngine.OnIsCancellableChanged(eventHandler);</example>
        /// <param name="workflowEngineStoppedEventHandler">EventHandler</param>
        /// <returns>Workflow engine</returns>
        IWorkflowEngine OnStopped(WorkflowEngineStoppedEventHandler workflowEngineStoppedEventHandler);

        /// <summary>
        /// Subscribe to workflow engine event occured event
        /// </summary>
        /// <example>workflowEngine.OnEventOccured(eventHandler);</example>
        /// <param name="workflowEngineEventOccurredEventHandler">EventHandler</param>
        /// <returns>Workflow engine</returns>
        IWorkflowEngine OnEventOccured(WorkflowEngineEventOccuredEventHandler workflowEngineEventOccurredEventHandler);

        /// <summary>
        /// Subscribe to workflow engine error occured event
        /// </summary>
        /// <example>workflowEngine.OnErrorOccured(eventHandler);</example>
        /// <param name="workflowEngineErrorOccurredEventHandler">EventHandler</param>
        /// <returns>Workflow engine</returns>
        IWorkflowEngine OnErrorOccurred(WorkflowEngineErrorOccuredEventHandler workflowEngineErrorOccurredEventHandler);

        /// <summary>
        /// Subscribe to workflow engine cancellation pending event
        /// </summary>
        /// <example>workflowEngine.OnCancellationPending(eventHandler);</example>
        /// <param name="workflowEngineCancellationPendingEventHandler">EventHandler</param>
        /// <returns>Workflow engine</returns>
        IWorkflowEngine OnCancellationPending(
            WorkflowEngineCancellationPendingEventHandler workflowEngineCancellationPendingEventHandler);

        /// <summary>
        /// Queue a workflow
        /// </summary>
        /// <param name="workflow">Workflow</param>
        /// <returns>Workflow engine</returns>
        IWorkflowEngine Queue(IWorkflow workflow);

        /// <summary>
        /// Run the workflows
        /// </summary>
        /// <returns>Workflow engine execution result</returns>
        WorkflowEngineExecutionResult Run();

        /// <summary>
        /// Run the workflows in another thread without blocking the current thread
        /// </summary>
        void RunAsync();

        /// <summary>
        /// Stop the workflow engine
        /// </summary>
        void CancelAsync();

        /// <summary>
        /// Unsubscribe all handlers
        /// </summary>
        /// <param name="target">Target</param>
        void UnsubscribeAll<TTarget>(TTarget target);
    }
}
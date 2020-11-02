using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleWorkflow.Test
{
public class DummyWorkflowEngineEventMonitor
    {
        private readonly IList<double> completionPercentages;
        private readonly IWorkflowEngine workflowEngine;

        public bool OnErrorTriggered { get; private set; }
        public bool OnEventTriggered { get; private set; }
        public bool OnCancellationPendingTriggered { get; private set; }
        public bool IsCancellableChangedTriggered { get; private set; }
        public bool RollbackTriggered { get; private set; }
        public int ActivityExecutionStartedCount { get; private set; }
        public int ActivityExecutionCompletedCount { get; private set; }
        public int WorkflowExecutionStartCount { get; private set; }
        public int WorkflowExecutionCompletedCount { get; private set; }
        public readonly IReadOnlyList<double> CompletionPercentages;

        public DummyWorkflowEngineEventMonitor(IWorkflowEngine workflowEngine)
        {
            this.workflowEngine = workflowEngine;

            completionPercentages = new List<double>();
            CompletionPercentages = new ReadOnlyCollection<double>(completionPercentages);

            workflowEngine.OnStarted(WorkflowEngineStartedEventHandler);
            workflowEngine.OnProgressChanged(WorkflowEngineProgressChangedEventHandler);
            workflowEngine.OnWorkflowRollbackStarted(WorkflowRollbackStartedEventHandler);
            workflowEngine.OnActivityExecutionStarted(WorkflowActivityExecutionStartedEventHandler);
            workflowEngine.OnActivityExecutionCompleted(WorkflowActivityExecutionCompletedEventHandler);
            workflowEngine.OnWorkflowStarted(WorkflowStartedEventHandler);
            workflowEngine.OnWorkflowCompleted(WorkflowCompletedEventHandler);
            workflowEngine.OnIsCancellableChanged(WorkflowEngineIsCancellableChangedEventHandler);
            workflowEngine.OnCancellationPending(WorkflowEngineCancellationPendingEventHandler);
            workflowEngine.OnErrorOccurred(WorkflowEngineErrorOccurredEventHandler);
            workflowEngine.OnEventOccured(WorkflowEngineEventOccurredEventHandler);
        }

        private void WorkflowEngineEventOccurredEventHandler(object sender,
            WorkflowEngineEventOccurredEventArgument workflowEngineEventOccurredEventArgument)
        {
            OnEventTriggered = true;
        }

        private void WorkflowEngineErrorOccurredEventHandler(object sender,
            WorkflowEngineErrorOccurredEventArgument workflowEngineErrorOccurredEventArgument)
        {
            OnErrorTriggered = true;
        }

        private void WorkflowEngineCancellationPendingEventHandler(object sender, WorkflowEngineCancellationPendingEventArgument workflowEngineCancellationPendingEventArgument)
        {
            OnCancellationPendingTriggered = true;
        }

        private void WorkflowEngineIsCancellableChangedEventHandler(object sender,
            WorkflowEngineIsCancellableChangedEventArgument workflowEngineIsCancellableChangedEventArgument)
        {
            IsCancellableChangedTriggered = true;
        }

        public void UnsubscribeAll()
        {
            workflowEngine.UnsubscribeAll(this);
        }

        private void WorkflowCompletedEventHandler(object sender,
            WorkflowCompletedEventArgument workflowCompletedEventArgument)
        {
            WorkflowExecutionCompletedCount++;
        }

        private void WorkflowStartedEventHandler(object sender,
            WorkflowStartedEventArgument workflowStartedEventArgument)
        {
            WorkflowExecutionStartCount++;
        }

        private void WorkflowActivityExecutionCompletedEventHandler(object sender,
            WorkflowActivityExecutionCompletedEventArgument workflowActivityExecutionCompletedEventArgument)
        {
            ActivityExecutionCompletedCount++;
        }

        private void WorkflowActivityExecutionStartedEventHandler(object sender,
            WorkflowActivityExecutionStartedEventArgument workflowActivityExecutionStartedEventArgument)
        {
            ActivityExecutionStartedCount++;
        }

        private void WorkflowRollbackStartedEventHandler(object sender,
            WorkflowRollbackStartedEventArgument workflowRollbackStartedEventArgument)
        {
            RollbackTriggered = true;
        }

        private void WorkflowEngineStartedEventHandler(object sender,
            WorkflowEngineStartedEventArgument workflowEngineStartedEventArgument)
        {
            Reset();
        }

        private void WorkflowEngineProgressChangedEventHandler(object sender,
            WorkflowEngineProgressChangedEventArgument workflowEngineProgressChangedEventArgument)
        {
            completionPercentages.Add(workflowEngineProgressChangedEventArgument.CompletionPercentage);
        }

        private void Reset()
        {
            OnErrorTriggered = false;
            OnEventTriggered = false;
            OnCancellationPendingTriggered = false;
            RollbackTriggered = false;
            IsCancellableChangedTriggered = false;
            ActivityExecutionStartedCount = 0;
            ActivityExecutionCompletedCount = 0;
            WorkflowExecutionStartCount = 0;
            WorkflowExecutionCompletedCount = 0;
            completionPercentages.Clear();
        }
    }
}
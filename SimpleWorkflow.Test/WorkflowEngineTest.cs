using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleWorkflow.Test
{
[TestClass]
    public class WorkflowEngineTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            SynchronizationContext.SetSynchronizationContext(new TestSynchronizationContext());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WorkflowEngine_NoWorkflow_InvalidOperationException()
        {
            IWorkflowEngine workflowEngine = new WorkflowEngine();
            workflowEngine.RunAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WorkflowEngine_RunAsyncAlreadyRunning_InvalidOperationException()
        {
            IWorkflow dummyWorkflowOne = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowTwo = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowThree = new DummyWorkflow(new DummyWorkflowContext());

            IWorkflowEngine workflowEngine = new WorkflowEngine();
            ManualResetEvent workflowEngineStarted = new ManualResetEvent(false);
            dummyWorkflowOne.Do(
            new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetSuccessful()));

            workflowEngine.Queue(dummyWorkflowOne);
            workflowEngine.OnStarted((sender, argument) =>
            {
                workflowEngineStarted.Set();
            });

            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.NotStarted);
            workflowEngine.RunAsync();
            //wait for workflow engine to start
            workflowEngineStarted.WaitOne();
            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.Running);
            workflowEngine.RunAsync();
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void WorkflowEngine_NotStartedCancelAsync_InvalidOperationException()
        {
            IWorkflowEngine workflowEngine = new WorkflowEngine();
            IWorkflow dummyWorkflowOne = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowTwo = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowThree = new DummyWorkflow(new DummyWorkflowContext());

            dummyWorkflowOne.Do(
                new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetSuccessful()));

            workflowEngine.Queue(dummyWorkflowOne);
            workflowEngine.CancelAsync();
        }

        [TestMethod]
        public void WorkflowEngine_CompletedSuccessfully()
        {
            IWorkflowEngine workflowEngine = new WorkflowEngine();
            IWorkflow dummyWorkflowOne = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowTwo = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowThree = new DummyWorkflow(new DummyWorkflowContext());

            ManualResetEvent workflowEngineStarted = new ManualResetEvent(false);
            ManualResetEvent workflowEngineStopped = new ManualResetEvent(false);
            dummyWorkflowOne.Do(
            new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetSuccessful()));

            workflowEngine.Queue(dummyWorkflowOne);
            workflowEngine.OnStarted((sender, argument) =>
            {
                workflowEngineStarted.Set();
            });
            workflowEngine.OnStopped((sender, argument) =>
            {
                workflowEngineStopped.Set();
            });

            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.NotStarted);
            workflowEngine.RunAsync();
            //wait for workflow engine to start
            workflowEngineStarted.WaitOne();
            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.Running);
            //wait for workflow engine to stop
            workflowEngineStopped.WaitOne();
            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.Stopped);
            Assert.IsTrue(workflowEngine.ExecutionResult.ResultKind == WorkflowEngineExecutionResultKind.Completed);
        }

        [TestMethod]
        public void WorkflowEngine_ExecuteWithOneFailedWorkflow_GetExecutionResultCompletedWithErrors()
        {
            IWorkflowEngine workflowEngine = new WorkflowEngine();
            IWorkflow dummyWorkflowOne = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowTwo = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowThree = new DummyWorkflow(new DummyWorkflowContext());

            ManualResetEvent workflowEngineStarted = new ManualResetEvent(false);
            ManualResetEvent workflowEngineStopped = new ManualResetEvent(false);
            dummyWorkflowOne.Do(
            new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetSuccessful()));
            dummyWorkflowTwo.Do(new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetFailed()));
            dummyWorkflowThree.Do(new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetSuccessful()));

            workflowEngine
                .Queue(dummyWorkflowOne)
                .Queue(dummyWorkflowTwo)
                .Queue(dummyWorkflowThree);
            workflowEngine.OnStarted((sender, argument) =>
            {
                workflowEngineStarted.Set();
            });
            workflowEngine.OnStopped((sender, argument) =>
            {
                workflowEngineStopped.Set();
            });

            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.NotStarted);
            workflowEngine.RunAsync();
            //wait for workflow engine to start
            workflowEngineStarted.WaitOne();
            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.Running);
            //wait for workflow engine to stop
            workflowEngineStopped.WaitOne();
            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.Stopped);
            Assert.IsTrue(workflowEngine.ExecutionResult.ResultKind == WorkflowEngineExecutionResultKind.CompletedWithErrors);
            Assert.IsTrue(dummyWorkflowOne.State == WorkflowState.Stopped);
            Assert.IsTrue(dummyWorkflowTwo.State == WorkflowState.Stopped);
            //Even though DummyWorkflowTwo failed, workflowEngine will still continue to run remaining workflow
            Assert.IsTrue(dummyWorkflowThree.State == WorkflowState.Stopped);
        }

        [TestMethod]
        public void WorkflowEngine_TestNormalEvents()
        {
            IWorkflowEngine workflowEngine = new WorkflowEngine();
            DummyWorkflowEngineEventMonitor dummyWorkflowEngineEventMonitor =
                new DummyWorkflowEngineEventMonitor(workflowEngine);

            IWorkflow dummyWorkflowOne = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowTwo = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowThree = new DummyWorkflow(new DummyWorkflowContext());

            ManualResetEvent workflowEngineStarted = new ManualResetEvent(false);
            ManualResetEvent workflowEngineStopped = new ManualResetEvent(false);

            dummyWorkflowOne.Do(
                new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetSuccessful(), publishError: true,
                    publishEvent: true))
                .SetCompletionPercentage(100);

            dummyWorkflowTwo.Do(new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetFailed()))
                .SetCompletionPercentage(100)
                .SetRollbackActivity(
                    new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetSuccessful(),
                        isCancellable: false));

            dummyWorkflowThree.Do(new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetSuccessful()))
                .SetCompletionPercentage(100);

            workflowEngine
                .Queue(dummyWorkflowOne)
                .Queue(dummyWorkflowTwo)
                .Queue(dummyWorkflowThree);

            workflowEngine.OnStarted((sender, argument) =>
            {
                workflowEngineStarted.Set();
            });
            workflowEngine.OnStopped((sender, argument) =>
            {
                workflowEngineStopped.Set();
            });

            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.NotStarted);
            workflowEngine.RunAsync();
            //wait for workflow engine to start
            workflowEngineStarted.WaitOne();
            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.Running);
            //wait for workflow engine to stop
            workflowEngineStopped.WaitOne();
            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.Stopped);
            Assert.IsTrue(workflowEngine.ExecutionResult.ResultKind == WorkflowEngineExecutionResultKind.CompletedWithErrors);
            Assert.IsTrue(dummyWorkflowOne.State == WorkflowState.Stopped);
            Assert.IsTrue(dummyWorkflowTwo.State == WorkflowState.Stopped);
            //Even though DummyWorkflowTwo failed, workflowEngine will still continue to run remaining workflow
            Assert.IsTrue(dummyWorkflowThree.State == WorkflowState.Stopped);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.RollbackTriggered);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.OnErrorTriggered);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.OnEventTriggered);
            //including rollback activity
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.ActivityExecutionStartedCount == 4);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.ActivityExecutionCompletedCount == 4);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.WorkflowExecutionStartCount == 3);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.WorkflowExecutionCompletedCount == 3);
        }

        [TestMethod]
        public void WorkflowEngine_UnsubscribeAllEvents()
        {
            IWorkflowEngine workflowEngine = new WorkflowEngine();
            DummyWorkflowEngineEventMonitor dummyWorkflowEngineEventMonitor =
                new DummyWorkflowEngineEventMonitor(workflowEngine);
            dummyWorkflowEngineEventMonitor.UnsubscribeAll();

            IWorkflow dummyWorkflowOne = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowTwo = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowThree = new DummyWorkflow(new DummyWorkflowContext());

            ManualResetEvent workflowEngineStarted = new ManualResetEvent(false);
            ManualResetEvent workflowEngineStopped = new ManualResetEvent(false);

            dummyWorkflowOne.Do(
                new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetSuccessful()))
                .SetCompletionPercentage(100);

            dummyWorkflowTwo.Do(new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetFailed()))
                .SetCompletionPercentage(100)
                .SetRollbackActivity(
                    new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetSuccessful()));

            dummyWorkflowThree.Do(new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetSuccessful()))
                .SetCompletionPercentage(100);

            workflowEngine
                .Queue(dummyWorkflowOne)
                .Queue(dummyWorkflowTwo)
                .Queue(dummyWorkflowThree);

            workflowEngine.OnStarted((sender, argument) =>
            {
                workflowEngineStarted.Set();
            });
            workflowEngine.OnStopped((sender, argument) =>
            {
                workflowEngineStopped.Set();
            });

            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.NotStarted);
            workflowEngine.RunAsync();
            //wait for workflow engine to start
            workflowEngineStarted.WaitOne();
            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.Running);
            //wait for workflow engine to stop
            workflowEngineStopped.WaitOne();
            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.Stopped);
            Assert.IsTrue(workflowEngine.ExecutionResult.ResultKind == WorkflowEngineExecutionResultKind.CompletedWithErrors);
            Assert.IsTrue(dummyWorkflowOne.State == WorkflowState.Stopped);
            Assert.IsTrue(dummyWorkflowTwo.State == WorkflowState.Stopped);
            //Even though DummyWorkflowTwo failed, workflowEngine will still continue to run remaining workflow
            Assert.IsTrue(dummyWorkflowThree.State == WorkflowState.Stopped);
            //No event recieved, so all the data are either false or 0
            Assert.IsFalse(dummyWorkflowEngineEventMonitor.RollbackTriggered);
            Assert.IsFalse(dummyWorkflowEngineEventMonitor.OnErrorTriggered);
            Assert.IsFalse(dummyWorkflowEngineEventMonitor.OnEventTriggered);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.ActivityExecutionStartedCount == 0);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.ActivityExecutionCompletedCount == 0);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.WorkflowExecutionStartCount == 0);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.WorkflowExecutionCompletedCount == 0);
        }

        [TestMethod]
        public void WorkflowEngine_CancelAsync_GetExecutionResultCancelled()
        {
            IWorkflowEngine workflowEngine = new WorkflowEngine();
            DummyWorkflowEngineEventMonitor dummyWorkflowEngineEventMonitor =
                new DummyWorkflowEngineEventMonitor(workflowEngine);

            IWorkflow dummyWorkflowOne = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowTwo = new DummyWorkflow(new DummyWorkflowContext());
            IWorkflow dummyWorkflowThree = new DummyWorkflow(new DummyWorkflowContext());

            ManualResetEvent workflowEngineStarted = new ManualResetEvent(false);
            ManualResetEvent workflowEngineStopped = new ManualResetEvent(false);
            ManualResetEvent firstActivityCompleted = new ManualResetEvent(false);

            dummyWorkflowOne.Do(
                new DummyActivity(executionFunc: () =>
                {
                    firstActivityCompleted.Set();
                    return new ActivityExecutionResult().SetSuccessful();
                }, publishError: true,
                    publishEvent: true))
                .SetCompletionPercentage(100);

            dummyWorkflowTwo.Do(new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetFailed()))
                .SetCompletionPercentage(100)
                .SetRollbackActivity(
                    new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetSuccessful(),
                        isCancellable: false));

            dummyWorkflowThree.Do(new DummyActivity(executionFunc: () => new ActivityExecutionResult().SetSuccessful()))
                .SetCompletionPercentage(100);

            workflowEngine
                .Queue(dummyWorkflowOne)
                .Queue(dummyWorkflowTwo)
                .Queue(dummyWorkflowThree);

            workflowEngine.OnStarted((sender, argument) =>
            {
                workflowEngineStarted.Set();
            });
            workflowEngine.OnStopped((sender, argument) =>
            {
                workflowEngineStopped.Set();
            });

            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.NotStarted);
            workflowEngine.RunAsync();
            //wait for workflow engine to start
            workflowEngineStarted.WaitOne();
            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.Running);
            firstActivityCompleted.WaitOne();
            workflowEngine.CancelAsync();
            //wait for workflow engine to stop
            workflowEngineStopped.WaitOne();
            Assert.IsTrue(workflowEngine.State == WorkflowEngineState.Stopped);
            Assert.IsTrue(workflowEngine.ExecutionResult.ResultKind == WorkflowEngineExecutionResultKind.Canceled);
            Assert.IsTrue(dummyWorkflowOne.State == WorkflowState.Stopped);
            Assert.IsTrue(dummyWorkflowTwo.State == WorkflowState.NotStarted);
            Assert.IsTrue(dummyWorkflowThree.State == WorkflowState.NotStarted);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.IsCancellableChangedTriggered);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.OnCancellationPendingTriggered);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.ActivityExecutionStartedCount == 1);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.ActivityExecutionCompletedCount == 1);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.WorkflowExecutionStartCount == 1);
            Assert.IsTrue(dummyWorkflowEngineEventMonitor.WorkflowExecutionCompletedCount == 1);
        }
    }
}
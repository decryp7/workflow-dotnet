using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleWorkflow.Events;

namespace SimpleWorkflow.Test
{
    [TestClass]
    public class WorkflowTest
    {
        private DummyWorkflow dummyWorkflow;
        private IWorkflowEngineEventAggregator workflowEngineEventAggregator;
        private WorkflowEngineContext workflowEngineContext;

        [TestInitialize]
        public void TestInitialize()
        {
            workflowEngineEventAggregator = new WorkflowEngineEventAggregator();
            workflowEngineContext = new WorkflowEngineContext(workflowEngineEventAggregator);
            dummyWorkflow = new DummyWorkflow(new DummyWorkflowContext());
            ((IRequireWorkflowEngineContext) dummyWorkflow).WorkflowEngineContext = workflowEngineContext;
        }

        [TestMethod]
        public void Constructor_NullArgument_ArgumentNullException()
        {
            object[] args = new object[1]
            {
                new DummyWorkflowContext()
            };

            int testArgIndex = 0;

            for (int argumentIndex = 0; argumentIndex < args.Length; argumentIndex++)
            {
                try
                {
                    object[] clonedArgs = args.Clone() as object[];
                    clonedArgs[argumentIndex] = null;
                    Activator.CreateInstance(typeof(DummyWorkflow), clonedArgs);
                }
                catch (TargetInvocationException ex)
                {
                    Assert.IsTrue(ex.InnerException is ArgumentNullException);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void GetExecutionResult_WithoutRunningWorkflow_InvalidOperationException()
        {
            Assert.IsTrue(dummyWorkflow.State == WorkflowState.NotStarted);
            WorkflowExecutionResult workflowExecutionResult = dummyWorkflow.ExecutionResult;
        }

        [TestMethod]
        public void ExecuteSuccessfulWorkflow_GetWorkflowExecutionResultSuccessful()
        {
            dummyWorkflow.Do(
                new DummyActivity(executionFunc: () => { return new ActivityExecutionResult().SetSuccessful(); }));

            Assert.IsTrue(dummyWorkflow.State == WorkflowState.NotStarted);
            WorkflowExecutionResult workflowExecutionResult = dummyWorkflow.Execute();
            Assert.IsTrue(workflowExecutionResult.ResultKind == WorkflowExecutionResultKind.Successful);
            Assert.IsTrue(dummyWorkflow.ExecutionResult.ResultKind == WorkflowExecutionResultKind.Successful);
            Assert.IsTrue(dummyWorkflow.State == WorkflowState.Stopped);
        }

        [TestMethod]
        public void ExecuteWorkflow_ActivityFailed_GetWorkflowExecutionResultFailed()
        {
            dummyWorkflow.Do(
                new DummyActivity(executionFunc: () => { return new ActivityExecutionResult().SetFailed(); }));

            WorkflowExecutionResult workflowExecutionResult = dummyWorkflow.Execute();
            Assert.IsTrue(workflowExecutionResult.ResultKind == WorkflowExecutionResultKind.Failed);
        }

        [TestMethod]
        public void ExecuteWorkflow_OneActivityFailed_GetWorkflowExecutionResultFailed()
        {
            bool dummyActivityOneExecuted = false;
            bool dummyActivityTwoExecuted = false;

            DummyActivity dummyActivityOne = new DummyActivity(executionFunc: () =>
            {
                dummyActivityOneExecuted = true;
                return new ActivityExecutionResult().SetFailed();
            });

            DummyActivity dummyActivityTwo = new DummyActivity(executionFunc: () =>
            {
                dummyActivityTwoExecuted = true;
                return new ActivityExecutionResult().SetSuccessful();
            });

            dummyWorkflow
                .Do(dummyActivityOne)
                .Do(dummyActivityTwo);

            WorkflowExecutionResult workflowExecutionResult = dummyWorkflow.Execute();
            Assert.IsTrue(workflowExecutionResult.ResultKind == WorkflowExecutionResultKind.Failed);
            Assert.IsTrue(dummyActivityOneExecuted);
            //DummyActivityTwo should not be executed
            Assert.IsFalse(dummyActivityTwoExecuted);
        }

        [TestMethod]
        public void ExecuteWorkflow_OneActivityFailed_IgnoreFailure_GetWorkflowExecutionResultSucessful()
        {
            bool dummyActivityOneExecuted = false;
            bool dummyActivityTwoExecuted = false;

            DummyActivity dummyActivityOne = new DummyActivity(executionFunc: () =>
            {
                dummyActivityOneExecuted = true;
                return new ActivityExecutionResult().SetFailed();
            });

            DummyActivity dummyActivityTwo = new DummyActivity(executionFunc: () =>
            {
                dummyActivityTwoExecuted = true;
                return new ActivityExecutionResult().SetSuccessful();
            });

            dummyWorkflow
                .Do(dummyActivityOne)
                    .IgnoreExecutionFailure()
                .Do(dummyActivityTwo);

            WorkflowExecutionResult workflowExecutionResult = dummyWorkflow.Execute();
            Assert.IsTrue(workflowExecutionResult.ResultKind == WorkflowExecutionResultKind.Successful);
            Assert.IsTrue(dummyActivityOneExecuted);
            //DummyActivityOne failure is ignored so workflow continue to execute
            Assert.IsTrue(dummyActivityTwoExecuted);
        }

        [TestMethod]
        public void ExecuteWorkflow_OneActivityFailed_RollbackNotNecessary_GetWorkflowExecutionResultFailed()
        {
            bool dummyActivityOneExecuted = false;
            bool dummyActivityTwoExecuted = false;
            bool dummyActivityThreeExecuted = false;

            DummyActivity dummyActivityOne = new DummyActivity(executionFunc: () =>
            {
                dummyActivityOneExecuted = true;
                return new ActivityExecutionResult().SetFailed();
            });

            DummyActivity dummyActivityTwo = new DummyActivity(executionFunc: () =>
            {
                dummyActivityTwoExecuted = true;
                return new ActivityExecutionResult().SetSuccessful();
            });

            DummyActivity dummyActivityThree = new DummyActivity(executionFunc: () =>
            {
                dummyActivityThreeExecuted = true;
                return new ActivityExecutionResult().SetSuccessful();
            });

            dummyWorkflow
                .Do(dummyActivityOne)
                    .SetRollbackActivity(dummyActivityThree)
                .Do(dummyActivityTwo);

            WorkflowExecutionResult workflowExecutionResult = dummyWorkflow.Execute();
            Assert.IsTrue(workflowExecutionResult.ResultKind == WorkflowExecutionResultKind.Failed);
            Assert.IsTrue(dummyActivityOneExecuted);
            //DummyActivityTwo should not be executed
            Assert.IsFalse(dummyActivityTwoExecuted);
            //DummyActivityThree should not be executed because rollback is not necessary
            Assert.IsTrue(dummyActivityThreeExecuted);
        }

        [TestMethod]
        public void ExecuteWorkflow_OneActivityFailed_RollbackNecessary_GetWorkflowExecutionResultFailed()
        {
            bool dummyActivityOneExecuted = false;
            bool dummyActivityTwoExecuted = false;
            bool dummyActivityThreeExecuted = false;

            DummyActivity dummyActivityOne = new DummyActivity(executionFunc: () =>
            {
                dummyActivityOneExecuted = true;
                return new ActivityExecutionResult().SetSuccessful();
            });

            DummyActivity dummyActivityTwo = new DummyActivity(executionFunc: () =>
            {
                dummyActivityTwoExecuted = true;
                return new ActivityExecutionResult().SetFailed();
            });

            DummyActivity dummyActivityThree = new DummyActivity(executionFunc: () =>
            {
                dummyActivityThreeExecuted = true;
                return new ActivityExecutionResult().SetSuccessful();
            });

            dummyWorkflow
                .Do(dummyActivityOne)
                    .SetRollbackActivity(dummyActivityThree)
                .Do(dummyActivityTwo);

            WorkflowExecutionResult workflowExecutionResult = dummyWorkflow.Execute();
            Assert.IsTrue(workflowExecutionResult.ResultKind == WorkflowExecutionResultKind.Failed);
            Assert.IsTrue(dummyActivityOneExecuted);
            Assert.IsTrue(dummyActivityTwoExecuted);
            //DummyActivityThree should be executed because rollback is necessary
            Assert.IsTrue(dummyActivityThreeExecuted);
        }

        [TestMethod]
        public void ExecuteWorkflow_OneActivityFailed_RollbackthrowException_GetWorkflowExecutionResultFailed()
        {
            bool dummyActivityOneExecuted = false;
            bool dummyActivityTwoExecuted = false;
            bool dummyActivityThreeExecuted = false;

            DummyActivity dummyActivityOne = new DummyActivity(executionFunc: () =>
            {
                dummyActivityOneExecuted = true;
                return new ActivityExecutionResult().SetFailed();
            });

            DummyActivity dummyActivityTwo = new DummyActivity(executionFunc: () =>
            {
                dummyActivityTwoExecuted = true;
                return new ActivityExecutionResult().SetSuccessful();
            });

            DummyActivity dummyActivityThree = new DummyActivity(executionFunc: () =>
            {
                dummyActivityThreeExecuted = true;
                throw new IOException();
            });

            dummyWorkflow
                .Do(dummyActivityOne)
                    .SetRollbackActivity(dummyActivityThree)
                .Do(dummyActivityTwo);

            WorkflowExecutionResult workflowExecutionResult = dummyWorkflow.Execute();
            Assert.IsTrue(workflowExecutionResult.ResultKind == WorkflowExecutionResultKind.Failed);
            Assert.IsTrue(dummyActivityOneExecuted);
            //DummyActivityTwo should not be executed
            Assert.IsFalse(dummyActivityTwoExecuted);
            //DummyActivityThree should be executed because rollback is necessary
            Assert.IsTrue(dummyActivityThreeExecuted);
        }

        [TestMethod]
        public void ExecuteWorkflow_ResetWorkflow_WorkflowStateNotStarted()
        {
            bool dummyActivityOneExecuted = false;
            bool dummyActivityTwoExecuted = false;

            DummyActivity dummyActivityOne = new DummyActivity(executionFunc: () =>
            {
                dummyActivityOneExecuted = true;
                return new ActivityExecutionResult().SetSuccessful();
            });

            DummyActivity dummyActivityTwo = new DummyActivity(executionFunc: () =>
            {
                dummyActivityTwoExecuted = true;
                return new ActivityExecutionResult().SetSuccessful();
            });

            dummyWorkflow
                .Do(dummyActivityOne)
                .Do(dummyActivityTwo);

            WorkflowExecutionResult workflowExecutionResult = dummyWorkflow.Execute();
            Assert.IsTrue(workflowExecutionResult.ResultKind == WorkflowExecutionResultKind.Successful);
            Assert.IsTrue(dummyWorkflow.State == WorkflowState.Stopped);
            Assert.IsTrue(dummyActivityOneExecuted);
            Assert.IsTrue(dummyActivityOne.State == ActivityState.Stopped);
            Assert.IsTrue(dummyActivityTwoExecuted);
            Assert.IsTrue(dummyActivityTwo.State == ActivityState.Stopped);

            dummyWorkflow.Reset();
            Assert.IsTrue(dummyWorkflow.State == WorkflowState.NotStarted);
            Assert.IsTrue(dummyActivityOne.State == ActivityState.NotStarted);
            Assert.IsTrue(dummyActivityTwo.State == ActivityState.NotStarted);
        }

        [TestMethod]
        public void ExecuteWorkflow_ActivityWithCompletionPercentage_GetWorkflowExecutionResultSuccessful()
        {
            bool dummyActivityOneExecuted = false;
            bool dummyActivityTwoExecuted = false;
            IList<double> completionPercentages = new List<double>();

            DummyActivity dummyActivityOne = new DummyActivity(executionFunc: () =>
            {
                dummyActivityOneExecuted = true;
                return new ActivityExecutionResult().SetSuccessful();
            });

            DummyActivity dummyActivityTwo = new DummyActivity(executionFunc: () =>
            {
                dummyActivityTwoExecuted = true;
                return new ActivityExecutionResult().SetSuccessful();
            });

            dummyWorkflow
                .Do(dummyActivityOne)
                    .SetCompletionPercentage(10)
                .Do(dummyActivityTwo)
                    .SetCompletionPercentage(100);

            workflowEngineEventAggregator.GetMessage<WorkflowProgressChanged>().Subscribe(info =>
            {
                completionPercentages.Add(info.CompletionPercentage);
            });

            WorkflowExecutionResult workflowExecutionResult = dummyWorkflow.Execute();
            Assert.IsTrue(workflowExecutionResult.ResultKind == WorkflowExecutionResultKind.Successful);
            Assert.IsTrue(dummyWorkflow.State == WorkflowState.Stopped);
            Assert.IsTrue(dummyActivityOneExecuted);
            Assert.IsTrue(dummyActivityOne.State == ActivityState.Stopped);
            Assert.IsTrue(dummyActivityTwoExecuted);
            Assert.IsTrue(dummyActivityTwo.State == ActivityState.Stopped);
            Assert.IsTrue(completionPercentages[0] == 10);
            Assert.IsTrue(completionPercentages[1] == 100);
        }

        [TestMethod]
        public void ExecuteWorkflow_ActivityCancelled_GetWorkflowExecutionResultCancelled()
        {
            bool dummyActivityOneExecuted = false;
            bool dummyActivityTwoExecuted = false;
            IList<double> completionPercentages = new List<double>();

            DummyActivity dummyActivityOne = new DummyActivity(executionFunc: () =>
            {
                dummyActivityOneExecuted = true;
                return new ActivityExecutionResult().SetCanceled();
            });

            DummyActivity dummyActivityTwo = new DummyActivity(executionFunc: () =>
            {
                dummyActivityTwoExecuted = true;
                return new ActivityExecutionResult().SetSuccessful();
            });

            dummyWorkflow
                .Do(dummyActivityOne)
                    .SetCompletionPercentage(10)
                .Do(dummyActivityTwo)
                    .SetCompletionPercentage(100);

            workflowEngineEventAggregator.GetMessage<WorkflowProgressChanged>().Subscribe(info =>
            {
                completionPercentages.Add(info.CompletionPercentage);
            });

            WorkflowExecutionResult workflowExecutionResult = dummyWorkflow.Execute();
            Assert.IsTrue(workflowExecutionResult.ResultKind == WorkflowExecutionResultKind.Canceled);
            Assert.IsTrue(dummyWorkflow.State == WorkflowState.Stopped);
            Assert.IsTrue(dummyActivityOneExecuted);
            Assert.IsTrue(dummyActivityOne.State == ActivityState.Stopped);
            //Since DummyActivityOne is cancelled, workflow does not continue to execute
            Assert.IsFalse(dummyActivityTwoExecuted);
            Assert.IsTrue(dummyActivityTwo.State == ActivityState.NotStarted);
            //Progress changed should not be fired.
            Assert.IsTrue(completionPercentages.Count == 0);
        }


        [TestMethod]
        public void ExecuteWorkflow_ActivityThrowException_GetWorkflowExecutionResultFailed()
        {
            bool dummyActivityOneExecuted = false;
            bool dummyActivityTwoExecuted = false;
            IList<double> completionPercentages = new List<double>();

            DummyActivity dummyActivityOne = new DummyActivity(executionFunc: () =>
            {
                dummyActivityOneExecuted = true;
                throw new IOException();
            });

            DummyActivity dummyActivityTwo = new DummyActivity(executionFunc: () =>
            {
                dummyActivityTwoExecuted = true;
                return new ActivityExecutionResult().SetSuccessful();
            });

            dummyWorkflow
                .Do(dummyActivityOne)
                    .SetCompletionPercentage(10)
                .Do(dummyActivityTwo)
                    .SetCompletionPercentage(100);

            workflowEngineEventAggregator.GetMessage<WorkflowProgressChanged>().Subscribe(info =>
            {
                completionPercentages.Add(info.CompletionPercentage);
            });

            WorkflowExecutionResult workflowExecutionResult = dummyWorkflow.Execute();
            Assert.IsTrue(workflowExecutionResult.ResultKind == WorkflowExecutionResultKind.Failed);
            Assert.IsTrue(dummyWorkflow.State == WorkflowState.Stopped);
            Assert.IsTrue(dummyActivityOneExecuted);
            Assert.IsTrue(dummyActivityOne.State == ActivityState.Stopped);
            //Since DummyActivityOne failed because of exception, workflow does not continue to execute
            Assert.IsFalse(dummyActivityTwoExecuted);
            Assert.IsTrue(dummyActivityTwo.State == ActivityState.NotStarted);
            //Progress changed should not be fired.
            Assert.IsTrue(completionPercentages.Count == 0);
        }
    }
}
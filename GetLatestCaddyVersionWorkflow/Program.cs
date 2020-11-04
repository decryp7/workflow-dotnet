using System;
using GetLatestCaddyVersionWorkflow.Activities;
using SimpleWorkflow;

namespace GetLatestCaddyVersionWorkflow
{
    class Program
    {
        static void Main(string[] args)
        {
            IWorkflow getLatestCaddyVersion = new GetLatestCaddyVersion(new GetLatestCaddyVersionContext());

            getLatestCaddyVersion
                .Do(new ReadCaddyRSSFeed())
                .Do(new GetLatestVersion())
                .Do(new PrintLatestCaddyVersion());

            IWorkflowEngine workflowEngine = new WorkflowEngine();
            workflowEngine.Queue(getLatestCaddyVersion);
            WorkflowEngineExecutionResult executionResult = workflowEngine.Run();

            Console.WriteLine(
                FormattableString.Invariant($"Workflow engine execution result: {executionResult.ResultKind}"));
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}

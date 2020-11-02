using SimpleWorkflow.Events.Payload;

namespace SimpleWorkflow.Events
{
    /// <summary>
    /// Workflow rollback started event
    /// </summary>
    public class WorkflowRollbackStarted : WorkflowEngineEvent<WorkflowInfo>
    {
    }
}
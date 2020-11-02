using SimpleWorkflow.Events.Payload;

namespace SimpleWorkflow.Events
{
    /// <summary>
    /// Workflow started event
    /// </summary>
    public class WorkflowStarted : WorkflowEngineEvent<WorkflowInfo>
    {
    }
}
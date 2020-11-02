using SimpleWorkflow.Events.Payload;

namespace SimpleWorkflow.Events
{
    /// <summary>
    /// Workflow progress changed event
    /// </summary>
    public class WorkflowProgressChanged : WorkflowEngineEvent<WorkflowProgressInfo>
    {
    }
}
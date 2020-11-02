using SimpleWorkflow.Events.Payload;

namespace SimpleWorkflow.Events
{
    /// <summary>
    /// Workflow completed event
    /// </summary>
    public class WorkflowCompleted : WorkflowEngineEvent<WorkflowCompletedInfo>
    {
    }
}
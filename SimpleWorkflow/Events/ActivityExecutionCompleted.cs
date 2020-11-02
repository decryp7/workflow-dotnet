using SimpleWorkflow.Events.Payload;

namespace SimpleWorkflow.Events
{
    /// <summary>
    /// Activity execution completed event
    /// </summary>
    public class ActivityExecutionCompleted : WorkflowEngineEvent<ActivityExecutionCompletedInfo>
    {
    }
}
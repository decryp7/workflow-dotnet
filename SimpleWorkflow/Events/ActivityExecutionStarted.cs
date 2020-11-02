using SimpleWorkflow.Events.Payload;

namespace SimpleWorkflow.Events
{
    /// <summary>
    /// Activity execution started event
    /// </summary>
    public class ActivityExecutionStarted : WorkflowEngineEvent<ActivityExecutionStartedInfo>
    {
    }
}
using SimpleWorkflow.Events.Payload;

namespace SimpleWorkflow.Events
{
    /// <summary>
    /// Workflow engine error occured event
    /// </summary>
    public class ErrorOccured : WorkflowEngineEvent<ErrorMessage>
    {
    }
}
namespace SimpleWorkflow
{
    /// <summary>
    /// WorkflowEngine state
    /// </summary>
    public enum WorkflowEngineState
    {
        //WorkflowEngine was not started
        NotStarted,
        //WorkflowEngine is running
        Running,
        //WorkflowEngine started and stopped
        Stopped
    }
}
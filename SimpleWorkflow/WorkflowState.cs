namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow State
    /// </summary>
    public enum WorkflowState
    {
        //Workflow was not executed
        NotStarted,
        //Workflow is running
        Running,
        //Workflow was executed and stopped
        Stopped
    }
}
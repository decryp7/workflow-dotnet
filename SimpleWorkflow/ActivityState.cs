namespace SimpleWorkflow
{
    /// <summary>
    /// Activity State
    /// </summary>
    public enum ActivityState
    {
        //Activity was not executed
        NotStarted,
        //Activity is running
        Running,
        //Activity was executed and stopped
        Stopped
    }
}
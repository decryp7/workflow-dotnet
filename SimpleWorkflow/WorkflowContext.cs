namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow context
    /// </summary>
    public abstract class WorkflowContext
    {
        /// <summary>
        /// Get the cache
        /// </summary>
        public IRuntimeCache Cache { get; private set; }

        protected WorkflowContext()
        {
            Cache = new RuntimeCache();
        }
    }
}
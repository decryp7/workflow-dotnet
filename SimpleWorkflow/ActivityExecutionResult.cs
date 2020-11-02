namespace SimpleWorkflow
{
    /// <summary>
    /// Activity execution result
    /// </summary>
    public class ActivityExecutionResult
    {
        /// <summary>
        /// Get the kind of the activity execution result
        /// </summary>
        public ActivityExecutionResultKind ResultKind { get; private set; }

        /// <summary>
        /// Set the activity execution result to failed.
        /// </summary>
        /// <returns>Activity execution result</returns>
        public ActivityExecutionResult SetFailed()
        {
            ResultKind = ActivityExecutionResultKind.Failed;
            return this;
        }

        /// <summary>
        /// Set the activity execution result to successful.
        /// </summary>
        /// <returns>Activity execution result</returns>
        public ActivityExecutionResult SetSuccessful()
        {
            ResultKind = ActivityExecutionResultKind.Successful;
            return this;
        }

        /// <summary>
        /// Set the activity execution result to canceled.
        /// </summary>
        /// <returns>Activity execution result</returns>
        public ActivityExecutionResult SetCanceled()
        {
            ResultKind = ActivityExecutionResultKind.Canceled;
            return this;
        }
    }
}
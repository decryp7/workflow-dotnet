using SimpleWorkflow.SanityCheck;

namespace SimpleWorkflow.Events.Payload
{
    /// <summary>
    /// Error Information
    /// </summary>
    public class ErrorMessage : Message
    {
        /// <summary>
        /// Get the error message
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// Error Information
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="error">Error</param>
        public ErrorMessage(object source, string error) : base(source)
        {
            Guard.Ensure(error, nameof(error)).IsNotNullOrEmpty();
            Error = error;
        }
    }
}
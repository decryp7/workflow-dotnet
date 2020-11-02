namespace SimpleWorkflow.Events.Payload
{
    /// <summary>
    /// Source Information
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// Get the source
        /// </summary>
        public object Source { get; private set; }

        /// <summary>
        /// Source information
        /// </summary>
        /// <param name="source"></param>
        protected Message(object source)
        {
            //source can be null
            Source = source;
        }
    }
}
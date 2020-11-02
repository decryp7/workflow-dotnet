namespace SimpleWorkflow
{
    /// <summary>
    /// Workflow run time cache
    /// </summary>
    public interface IRuntimeCache : IReadOnlyRuntimeCache
    {
        /// <summary>
        /// Add the target object to the cache
        /// </summary>
        /// <typeparam name="TObject">Target object Type</typeparam>
        /// <param name="id">Target object unique identifier</param>
        /// <param name="target">Target object</param>
        void Set<TObject>(string id, TObject target);

        /// <summary>
        /// Copy the contents of the cache to the target runtime cache
        /// </summary>
        /// <param name="readOnlyRuntimeCache">Target runtime cache</param>
        void CopyTo(IReadOnlyRuntimeCache readOnlyRuntimeCache);

        /// <summary>
        /// Clear the cache
        /// </summary>
        void Clear();
    }
}
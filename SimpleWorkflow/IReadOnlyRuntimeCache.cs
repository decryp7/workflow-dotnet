namespace SimpleWorkflow
{
    /// <summary>
    /// Readonly interface of runtime cache
    /// </summary>
    public interface IReadOnlyRuntimeCache
    {
        /// <summary>
        /// Read the object from the cache using its unique identifier
        /// </summary>
        /// <typeparam name="TObject">Target object type</typeparam>
        /// <param name="id">Target object unique identifier</param>
        /// <returns>Target object</returns>
        TObject Read<TObject>(string id);
    }
}
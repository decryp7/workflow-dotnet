using System.Collections.Generic;
using System.Collections.ObjectModel;
using SimpleWorkflow.SanityCheck;

namespace SimpleWorkflow
{
    /// <summary>
    /// Runtime cache
    /// </summary>
    public class RuntimeCache : IRuntimeCache
    {
        private IDictionary<string, object> cache;

        public IReadOnlyDictionary<string, object> Cache { get; private set; }

        public RuntimeCache()
        {
            cache = new Dictionary<string, object>();
            Cache = new ReadOnlyDictionary<string, object>(cache);
        }

        #region Implementation of IRuntimeCache

        /// <summary>
        /// Add the target object to the cache
        /// </summary>
        /// <typeparam name="TObject">Target object Type</typeparam>
        /// <param name="id">Target object unique identifier</param>
        /// <param name="target">Target object</param>
        public void Set<TObject>(string id, TObject target)
        {
            Guard.Ensure(id, "id").IsNotNullOrEmpty();
            Guard.Ensure(target, "target").IsNotNull();

            if (cache.ContainsKey(id))
            {
                cache[id] = target;
            }
            else
            {
                cache.Add(id, target);
            }
        }

        /// <summary>
        /// Read the object from the cache using its unique identifier
        /// </summary>
        /// <typeparam name="TObject">Target object type</typeparam>
        /// <param name="id">Target object unique identifier</param>
        /// <returns>Target object</returns>
        public TObject Read<TObject>(string id)
        {
            Guard.Ensure(id, "id").IsNotNullOrEmpty();

            if (!cache.TryGetValue(id, out var target) || !(target is TObject))
            {
                return default(TObject);
            }

            return (TObject)target;
        }

        /// <summary>
        /// Copy the contents of the cache to the target runtime cache
        /// </summary>
        /// <param name="readOnlyRuntimeCache">Target runtime cache</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void CopyTo(IReadOnlyRuntimeCache readOnlyRuntimeCache)
        {
            Guard.Ensure(readOnlyRuntimeCache, "runtimeCache")
                .IsNotNull()
                .MeetThisCondition(targetCache => !ReferenceEquals(this, readOnlyRuntimeCache),
                    "Same instance cannot be used for copying.")
                .MeetThisCondition(targetCache => targetCache is IRuntimeCache,
                    "The cache is readonly. (Does not implement IRuntimeCache).");

            IRuntimeCache runtimeCache = readOnlyRuntimeCache as IRuntimeCache;

            if (runtimeCache != null)
            {
                foreach (KeyValuePair<string, object> keyValuePair in cache)
                {
                    runtimeCache.Set(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }

        /// <summary>
        /// Clear the cache
        /// </summary>
        public void Clear()
        {
            cache.Clear();
        }

        #endregion
    }
}
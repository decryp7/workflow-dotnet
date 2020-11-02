using System;

namespace SimpleWorkflow.SanityCheck
{
    /// <summary>
    /// Ensure class
    /// Part of the sanity check framework
    /// </summary>
    /// <typeparam name="TTarget">Target</typeparam>
    public class Ensure<TTarget>
    {
        /// <summary>
        /// The target object
        /// </summary>
        public TTarget Target { get; }

        /// <summary>
        /// Name of target object
        /// </summary>
        public string TargetName { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">Target object</param>
        /// <param name="targetName">Target name</param>
        /// <exception cref="ArgumentNullException"><paramref name="targetName"/> is <see langword="null" />.</exception>
        public Ensure(TTarget target, string targetName)
        {
            if (string.IsNullOrEmpty(targetName))
            {
                throw new ArgumentNullException(nameof(targetName));
            }

            this.Target = target;
            this.TargetName = targetName;
        }
    }
}
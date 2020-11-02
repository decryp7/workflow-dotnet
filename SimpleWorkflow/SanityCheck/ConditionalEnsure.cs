using System;

namespace SimpleWorkflow.SanityCheck
{
    /// <summary>
    /// Condition ensure class.
    /// Part of the sanity check framework
    /// </summary>
    public class ConditionalEnsure
    {
        /// <summary>
        /// Return true if condition is met, otherwise false.
        /// </summary>
        public bool Passed { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="condition">Condition to check</param>
        public ConditionalEnsure(Func<bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            Passed = condition.Invoke();
        }
    }
}
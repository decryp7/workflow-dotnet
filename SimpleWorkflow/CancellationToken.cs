using System;
using GuardLibrary;

namespace SimpleWorkflow
{
    /// <summary>
    /// Cancellation token
    /// </summary>
    public class CancellationToken
    {
        /// <summary>
        /// Checks if cancellation has been triggered.
        /// </summary>
        public bool CancellationPending { get; private set; }

        /// <summary>
        /// Set the cancellation token to signal that cancel has been triggered.
        /// </summary>
        internal void Cancel()
        {
            Guard.EnsureThisConditionIsMet(() => CancellationPending != true)
                .OrThrowException(new InvalidOperationException("Cancellation is already pending"));
            CancellationPending = true;
        }

        /// <summary>
        /// Reset the cancellation.
        /// </summary>
        internal void Reset()
        {
            CancellationPending = false;
        }
    }
}
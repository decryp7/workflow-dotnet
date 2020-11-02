using System;

namespace SimpleWorkflow.SanityCheck
{
    /// <summary>
    /// Static class to expose the sanity check framework.
    /// Part of the sanity check framework
    /// Guard.Ensure(targetObject, "targetObjectName").IsNotNull();
    /// Guard.Ensure(targetString, "targetStringName").IsNotNullOrEmpty();
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Ensure the target.
        /// On its own it does nothing. 
        /// It is supposed to be chained with checks in EnsureExtensions.cs
        /// </summary>
        /// <typeparam name="TTarget">Target object type</typeparam>
        /// <param name="target">Target object</param>
        /// <param name="targetName">Target object name</param>
        /// <returns>The ensure object wrapping the target object</returns>
        public static Ensure<TTarget> Ensure<TTarget>(TTarget target, string targetName)
        {
            return new Ensure<TTarget>(target, targetName);
        }

        /// <summary>
        /// Ensure the target woth condition.
        /// On its own it does nothing. 
        /// It is supposed to be chained with checks in EnsureExtensions.cs
        /// </summary>
        /// <param name="condition">The condition to be met</param>
        /// <returns>The conditional ensure object wrapping the target object.</returns>
        public static ConditionalEnsure EnsureThisConditionIsMet(Func<bool> condition)
        {
            return new ConditionalEnsure(condition);
        }
    }
}
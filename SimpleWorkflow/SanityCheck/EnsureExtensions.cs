using System;
using System.Globalization;
using System.IO;

namespace SimpleWorkflow.SanityCheck
{
    /// <summary>
    /// Ensure extension methods to include more checking mechanism
    /// Part of the sanity check framework
    /// </summary>
    public static class EnsureExtensions
    {
        /// <summary>
        /// Ensure that the target is not null or throw an ArgumentNullException.
        /// </summary>
        /// <typeparam name="TTarget">The target object type</typeparam>
        /// <param name="ensure">The target object to ensure that it is not null</param>
        /// <returns>The target object</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ensure.TargetName"/> is <see langword="null" />.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static Ensure<TTarget> IsNotNull<TTarget>(this Ensure<TTarget> ensure)
        {
            if (ensure.Target == null)
            {
                throw new ArgumentNullException(ensure.TargetName);
            }

            return ensure;
        }

        /// <summary>
        /// Ensure that the target meet the specified condition.
        /// </summary>
        /// <typeparam name="TTarget">The target object type.</typeparam>
        /// <param name="ensure">The target object to ensure that it is not null.</param>
        /// <param name="condition">The condition to be met.</param>
        /// <param name="errorMessage">The error message that will be thrown as InvalidOperationException if condition is note met.</param>
        /// <returns>The target object</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ensure.TargetName"/> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the condition is not met.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static Ensure<TTarget> MeetThisCondition<TTarget>(this Ensure<TTarget> ensure,
            Predicate<TTarget> condition, string errorMessage)
        {
            if (ensure.Target == null)
            {
                throw new ArgumentNullException(ensure.TargetName);
            }

            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            if (!condition.Invoke(ensure.Target))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return ensure;
        }

        /// <summary>
        /// Ensure the target(string type) is not null or empty.
        /// </summary>
        /// <param name="ensure">The target object to ensure that it is not null.</param>
        /// <returns>The target object</returns>
        /// <exception cref="ArgumentException">Thrown if the string is null or empty.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static Ensure<string> IsNotNullOrEmpty(this Ensure<string> ensure)
        {
            if (string.IsNullOrEmpty(ensure.Target))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "The parameter {0} cannot be null or empty.", ensure.TargetName));
            }

            return ensure;
        }

        /// <summary>
        /// Ensure the target(string type) is a valid Guid string
        /// </summary>
        /// <param name="ensure">The target object to ensure that it a valid Guid string.</param>
        /// <returns>The target object</returns>
        /// <exception cref="ArgumentException">Thrown if the string is not a valid Guid string.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static Ensure<string> IsValidGuid(this Ensure<string> ensure)
        {
            Guid id = Guid.Empty;
            if (!Guid.TryParse(ensure.Target, out id))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "The parameter {0} is not a valid guid string", ensure.TargetName));
            }

            return ensure;
        }

        /// <summary>
        /// Ensure the target(Guid) is not empty.
        /// </summary>
        /// <param name="ensure">The target object to ensure that it is not null.</param>
        /// <returns>The target object</returns>
        /// <exception cref="ArgumentException">Thrown if the string is null or empty.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static Ensure<Guid> IsNotEmpty(this Ensure<Guid> ensure)
        {
            if (ensure.Target == Guid.Empty)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "The parameter {0} cannot be empty.", ensure.TargetName));
            }

            return ensure;
        }

        /// <summary>
        /// Ensure the target(string type) file path exist.
        /// </summary>
        /// <param name="ensure">The target object to ensure that it is not null.</param>
        /// <returns>The target object</returns>
        /// <exception cref="ArgumentException">Thrown if the file path does not exist.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static Ensure<string> FilePathExist(this Ensure<string> ensure)
        {
            if (!File.Exists(ensure.Target))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "The file path {0} does not exist.", ensure.Target),
                    ensure.TargetName);
            }

            return ensure;
        }

        /// <summary>
        /// Ensure the target(string type) directory path exist.
        /// </summary>
        /// <param name="ensure">The target object to ensure that it is not null.</param>
        /// <returns>The target object</returns>
        /// <exception cref="ArgumentException">Thrown if the directory path does not exist.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static Ensure<string> DirectoryPathExist(this Ensure<string> ensure)
        {
            if (!Directory.Exists(ensure.Target))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "The directory path {0} does not exist.",
                        ensure.Target),
                    ensure.TargetName);
            }

            return ensure;
        }

        /// <summary>
        /// Ensure the target(int type) is more then zero
        /// </summary>
        /// <param name="ensure">The target object to ensure that it is not null.</param>
        /// <returns>The target object</returns>
        /// <exception cref="ArgumentException">Thrown if the int value is less then zero.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static Ensure<int> IsMoreThenZero(this Ensure<int> ensure)
        {
            if (!(ensure.Target > 0))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "The parameter {0} must be more then 0.", ensure.TargetName));
            }

            return ensure;
        }

        /// <summary>
        /// Used together with ConditionEnsure.
        /// Throw the specified exception if condition is not met.
        /// </summary>
        /// <param name="conditionalEnsure">The condition ensure.</param>
        /// <param name="exception">The specified exception to be thrown</param>
        /// <exception cref="Exception">Thrown if the condition is not met.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
            "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static void OrThrowException(this ConditionalEnsure conditionalEnsure, Exception exception)
        {
            if (!conditionalEnsure.Passed)
            {
                throw exception;
            }
        }
    }
}
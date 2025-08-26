using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Shared.Extensions
{
    /// <summary>
    /// Provides extension methods to conditionally run actions or asynchronous functions
    /// on a background thread using <see cref="Task.Run"/>.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Runs the specified asynchronous function on a background thread if the condition is true.
        /// </summary>
        /// <param name="taskFunc">The asynchronous function to run. Can be null.</param>
        /// <param name="condition">A boolean indicating whether to run the function.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation if run; otherwise <c>null</c> 
        /// if the delegate is null or the condition is false.
        /// </returns>
        public static Task RunIf(this Func<Task> taskFunc, bool condition)
        {
            if (taskFunc == null)
                return null;
            return condition? Task.Run(taskFunc) : null;
        }

        /// <summary>
        /// Runs the specified synchronous action on a background thread if the condition is true.
        /// </summary>
        /// <param name="action">The action to run. Can be null.</param>
        /// <param name="condition">A boolean indicating whether to run the action.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation if run; otherwise <c>null</c> 
        /// if the delegate is null or the condition is false.
        /// </returns>
        public static Task RunIf(this Action action, bool condition)
        {
            if (action == null)
                return null;
            return condition ? Task.Run(action) : null;
        }
    }
}

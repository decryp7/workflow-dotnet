using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SimpleWorkflow.Test")]
namespace SimpleWorkflow
{
    /// <summary>
    /// Implement this interface for workflow engine context to be injected.
    /// </summary>
    internal interface IRequireWorkflowEngineContext
    {
        /// <summary>
        /// The workflow engine context that is be injected.
        /// </summary>
        WorkflowEngineContext WorkflowEngineContext { set; }
    }
}
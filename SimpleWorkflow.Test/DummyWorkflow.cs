namespace SimpleWorkflow.Test
{
    public class DummyWorkflow : Workflow<DummyWorkflowContext>
    {
        public DummyWorkflow(DummyWorkflowContext workflowContext) : base(workflowContext)
        {
        }
    }
}
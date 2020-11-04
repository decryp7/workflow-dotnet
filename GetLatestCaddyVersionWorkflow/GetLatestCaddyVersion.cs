using SimpleWorkflow;

namespace GetLatestCaddyVersionWorkflow
{
    public class GetLatestCaddyVersion : Workflow<GetLatestCaddyVersionContext>
    {
        public GetLatestCaddyVersion(GetLatestCaddyVersionContext workflowContext) : 
            base(workflowContext)
        {
        }
    }
}
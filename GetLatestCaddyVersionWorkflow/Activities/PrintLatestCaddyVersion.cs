using System;
using System.Linq;
using System.Xml.Linq;
using SimpleWorkflow;

namespace GetLatestCaddyVersionWorkflow.Activities
{
    public class PrintLatestCaddyVersion : Activity<GetLatestCaddyVersionContext>
    {
        public const string VersionCacheKey = nameof(VersionCacheKey);

        public override string Description => "Print latest version";

        public override bool IsCancellable => true;

        protected override ActivityExecutionResult ExecuteImpl()
        {
            try
            {
                string version = WorkflowContext.Cache.Read<string>(GetLatestVersion.VersionCacheKey);
                
                Console.WriteLine(FormattableString.Invariant($"Latest Caddy Version: {version}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(FormattableString.Invariant($"Exception occurred during reading version from RSS feed. {ex}"));
                return new ActivityExecutionResult().SetFailed();
            }

            return new ActivityExecutionResult().SetSuccessful();
        }
    }
}
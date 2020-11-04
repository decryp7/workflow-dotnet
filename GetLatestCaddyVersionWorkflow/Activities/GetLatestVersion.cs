using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using SimpleWorkflow;

namespace GetLatestCaddyVersionWorkflow.Activities
{
    public class GetLatestVersion : Activity<GetLatestCaddyVersionContext>
    {
        public const string VersionCacheKey = nameof(VersionCacheKey);

        public override string Description => "Get latest version from RSS feed";

        public override bool IsCancellable => true;

        protected override ActivityExecutionResult ExecuteImpl()
        {
            try
            {
                XNamespace ns = "http://www.w3.org/2005/Atom";
                XElement rssFeed = WorkflowContext.Cache.Read<XElement>(ReadCaddyRSSFeed.RSSCacheKey);
                XElement entry = rssFeed.Elements(ns + "entry")
                    .First();

                WorkflowContext.Cache.Set(VersionCacheKey, entry.Element(ns + "title").Value);
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
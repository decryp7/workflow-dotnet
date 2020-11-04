using System;
using System.IO;
using System.Net.Http;
using System.Xml.Linq;
using SimpleWorkflow;

namespace GetLatestCaddyVersionWorkflow.Activities
{
    public class ReadCaddyRSSFeed : Activity<WorkflowContext>
    {
        public const string RSSCacheKey = nameof(RSSCacheKey);

        private const string url = "https://github.com/caddyserver/caddy/releases.atom";

        public override string Description => FormattableString.Invariant($"Read Caddy RSS feed from url: {url}.");

        public override bool IsCancellable => true;

        protected override ActivityExecutionResult ExecuteImpl()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage result = httpClient.GetAsync(url)
                    .Result;

                Stream stream = result.Content.ReadAsStreamAsync().Result;

                XElement itemXml = XElement.Load(stream);

                WorkflowContext.Cache.Set(RSSCacheKey, itemXml);
            }
            catch (Exception ex)
            {
                Console.WriteLine(FormattableString.Invariant($"Exception occurred during reading RSS feed. {ex}"));
                return new ActivityExecutionResult().SetFailed();
            }

            return new ActivityExecutionResult().SetSuccessful();
        }
    }
}
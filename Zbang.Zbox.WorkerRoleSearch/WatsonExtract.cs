using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlchemyAPIClient;
using AlchemyAPIClient.Requests;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class WatsonExtract : IWatsonExtract
    {
        readonly AlchemyClient m_Client =
              new AlchemyClient("e05317b0a67d8a3d0bf82f5b0e0b58012b717779");

        public async Task<IEnumerable<string>> GetConceptAsync(string text, CancellationToken token)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            var request = new AlchemyTextConceptsRequest(text, m_Client)
            {
                KnowledgeGraph = true,
                LinkedData = true,
                ShowSourceText = true,
                MaxRetrieve = 30
            };
            try
            {
                var result = await request.GetResponse(token);
                return result.Concepts.Where(w => w.Relevance > 0.75).Select(s => s.Text);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("watson concept error" + text, ex);
                return null;
            }
        }

    }

    public interface IWatsonExtract
    {
        Task<IEnumerable<string>> GetConceptAsync(string text, CancellationToken token);
    }
}

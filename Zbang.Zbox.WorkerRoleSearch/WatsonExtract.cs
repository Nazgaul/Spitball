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
              new AlchemyClient("785ea0b610cc18cf9cb3815552d2bbd979133a5b");

        public async Task<IEnumerable<string>> GetConceptAsync(string text, CancellationToken token)
        {
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
                return new List<string>();
            }
        }

    }

    public interface IWatsonExtract
    {
        Task<IEnumerable<string>> GetConceptAsync(string text, CancellationToken token);
    }
}

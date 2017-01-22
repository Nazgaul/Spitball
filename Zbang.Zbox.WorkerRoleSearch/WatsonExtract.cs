using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlchemyAPIClient;
using AlchemyAPIClient.Requests;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class WatsonExtract: IWatsonExtract
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
            var result = await request.GetResponse(token);
            return result.Concepts.Take(5).Select(s => s.Text);
        }
        
    }

    public interface IWatsonExtract
    {
        Task<IEnumerable<string>> GetConceptAsync(string text, CancellationToken token);
    }
}

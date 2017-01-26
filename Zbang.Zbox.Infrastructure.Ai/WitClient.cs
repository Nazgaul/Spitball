using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public class WitClient
    {
        private const string DefaultApiVersion = "20160516";
        private readonly string m_Token;
        private readonly RestClient m_Client;

        public WitClient(string token)
        {
            m_Token = token;
            m_Client = new RestClient("https://api.wit.ai");
            m_Client.AddHandler("*", new JsonDeserializer());
            
        }
        public async Task<BaseIntent> GetMessageAsync(string q, CancellationToken token)
        {
            var request = new RestRequest("message", Method.GET);
            request.AddQueryParameter("q", q);
            AddDefaultParametes(request);
            var json = await m_Client.ExecuteTaskAsync(request,token);
            var t = JsonConvert.DeserializeObject<BaseIntent>(json.Content);
            
            //var data = await m_Client.ExecuteTaskAsync<WitResponse>(request, token);
            //var data2 = await m_Client.ExecuteTaskAsync<BaseIntent>(request, token);
            return t;
        }

        private void AddDefaultParametes(RestRequest request)
        {
            request.AddQueryParameter("v", DefaultApiVersion);
            request.AddHeader("Authorization", "Bearer " + m_Token);
        }


        internal async Task AddEntityValueAsync(string entityName, string value, IEnumerable<string> expressions,
            string metadata, CancellationToken token)
        {
            var request = new RestRequest($"entities/{entityName}/values", Method.POST);
            AddDefaultParametes(request);
            request.RequestFormat = DataFormat.Json;
            var expressionList = expressions?.ToList() ?? new List<string>();
            expressionList.Add(value);
            request.AddBody(new
            {
                value,
                expressions = expressionList.Distinct(),
                metadata
            });
            var result = await m_Client.ExecuteTaskAsync(request, token);
            if (result.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                await UpdateEntityValueAsync(entityName, value, expressionList, token);
                //await RemoveEntityValueAsync(entityName, value, token);
                //await AddEntityValueAsync(entityName, value, expressionList, metadata, token);
            }
        }

        internal Task RemoveEntityValueAsync(string entityName, string value, CancellationToken token)
        {
            var request = new RestRequest($"entities/{entityName}/values/{value}", Method.DELETE);
            AddDefaultParametes(request);
            return m_Client.ExecuteTaskAsync(request, token);
        }

        internal Task UpdateEntityValueAsync(string entityName, string value, IEnumerable<string> expressions, CancellationToken token)
        {
            var tasks = new List<Task>();
            foreach (var expression in expressions.Distinct())
            {
                var request = new RestRequest($"entities/{entityName}/values/{value}/expressions", Method.POST);
                AddDefaultParametes(request);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new
                {
                    expression
                });
                tasks.Add(m_Client.ExecuteTaskAsync(request, token));
            }
            return Task.WhenAll(tasks);

        }


    }


}

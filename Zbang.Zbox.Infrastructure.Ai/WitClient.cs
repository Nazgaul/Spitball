using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Deserializers;
using Zbang.Zbox.ViewModel.Dto;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public class Entity
    {
        //public string metadata { get; set; }

        public string Value { get; set; }

        public double Confidence { get; set; }
    }


    public class WitClient
    {
        const string DefaultApiVersion = "20160516";
        private readonly string m_Token;
        private readonly RestClient m_Client;

        public WitClient(string token)
        {
            m_Token = token;
            m_Client = new RestClient("https://api.wit.ai");
            m_Client.AddHandler("*", new JsonDeserializer());
        }
        public async Task<WitResponse> GetMessageAsync(string q)
        {
            //var client = new RestClient("https://api.wit.ai");

            var request = new RestRequest("message", Method.GET);

            request.AddQueryParameter("q", q);

            //client.AddHandler("*", new JsonDeserializer());
            AddDefaultParametes(request);

            var data = await m_Client.ExecuteTaskAsync<WitResponse>(request);
            return data.Data;
        }

        private void AddDefaultParametes(RestRequest request)
        {
            request.AddQueryParameter("v", DefaultApiVersion);
            request.AddHeader("Authorization", "Bearer " + m_Token);
        }


        internal Task UpdateEntityValuesAsync(string entityName, string value, IList<string> expressions,
            string metadata)
        {
            var request = new RestRequest($"entities/{entityName}/values", Method.POST);
            AddDefaultParametes(request);
            request.RequestFormat = DataFormat.Json;
            if (expressions == null)
            {
                expressions = new List<string>();
            }
            expressions.Add(value);

            request.AddBody(new
            {
                value,
                expressions,
                metadata
            });
            return m_Client.ExecuteTaskAsync(request);
        }


    }


}

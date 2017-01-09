using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Deserializers;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public class WitResponse
    {
        //public List<Intent> Intent { get; set; }

        public Dictionary<string, List<Entity>> Entities { get; set; }

        public Entity Intent
        {
            get
            {
                List<Entity> entity;
                if (Entities.TryGetValue("Intent", out entity))
                {
                    return entity[0];
                }
                return null;
            }
        }
    }

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

        public WitClient(string token)
        {
            m_Token = token;
        }
        public async Task<WitResponse> GetMessageAsync(string q)
        {
            var client = new RestClient("https://api.wit.ai");

            var request = new RestRequest("message", Method.GET);
            request.AddQueryParameter("v", DefaultApiVersion);
            request.AddQueryParameter("q", q);

            client.AddHandler("*", new JsonDeserializer());
            request.AddHeader("Authorization", "Bearer " + m_Token);

            var data = await client.ExecuteTaskAsync<WitResponse>(request);
            return data.Data;
            //// execute the request
            //IRestResponse response = client.Execute(request);
            //var content = response.Content;

            //return JsonConvert.DeserializeObject<Message>(content);
        }
    }
}

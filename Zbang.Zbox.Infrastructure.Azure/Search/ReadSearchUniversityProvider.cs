using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.ViewModel.Dto.Library;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class ReadSearchUniversityProvider : IUniversityReadSearchProvider, IDisposable
    {
        const string CloudentssearchSearchWindowsNet = "cloudentssearch.search.windows.net";
        const string ApiKey = "3631C973B9E89471C33C9BA7CD98475B";

        private readonly HttpClient m_HttpClient;

        public ReadSearchUniversityProvider()
        {
            m_HttpClient = new HttpClient();
            m_HttpClient.DefaultRequestHeaders.Add("Host", CloudentssearchSearchWindowsNet);
            m_HttpClient.DefaultRequestHeaders.Add("api-key", ApiKey);
            m_HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<UniversityByPrefixDto>> SearchUniversity(string term)
        {
            //using (var httpClient = new HttpClient())
            //{




            var retVal = new List<UniversityByPrefixDto>();
            using (var httpResponse = await m_HttpClient.GetAsync(
                string.Format("https://cloudentssearch.search.windows.net/indexes/universities/docs?search={0}&api-version=2014-07-31-Preview&$select=id,name,imageField,membersCount", BuildQuery(term))))
            {
                var str = await httpResponse.Content.ReadAsStringAsync();
                dynamic o = JObject.Parse(str);
                foreach (dynamic university in o.value)
                {
                    retVal.Add(new UniversityByPrefixDto(
                        Convert.ToString(university.name),
                        Convert.ToString(university.imageField),
                        Convert.ToInt64(university.id),
                        Convert.ToInt64(university.membersCount)));
                }
            }
            if (retVal.Count == 0)
            {
                return await Suggest(term);
            }
            return retVal;

            // }
        }

        public async Task<IEnumerable<UniversityByPrefixDto>> Suggest(string term)
        {
            //using (var httpClient = new HttpClient())
            //{



            var retVal = new List<UniversityByPrefixDto>();
            using (var httpResponse = await m_HttpClient.GetAsync(
                string.Format("https://cloudentssearch.search.windows.net/indexes/universities/docs/suggest?search={0}&fuzzy=true&api-version=2014-07-31-Preview&$select=id,name,imageField,membersCount", term)))
            {
                var str = await httpResponse.Content.ReadAsStringAsync();
                dynamic o = JObject.Parse(str);
                foreach (dynamic suggest in o.value)
                {
                    retVal.Add(new UniversityByPrefixDto(
                       Convert.ToString(suggest.name),
                       Convert.ToString(suggest.imageField),
                       Convert.ToInt64(suggest.id),
                       Convert.ToInt64(suggest.membersCount)));
                }
            }
            return retVal;

            // }

        }

        private string BuildQuery(string term)
        {
            return term.Replace(" ", "+") + "*";
        }

        public void Dispose()
        {
            m_HttpClient.Dispose();
        }
    }
}

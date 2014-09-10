using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Zbang.Zbox.ViewModel.Dto.Library;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class ReadSearchUniversityProvider : IUniversityReadSearchProvider, IDisposable
    {

        private readonly HttpClient m_HttpClient;

        public ReadSearchUniversityProvider()
        {
            m_HttpClient = new HttpClient();
            m_HttpClient.DefaultRequestHeaders.Host = WriteSearchUniversityProvider.CloudentssearchSearchWindowsNet;
            m_HttpClient.DefaultRequestHeaders.Add("api-key", WriteSearchUniversityProvider.ApiKey);
            m_HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<UniversityByPrefixDto>> SearchUniversity(string term)
        {
            //using (var httpClient = new HttpClient())
            //{




            var retVal = new List<UniversityByPrefixDto>();
            using (var httpResponse = await m_HttpClient.GetAsync(
                string.Format("{1}/indexes/universities/docs?search={0}&api-version=2014-07-31-Preview&$select=id,name,imageField", BuildQuery(term), WriteSearchUniversityProvider.CloudentsSearchApiPrefix)))
            {
                var str = await httpResponse.Content.ReadAsStringAsync();
                ConvertToDto(retVal, str);
            }
            if (retVal.Count == 0)
            {
                return await Suggest(term);
            }
            return retVal;

            // }
        }

        private void ConvertToDto(List<UniversityByPrefixDto> retVal, string str)
        {
            dynamic o = JObject.Parse(str);
            if (o == null)
            {
                return;
            }
            foreach (dynamic university in o.value)
            {
                retVal.Add(new UniversityByPrefixDto(
                    Convert.ToString(university.name),
                    Convert.ToString(university.imageField),
                    Convert.ToInt64(university.id)
                    ));
            }
        }

        public async Task<IEnumerable<UniversityByPrefixDto>> Suggest(string term)
        {
            //using (var httpClient = new HttpClient())
            //{



            var retVal = new List<UniversityByPrefixDto>();
            using (var httpResponse = await m_HttpClient.GetAsync(
                string.Format("{1}/indexes/universities/docs/suggest?search={0}&fuzzy=true&api-version=2014-07-31-Preview&$select=id,name,imageField", term, WriteSearchUniversityProvider.CloudentsSearchApiPrefix)))
            {
                var str = await httpResponse.Content.ReadAsStringAsync();
                ConvertToDto(retVal, str);
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

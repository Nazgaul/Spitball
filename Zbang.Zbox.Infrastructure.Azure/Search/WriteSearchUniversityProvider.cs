using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.Library;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class WriteSearchUniversityProvider : IUniversityWriteSearchProvider
    {
        private readonly IZboxReadServiceWorkerRole m_DbReadService;
        internal const string CloudentssearchSearchWindowsNet = "cloudents.search.windows.net";
        internal const string ApiKey = "5B0433BFBBE625C9D60F7330CFF103F0";
        internal const string CloudentsSearchApiPrefix = "https://cloudents.search.windows.net";

        public WriteSearchUniversityProvider(IZboxReadServiceWorkerRole dbReadService)
        {
            m_DbReadService = dbReadService;
        }
        public async Task BuildUniversityData()
        {
            var indexExists = await CheckIndexExits();
            if (!indexExists)
            {
                await BuildIndex();
            }
            var resource = LoadResource("UniversityData.txt");
            var universities = ConvertToObject(resource);
            await BuildData(universities);
        }

        public async Task UpdateData(UniversitySearchDto university)
        {
            var item = new SearchUniversity
            {
                Id = university.Id.ToString(CultureInfo.InvariantCulture),
                Name = university.Name,
                @SearchAction = "upload",
                ImageField = university.Image,
                Extra4 = String.Join(
                " ",
                university.Name.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.StartsWith("ה") || w.StartsWith("ל"))
                .Select(s => s.Remove(0, 1)))
            };
            await UpdateDocuments(new[] { item });
        }

        private async Task BuildData(IEnumerable<University> universitiesExtra)
        {
            universitiesExtra = universitiesExtra.ToList();
            var universities = await m_DbReadService.GetUniversityDetail();

            var list = new List<SearchUniversity>();
            foreach (var university in universities)
            {
                var extraDetail = universitiesExtra.FirstOrDefault(f => f.Id == university.Id);

                var item = new SearchUniversity
                {
                    Id = university.Id.ToString(CultureInfo.InvariantCulture),
                    Name = university.Name,
                    @SearchAction = "upload",
                    ImageField = university.Image,
                    Extra4 = String.Join(
                    " ",
                    university.Name.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.StartsWith("ה") || w.StartsWith("ל"))
                    .Select(s => s.Remove(0, 1)))
                };
                if (extraDetail != null)
                {
                    item.Extra1 = extraDetail.Extra1;
                    item.Extra2 = extraDetail.Extra2;
                    item.Extra3 = extraDetail.Extra3;
                }


                list.Add(item);



            }

            await UpdateDocuments(list);
        }

        private static async Task UpdateDocuments(IEnumerable<SearchUniversity> list)
        {
            using (var httpClient = new HttpClient())
            {

                httpClient.DefaultRequestHeaders.Add("Host", CloudentssearchSearchWindowsNet);

                httpClient.DefaultRequestHeaders.Add("api-key", ApiKey);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string output = JsonConvert.SerializeObject(new { value = list }, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });


                using (var bodyContent = new StringContent(output, Encoding.UTF8, "application/json"))
                {
                    using (var retVal = await httpClient.PostAsync(
                       CloudentsSearchApiPrefix + "/indexes/universities/docs/index?api-version=2014-07-31-Preview",
                        bodyContent))
                    {
                         var x = await retVal.Content.ReadAsStringAsync();
                    }
                }
            }
        }

       

        private async Task<bool> CheckIndexExits()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Host", CloudentssearchSearchWindowsNet);
                httpClient.DefaultRequestHeaders.Add("api-key", ApiKey);

                using (var retVal = await httpClient.GetAsync(
                    CloudentsSearchApiPrefix + "/indexes/universities?api-version=2014-07-31-Preview"))
                {
                    return retVal.IsSuccessStatusCode;
                }
            }
        }

        public async Task BuildIndex()
        {


            var list = new List<SearchField>
            {
                new SearchField {Name = "id", Type = "Edm.String", Key = true},
                new SearchField {Name = "name", Type = "Edm.String", Suggestions = true, Searchable = true},
                new SearchField {Name = "extra1", Type = "Edm.String", Searchable = true},
                new SearchField {Name = "extra2", Type = "Edm.String", Searchable = true},
                new SearchField {Name = "extra3", Type = "Edm.String", Searchable = true},
                new SearchField {Name = "extra4", Type = "Edm.String", Searchable = true},
                new SearchField {Name = "imageField", Type = "Edm.String", Searchable = false},
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Host", CloudentssearchSearchWindowsNet);
                httpClient.DefaultRequestHeaders.Add("api-key", ApiKey);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                string output = JsonConvert.SerializeObject(new SearchIndex("universities", list), new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });


                using (var bodyContent = new StringContent(output, Encoding.UTF8, "application/json"))
                {
                    using (await httpClient.PutAsync(
                        CloudentsSearchApiPrefix + "/indexes/universities?api-version=2014-07-31-Preview",
                        bodyContent))
                    {
                    }
                }
            }
        }

        private string LoadResource(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Zbang.Zbox.Infrastructure.Azure.Search." + resourceName))
            {
                if (stream != null)
                {
                    var content = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(content, 0, (int)stream.Length);
                    return Encoding.UTF8.GetString(content).Replace(Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble()), string.Empty);
                }
                return string.Empty;
            }
        }

        private IEnumerable<University> ConvertToObject(string data)
        {
            var universities = new List<University>();
            var universitiesData = data.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var universitydata in universitiesData)
            {
                var splitData = universitydata.Split(new[] { ',' });

                long id;
                long.TryParse(splitData[0], out id);
                var university = new University
                {
                    Id = long.Parse(splitData[0]),
                    // Name = splitData[1].Trim(),
                    Extra1 = splitData[1] != null ? splitData[1].Trim() : null,
                    Extra2 = splitData[2] != null ? splitData[2].Trim() : null,
                    Extra3 = splitData[3] != null ? splitData[3].Trim() : null
                };
                //university.Extra = String.Join(" ", splitData.Skip(2)).Trim();

                universities.Add(university);
            }

            return universities;

        }
    }
}

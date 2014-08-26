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

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class SearchUniversityProvider : IUniversityWriteSearchProvider
    {
        private readonly IZboxReadServiceWorkerRole m_DbReadService;
        public SearchUniversityProvider(IZboxReadServiceWorkerRole dbReadService)
        {
            m_DbReadService = dbReadService;
        }
        public async void BuildUniversityData()
        {
            var indexExists = await CheckIndexExits("universities");
            if (!indexExists)
            {
                await BuildIndex();
            }
            var resource = LoadResource("UniversityData.txt");
            var universities = ConvertToObject(resource);
            BuildData(universities);
        }

        private async void BuildData(IEnumerable<University> universitiesExtra)
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
                    MembersCount = (int)university.MemberCount
                };
                if (extraDetail != null)
                {
                    item.Extra1 = extraDetail.Extra1;
                    item.Extra2 = extraDetail.Extra2;
                    item.Extra3 = extraDetail.Extra3;
                }

                list.Add(item);

                /* doc.Add(new Field(IdField, university.Id.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
                  doc.Add(new Field(NameField, university.Name, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                  if (extraDetail != null)
                  {
                      doc.Add(new Field("extra1", extraDetail.Extra1, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                      doc.Add(new Field("extra2", extraDetail.Extra2, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                      doc.Add(new Field("extra3", extraDetail.Extra3, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO));
                  }
                  doc.Add(new Field(ImageField, university.Image, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
                  doc.Add(new Field(MembersCountField, university.MemberCount.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));*/
                // list.Add(new SearchField)


            }

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Host", "cloudentssearch.search.windows.net");
                httpClient.DefaultRequestHeaders.Add("api-key", "3631C973B9E89471C33C9BA7CD98475B");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string output = JsonConvert.SerializeObject(new { value = list }, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });


                using (var bodyContent = new StringContent(output, Encoding.UTF8, "application/json"))
                {
                    using (var retVal = await httpClient.PostAsync(
                        "https://cloudentssearch.search.windows.net/indexes/universities/docs/index?api-version=2014-07-31-Preview",
                        bodyContent))
                    {
                        var x = await retVal.Content.ReadAsStringAsync();
                    }
                }
            }
        }

        private async Task<bool> CheckIndexExits(string indexName)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Host", "cloudentssearch.search.windows.net");
                httpClient.DefaultRequestHeaders.Add("api-key", "3631C973B9E89471C33C9BA7CD98475B");

                using (var retVal = await httpClient.GetAsync(
                    "https://cloudentssearch.search.windows.net/indexes/universities?api-version=2014-07-31-Preview"))
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
                new SearchField {Name = "imageField", Type = "Edm.String", Searchable = false},
                new SearchField {Name = "membersCount", Type = "Edm.Int32"}
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Host", "cloudentssearch.search.windows.net");
                httpClient.DefaultRequestHeaders.Add("api-key", "3631C973B9E89471C33C9BA7CD98475B");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                string output = JsonConvert.SerializeObject(new SearchIndex("universities", list), new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });


                using (var bodyContent = new StringContent(output, Encoding.UTF8, "application/json"))
                {
                    using (var retVal = await httpClient.PutAsync(
                        "https://cloudentssearch.search.windows.net/indexes/universities?api-version=2014-07-31-Preview",
                        bodyContent))
                    {
                        var x = await retVal.Content.ReadAsStringAsync();
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

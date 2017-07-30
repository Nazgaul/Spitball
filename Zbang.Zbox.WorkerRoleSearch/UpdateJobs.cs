using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Spatial;
using Newtonsoft.Json.Linq;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateJobs : ISchedulerProcess
    {
        //private readonly ISearchServiceWrite<Job> m_JobSearchService;
        private readonly JobsProvider m_JobSearchService;
        private readonly Dictionary<string, GeographyPoint> m_ZipToLocationCache = new Dictionary<string, GeographyPoint>();
        private readonly ILogger m_Logger;
        private readonly ILocalStorageProvider m_LocalStorage;

        public UpdateJobs(JobsProvider jobSearchService, ILogger logger, ILocalStorageProvider localStorage)
        {
            m_JobSearchService = jobSearchService;
            m_Logger = logger;
            m_LocalStorage = localStorage;
        }



        public async Task<bool> ExecuteAsync(int index, Func<int, TimeSpan, Task> progressAsync,
            CancellationToken token)
        {
            if (progressAsync == null) throw new ArgumentNullException(nameof(progressAsync));
            m_Logger.Info("Update jobs starting to work");
            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential("cqSCcVaGdfHVTefIBGCTdLqmYPeboa", "LAGhyQrQdfLumRjMrXVVVISAnrbTZn")
            };
            string locationToSave;
            using (var client = new HttpClient(handler))
            {
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",)
                var result = await client.GetAsync("https://www.wayup.com/integrations/clickcast-feed/",
                    HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
                result.EnsureSuccessStatusCode();



                using (var stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    locationToSave = await m_LocalStorage.SaveFileToStorageAsync(stream, "jobs.xml", true)
                        .ConfigureAwait(false);
                }
            }
            var list = new List<Job>();
            foreach (var job in GetJobs(locationToSave))
            {
                var jobObject = await m_JobSearchService.GetByIdAsync(job.Id, token).ConfigureAwait(false);

                if (jobObject == null)
                {
                    jobObject = await CreateJobObjectAsync(job).ConfigureAwait(false);

                }
                else if (jobObject.Location == null && !string.IsNullOrEmpty(job.Zip))
                {
                    jobObject = await CreateJobObjectAsync(job).ConfigureAwait(false);
                }
                else if (jobObject.InsertDate > DateTime.UtcNow.AddDays(-1))
                {
                    continue;
                }
                jobObject.InsertDate = DateTime.UtcNow;
                list.Add(jobObject);
                if (list.Count > 200)
                {
                    var t1 = m_JobSearchService.UpdateDataAsync(list, token);

                    var t2 = progressAsync.Invoke(0, TimeSpan.FromMinutes(10));

                    await Task.WhenAll(t1, t2).ConfigureAwait(false);
                    list.Clear();
                }


            }



            if (list.Count > 0)
            {
                await m_JobSearchService.UpdateDataAsync(list, token).ConfigureAwait(false);
            }
            m_Logger.Info("Update jobs finish to work");
            return true;
        }

        private async Task<Job> CreateJobObjectAsync(WayUpJob job)
        {
            var dateTimeStr = job.PostedDate.Replace("UTC", string.Empty).Trim();

            DateTime? dateTime = null;
            if (DateTimeOffset.TryParse(dateTimeStr, out DateTimeOffset p))
            {
                dateTime = p.DateTime;
            }


            var location = await GetLocationViaZipAsync(job.Zip).ConfigureAwait(false);
            var searchJobObject = new Job
            {
                City = job.City,
                CompensationType = job.CompType,
                DateTime = dateTime,
                Id = job.Id,
                JobType = job.JobType,
                Location = location,
                Responsibilities = job.Responsibilities,
                State = job.State,
                Title = job.Title,
                InsertDate = DateTime.UtcNow
            };
            return searchJobObject;
        }


        private async Task<GeographyPoint> GetLocationViaZipAsync(string zip)
        {
            if (string.IsNullOrEmpty(zip))
            {
                return null;
            }
            if (m_ZipToLocationCache.TryGetValue(zip, out GeographyPoint point))
            {
                return point;
            }
            using (var client = new HttpClient())
            {
                try
                {
                    var str = await client.GetStringAsync(
                        "https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyC7lF3qKA4N8Ej6ycTEIB08h8rWUlKqPKY&components=postal_code:" +
                        zip).ConfigureAwait(false);
                    var jsonObject = JObject.Parse(str);
                    if (jsonObject["status"].Value<string>() != "OK")
                    {
                        return null;
                    }
                    var location = jsonObject["results"].ToArray()[0]["geometry"]["location"];
                    var lat = location["lat"].Value<double>();
                    var lng = location["lng"].Value<double>();
                    point = GeographyPoint.Create(lat, lng);
                    m_ZipToLocationCache[zip] = point;
                    return point;
                }
                catch (Exception ex)
                {
                    m_Logger.Exception(ex, new Dictionary<string, string>
                    {
                        {"service", "Jobs"},
                        {"zip", zip}
                    });
                    return null;
                }

            }
        }

        private IEnumerable<WayUpJob> GetJobs(string location)
        {

            var serializer = new XmlSerializer(typeof(WayUpJob));
            using (var reader = XmlReader.Create(location))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element) continue;
                    if (reader.Name != "job") continue;
                    if (!(XNode.ReadFrom(reader) is XElement el)) continue;
                    var str = el.ToString();
                    var stringReader = new StringReader(str);
                    var job = (WayUpJob)serializer.Deserialize(stringReader);
                    job.Id = Md5HashGenerator.GenerateKey(str);
                    yield return job;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateJobs : ISchedulerProcess
    {
        private readonly JobsProvider m_JobSearchService;
        private readonly ILogger m_Logger;
        private readonly ILocalStorageProvider m_LocalStorage;
        private readonly IZipToLocationProvider m_ZipToLocation;

        public UpdateJobs(JobsProvider jobSearchService, ILogger logger,
            ILocalStorageProvider localStorage, IZipToLocationProvider zipToLocation)
        {
            m_JobSearchService = jobSearchService;
            m_Logger = logger;
            m_LocalStorage = localStorage;
            m_ZipToLocation = zipToLocation;
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
                    locationToSave = await m_LocalStorage.SaveFileToStorageAsync(stream, "jobs.xml", index == 0)
                        .ConfigureAwait(false);
                }
            }
            var list = new List<Job>();
            var i = 0;
            foreach (var job in GetJobs(locationToSave))
            {
                if (i < index)
                {
                    continue;
                }
                i++;
                var jobObject = await CreateJobObjectAsync(job).ConfigureAwait(false);
                list.Add(jobObject);
                if (list.Count > 100)
                {
                    var t1 = m_JobSearchService.UpdateDataAsync(list, token);
                    var t2 = progressAsync.Invoke(i, TimeSpan.FromMinutes(10));
                    m_Logger.Info("Update jobs finish processing " + i);

                    await Task.WhenAll(t1, t2).ConfigureAwait(false);
                    token.ThrowIfCancellationRequested();
                    list.Clear();
                }


            }
            if (list.Count > 0)
            {
                await m_JobSearchService.UpdateDataAsync(list, token).ConfigureAwait(false);
            }
            m_Logger.Info("Update jobs Going To delete old jobs");
            var oldJobs = await m_JobSearchService.GetOldJobsAsync(token).ConfigureAwait(false);

            await m_JobSearchService.DeleteDataAsync(oldJobs, token).ConfigureAwait(false);
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


            var location = await m_ZipToLocation.GetLocationViaZipAsync(job.Zip).ConfigureAwait(false);
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
                InsertDate = DateTime.UtcNow,
                Url = job.Url,
                Company = job.Company,
            };
            return searchJobObject;
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


        private IEnumerable<T> GetJobs<T>(string location) where T : ISearchObject
        {

            var serializer = new XmlSerializer(typeof(T));
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
                    var job = (T)serializer.Deserialize(stringReader);
                    job.Id = Md5HashGenerator.GenerateKey(str);
                    yield return job;
                }
            }
        }

        private string JobTypeConversion(string jobType)
        {
            return jobType.Replace("_", " ").Replace("-", " ").ToLowerInvariant();
        }
    }
}

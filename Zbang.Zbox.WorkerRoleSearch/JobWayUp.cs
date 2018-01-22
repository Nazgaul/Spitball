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
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Enum;
using Cloudents.Infrastructure.Write;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class JobWayUp : UpdateAffiliate<WayUpJob, Job>
    {
        private readonly JobSearchWrite _jobSearchService;
        private readonly IZipToLocationProvider _zipToLocation;

        public JobWayUp(JobSearchWrite jobSearchService, ILogger logger,
            ILocalStorageProvider localStorage, IZipToLocationProvider zipToLocation) :
            base(logger, localStorage)
        {
            _jobSearchService = jobSearchService;
            _zipToLocation = zipToLocation;
        }

        protected override Task DeleteOldItemsAsync(CancellationToken token)
        {
            return _jobSearchService.DeleteOldJobsAsync(token);
        }

        protected override HttpClientHandler HttpHandler()
        {
            return new HttpClientHandler
            {
                Credentials = new NetworkCredential("cqSCcVaGdfHVTefIBGCTdLqmYPeboa", "LAGhyQrQdfLumRjMrXVVVISAnrbTZn")
            };
        }

        protected override string FileLocation => "wayUpJobs.xml";
        protected override string Url => "https://www.wayup.com/integrations/clickcast-feed/";
        protected override string Service => "WayUp jobs";

        protected override IEnumerable<WayUpJob> GetT(string location)
        {
            var serializer = new XmlSerializer(typeof(WayUpJob));
            using (var reader = XmlReader.Create(location))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element) continue;
                    if (reader.Name != "job") continue;
                    string str;
                    try
                    {
                        if (!(XNode.ReadFrom(reader) is XElement el)) continue;
                        str = el.ToString();
                    }
                    catch (XmlException)
                    {
                        continue;
                    }
                    var stringReader = new StringReader(str);
                    var job = (WayUpJob)serializer.Deserialize(stringReader);
                    job.Id = MD5HashGenerator.GenerateKey(str);
                    yield return job;
                }
            }
        }

        protected override async Task<Job> ParseTAsync(WayUpJob obj, CancellationToken token)
        {
            var dateTimeStr = obj.PostedDate.Replace("UTC", string.Empty).Trim();

            DateTime? dateTime = null;
            if (DateTimeOffset.TryParse(dateTimeStr, out var p))
            {
                dateTime = p.DateTime;
            }

            var location = await _zipToLocation.GetLocationViaZipAsync(obj.Zip).ConfigureAwait(false);
            return new Job
            {
                City = obj.City,
                Compensation = obj.CompType,
                DateTime = dateTime,
                Id = obj.Id,
                JobType = JobTypeConversion(obj.JobType),
                Location = location,
                Description = obj.Responsibilities,
                State = obj.State,
                Title = obj.Title,
                InsertDate = DateTime.UtcNow,
                Url = obj.Url,
                Company = obj.Company,
            };
        }

        protected override Task UpdateSearchAsync(IEnumerable<Job> list, CancellationToken token)
        {
            return _jobSearchService.UpdateDataAsync(list, token);
        }

        private static JobFilter JobTypeConversion(string jobType)
        {
            return JobFilter.None;// jobType?.Replace("_", " ").ToLowerInvariant();
        }
    }
}

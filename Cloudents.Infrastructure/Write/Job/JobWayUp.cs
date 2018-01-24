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
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Write.Job.Entities;

namespace Cloudents.Infrastructure.Write.Job
{
    public class JobWayUp : UpdateAffiliate<WayUpJob, Core.Entities.Search.Job>
    {
        private readonly JobSearchWrite _jobSearchService;
        private readonly IGooglePlacesSearch _zipToLocation;
        private readonly IKeyGenerator _keyGenerator;

        public JobWayUp(JobSearchWrite jobSearchService, ILogger logger,
            IDownloadFile localStorage, IGooglePlacesSearch zipToLocation, IKeyGenerator keyGenerator) :
            base(logger, localStorage)
        {
            _jobSearchService = jobSearchService;
            _zipToLocation = zipToLocation;
            _keyGenerator = keyGenerator;
        }

        protected override Task DeleteOldItemsAsync(CancellationToken token)
        {
            return _jobSearchService.DeleteOldJobsAsync(Source, token);
        }

        protected override HttpClientHandler HttpHandler()
        {
            return new HttpClientHandler
            {
                Credentials = new NetworkCredential("cqSCcVaGdfHVTefIBGCTdLqmYPeboa", "LAGhyQrQdfLumRjMrXVVVISAnrbTZn")
            };
        }

        protected override string FileLocation => "wayUpJobs.xml";
        protected override Uri Url => new Uri("https://www.wayup.com/integrations/clickcast-feed/");
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
                    job.Id = _keyGenerator.GenerateKey(str);
                    yield return job;
                }
            }
        }

        private const string Source = "WayUp";

        protected override async Task<Core.Entities.Search.Job> ParseTAsync(WayUpJob obj, CancellationToken token)
        {
            var dateTimeStr = obj.PostedDate.Replace("UTC", string.Empty).Trim();

            DateTime? dateTime = null;
            if (DateTimeOffset.TryParse(dateTimeStr, out var p))
            {
                dateTime = p.DateTime;
            }

            var location = await _zipToLocation.GeoCodingByZipAsync(obj.Zip, token).ConfigureAwait(false);
            var job = new Core.Entities.Search.Job
            {
                City = obj.City,
                Compensation = obj.CompType,
                DateTime = dateTime,
                Id = obj.Id,
                JobType2 = JobTypeConversion(obj.JobType),
                Location = GeoPoint.ToPoint(location),
                Description = obj.Responsibilities,
                State = obj.State,
                Title = obj.Title,
                InsertDate = DateTime.UtcNow,
                Url = obj.Url,
                Company = obj.Company,
                Source = Source
            };
            if (job.JobType2 == JobFilter.None && obj.JobType != null)
            {
                job.Extra = new[] { obj.JobType };

            }

            return job;
        }

        protected override Task UpdateSearchAsync(IEnumerable<Core.Entities.Search.Job> list, CancellationToken token)
        {
            return _jobSearchService.UpdateDataAsync(list, token);
        }

        private static JobFilter JobTypeConversion(string jobType)
        {
            if (string.IsNullOrEmpty(jobType))
            {
                return JobFilter.None;
            }
            jobType = jobType.ToLowerInvariant();


            switch (jobType)
            {
                case "full_time":
                    return JobFilter.FullTime;
                case "internship":
                    return JobFilter.Internship;
                case "part_time":
                    return JobFilter.PartTime;
                case "remote":
                    return JobFilter.Remote;
                case "campus_rep":
                    return JobFilter.CampusRep;
            }

            return JobFilter.None;// jobType?.Replace("_", " ").ToLowerInvariant();
        }
    }
}

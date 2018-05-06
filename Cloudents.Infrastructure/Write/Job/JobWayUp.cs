using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Write.Job.Entities;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Write.Job
{
    [UsedImplicitly]
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

        protected override Task DeleteOldItemsAsync(DateTime timeToDelete, CancellationToken token)
        {
            return _jobSearchService.DeleteOldJobsAsync(Source, timeToDelete, token);
        }

        protected override string FileLocation => "wayUpJobs.xml";
        protected override Uri Url => new Uri("https://clickcastfeeds.s3.amazonaws.com/714559ba13ba9675ace7233a8b08eb48/feed.xml.gz");
        protected override string Service => "WayUp jobs";
        protected override AuthenticationHeaderValue HttpHandler => null;
        //{
        //    get
        //    {
        //        var byteArray = Encoding.ASCII.GetBytes($"cqSCcVaGdfHVTefIBGCTdLqmYPeboa:LAGhyQrQdfLumRjMrXVVVISAnrbTZn");
        //        return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        //    }
        //}

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

            //var dateTimeStr = obj.PostedDate.Replace("UTC", string.Empty).Trim();

            DateTime? dateTime = null;
            if (DateTime.TryParse(obj.PostedDate, out var p))
            {
                dateTime = p;
            }


            var (_, point) = await _zipToLocation.GeoCodingByZipAsync(obj.Zip, token).ConfigureAwait(false);
            var job = new Core.Entities.Search.Job
            {
                City = obj.City,
                //Compensation = obj.CompType,
                DateTime = dateTime,
                Id = obj.Id,
                JobType = JobTypeConversion(obj.JobType),
                Location = point?.ToPoint(),
                Description = obj.Body,
                State = obj.State,
                Title = obj.Title,
                InsertDate = DateTime.UtcNow,
                Url = obj.Url,
                Company = obj.Company,
                Source = Source
                
            };
            if (job.JobType == JobFilter.None && obj.JobType != null)
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

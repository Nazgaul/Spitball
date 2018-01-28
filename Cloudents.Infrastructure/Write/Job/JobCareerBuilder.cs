using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Cloudents.Core;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Write.Job.Entities;

namespace Cloudents.Infrastructure.Write.Job
{
    public class JobCareerBuilder : UpdateAffiliate<CareerBuilderJobs, Core.Entities.Search.Job>, IDisposable
    {
        private readonly JobSearchWrite _jobSearchService;
        private readonly IGooglePlacesSearch _zipToLocation;
        public JobCareerBuilder(ILogger logger, IDownloadFile localStorage, JobSearchWrite jobSearchService, IGooglePlacesSearch zipToLocation) : base(logger, localStorage)
        {
            _jobSearchService = jobSearchService;
            _zipToLocation = zipToLocation;
            HttpHandler = new HttpClientHandler();
        }

        protected override string FileLocation => "CareerBuilderJobs.xml";

        protected override Uri Url =>
            new Uri("https://clickcastfeeds.s3.amazonaws.com/2221af50160b28c835156240c9f8d21f/feed.xml");

        protected override string Service => "CareerBuilder jobs";
        protected override HttpClientHandler HttpHandler { get; }

        protected override IEnumerable<CareerBuilderJobs> GetT(string location)
        {
            var serializer = new XmlSerializer(typeof(CareerBuilderJobs));
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
                    var job = (CareerBuilderJobs)serializer.Deserialize(stringReader);
                    yield return job;
                }
            }
        }

        protected override async Task<Core.Entities.Search.Job> ParseTAsync(CareerBuilderJobs obj, CancellationToken token)
        {
            var location = await _zipToLocation.GeoCodingByZipAsync(obj.Zip, token).ConfigureAwait(false);
            return new Core.Entities.Search.Job
            {
                City = obj.City,
                Compensation = "paid",
                DateTime = obj.posted_at,
                Id = obj.job_reference,
                JobType = JobTypeConversion(obj.job_type),
                Location = GeoPoint.ToPoint(location),
                Description = StripHtml(obj.body).RemoveEndOfString(300),
                State = obj.state,
                Title = obj.title,
                InsertDate = DateTime.UtcNow,
                Url = obj.url,
                Company = obj.company,
                Extra = new[] { obj.category },
                Source = "CareerBuilder"
            };
        }

        private static string StripHtml(string body)
        {
            var t = RegEx.RemoveHtmlTags.Replace(body, string.Empty);
            return RegEx.SpaceReg.Replace(t, " ");
        }

        protected override Task UpdateSearchAsync(IEnumerable<Core.Entities.Search.Job> list, CancellationToken token)
        {
            return _jobSearchService.UpdateDataAsync(list, token);
        }

        protected override Task DeleteOldItemsAsync(DateTime timeToDelete, CancellationToken token)
        {
            return _jobSearchService.DeleteOldJobsAsync("CareerBuilder", timeToDelete, token);
        }

        private static JobFilter JobTypeConversion(string jobType)
        {
            if (string.IsNullOrEmpty(jobType))
            {
                return JobFilter.None;
            }
            jobType = jobType.Replace("-", " ").ToLowerInvariant();
            switch (jobType)
            {
                case "full time":
                    return JobFilter.FullTime;
                case "internship":
                case "intern":
                    return JobFilter.Internship;
                case "part time":
                case "full time/part time":
                    return JobFilter.PartTime;
                case "remote":
                    return JobFilter.Remote;
                case "campus rep":
                    return JobFilter.CampusRep;
                case "contractor":
                case "contract to hire":
                    return JobFilter.Contractor;
                case "per diem":
                case "temporary/seasonal":
                case "seasonal/temp":
                    return JobFilter.Temporary;
            }
            return JobFilter.None; //jobType?.Replace("-", " ").ToLowerInvariant();
        }

        public void Dispose()
        {
            HttpHandler?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

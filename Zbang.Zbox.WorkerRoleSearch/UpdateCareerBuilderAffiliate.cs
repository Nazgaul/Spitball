using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateCareerBuilderAffiliate : UpdateAffiliate<CareerBuilderJobs, Job>
    {
        private readonly JobsProvider m_JobSearchService;
        private readonly IZipToLocationProvider m_ZipToLocation;
        public UpdateCareerBuilderAffiliate(ILogger logger, ILocalStorageProvider localStorage, JobsProvider jobSearchService, IZipToLocationProvider zipToLocation) : base(logger, localStorage)
        {
            m_JobSearchService = jobSearchService;
            m_ZipToLocation = zipToLocation;
        }

        protected override string FileLocation => "CareerBuilderJobs.xml";

        protected override string Url =>
            "https://clickcastfeeds.s3.amazonaws.com/2221af50160b28c835156240c9f8d21f/feed.xml";

        protected override string Service => "CareerBuilder jobs";
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

        protected override async Task<Job> ParseTAsync(CareerBuilderJobs obj, CancellationToken token)
        {
            var location = await m_ZipToLocation.GetLocationViaZipAsync(obj.Zip).ConfigureAwait(false);
            var searchJobObject = new Job
            {
                City = obj.City,
                CompensationType = "paid",
                DateTime = obj.posted_at,
                Id = obj.job_reference,
                JobType = JobTypeConversion(obj.job_type),
                Location = location,
                Responsibilities = StripHtml(obj.body),
                State = obj.state,
                Title = obj.title,
                InsertDate = DateTime.UtcNow,
                Url = obj.url,
                Company = obj.company,
                Extra = new[] { obj.category }
            };
            return searchJobObject;
        }

        private static string StripHtml(string body)
        {
            var t = TextManipulation.RemoveHtmlTags.Replace(body, string.Empty);
            return TextManipulation.SpaceReg.Replace(t, " ");
        }

        protected override Task UpdateSearchAsync(IEnumerable<Job> list, CancellationToken token)
        {
            return m_JobSearchService.UpdateDataAsync(list, token);
        }

        protected override async Task DeleteOldItemsAsync(CancellationToken token)
        {
            var oldJobs = await m_JobSearchService.GetOldJobsAsync(token).ConfigureAwait(false);
            await m_JobSearchService.DeleteDataAsync(oldJobs, token).ConfigureAwait(false);
        }

        private static string JobTypeConversion(string jobType)
        {
            return jobType?.Replace("-", " ").ToLowerInvariant();
        }
    }
}

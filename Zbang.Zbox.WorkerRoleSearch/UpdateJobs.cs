﻿using System;
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

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateJobs : UpdateAffiliate<WayUpJob, Job>
    {
        private readonly JobsProvider m_JobSearchService;
        private readonly IZipToLocationProvider m_ZipToLocation;

        public UpdateJobs(JobsProvider jobSearchService, ILogger logger,
            ILocalStorageProvider localStorage, IZipToLocationProvider zipToLocation) : base(logger, localStorage)
        {
            m_JobSearchService = jobSearchService;
            m_ZipToLocation = zipToLocation;
        }


        protected override Task DeleteOldItemsAsync(CancellationToken token)
        {
            return m_JobSearchService.DeleteOldJobsAsync(token);
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
            if (DateTimeOffset.TryParse(dateTimeStr, out DateTimeOffset p))
            {
                dateTime = p.DateTime;
            }


            var location = await m_ZipToLocation.GetLocationViaZipAsync(obj.Zip).ConfigureAwait(false);
            return new Job
            {
                City = obj.City,
                CompensationType = obj.CompType,
                DateTime = dateTime,
                Id = obj.Id,
                JobType = JobTypeConversion(obj.JobType),
                Location = location,
                Responsibilities = obj.Responsibilities,
                State = obj.State,
                Title = obj.Title,
                InsertDate = DateTime.UtcNow,
                Url = obj.Url,
                Company = obj.Company,
            };
        }

        protected override Task UpdateSearchAsync(IEnumerable<Job> list, CancellationToken token)
        {
            return m_JobSearchService.UpdateDataAsync(list, token);
        }




        private static string JobTypeConversion(string jobType)
        {
            return jobType?.Replace("_", " ").ToLowerInvariant();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class JobsProvider : SearchServiceWrite<Job>, IJobsProviderReadService
    {
        private readonly ILogger m_Logger;
        public JobsProvider(ISearchConnection connection, ILogger logger) : base(connection, "jobs")
        {
            m_Logger = logger;
        }

        public override Index GetIndexStructure(string indexName)
        {
            var definition = new Index
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<Job>()
            };
            return definition;
        }

        public async Task<Job> GetByIdAsync(string id, CancellationToken token)
        {
            try
            {
                var t = await IndexClient.Documents.GetAsync<Job>(id, cancellationToken: token).ConfigureAwait(false);
                return t;
            }
            catch (CloudException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex, new Dictionary<string, string>
                {
                    {"service", nameof(JobsProvider)},
                    {"id", id}
                });
                return null;
            }
        }

        public async Task<IEnumerable<string>> GetOldJobsAsync(CancellationToken token)
        {
            var parameters = new SearchParameters()
            {
                Filter = $"insertDate lt {DateTime.UtcNow.AddDays(-2):yyyy-MM-dd'T'hh:mm:ss'Z'}",
                Select = new[] { "id" }
            };
            var result = await IndexClient.Documents.SearchAsync<Job>("*", parameters, cancellationToken: token).ConfigureAwait(false);
            return result.Results.Select(s => s.Document.Id);
        }
    }

    public interface IJobsProviderReadService
    {
        Task<Job> GetByIdAsync(string id, CancellationToken token);
    }

}

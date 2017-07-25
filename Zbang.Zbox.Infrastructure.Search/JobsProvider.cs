using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class JobsProvider : SearchServiceWrite<Job>
    {
        public JobsProvider(ISearchConnection connection) : base(connection, "jobs")
        {
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
    }
}

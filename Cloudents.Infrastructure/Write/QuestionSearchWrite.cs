using System.Collections.Generic;
using Cloudents.Core.Entities.Search;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write
{
    public class QuestionSearchWrite : SearchServiceWrite<Question>
    {
        public QuestionSearchWrite(SearchServiceClient client, string indexName) : base(client, indexName)
        {
        }

        protected override Index GetIndexStructure(string indexName)
        {
         //   Microsoft.Azure.Management.Search.Fluent.
            return new Index()
            {
                Name = indexName,
                Fields = new List<Field>
                {

                }
            };
        }
    }
}
using Cloudents.Infrastructure.Search;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Document = Cloudents.Core.Entities.Search.Document;

namespace Cloudents.Infrastructure.Write
{
    public class DocumentSearchWrite : SearchServiceWrite<Document>
    {
        public DocumentSearchWrite(SearchService client) : base(client, "item3")
        {
        }

        

        protected override Index GetIndexStructure(string indexName)
        {
            throw new System.NotImplementedException();
        }
    }
}
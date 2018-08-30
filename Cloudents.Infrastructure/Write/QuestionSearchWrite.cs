using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Entities.Search;
using Cloudents.Infrastructure.Search;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write
{
    public class QuestionSearchWrite : SearchServiceWrite<Question>
    {
        public QuestionSearchWrite(SearchService client, string indexName) : base(client, indexName)
        {
        }

        protected override Index GetIndexStructure(string indexName)
        {
            //var z = FluentSearchField<Question>.Make;
            //var t = new FluentSearch();
            //  t.Map<Question>(x => x.Id);
            //   Microsoft.Azure.Management.Search.Fluent.
            return new Index()
            {
                Name = indexName,
                Fields = new List<Field>
                {
                   
                   GetFieldBuilder.Map(x=>x.Id).IsKey(),

                   GetFieldBuilder.Map(x=>x.UserId),
                   GetFieldBuilder.Map(x=>x.UserName),
                   GetFieldBuilder.Map(x=>x.UserImage),

                   GetFieldBuilder.Map(x=>x.AnswerCount).IsFilterable().IsFacetable(),
                   GetFieldBuilder.Map(x=>x.FilesCount),
                   GetFieldBuilder.Map(x=>x.DateTime).IsSortable(),
                   GetFieldBuilder.Map(x=>x.HasCorrectAnswer).IsFilterable(),
                   GetFieldBuilder.Map(x=>x.Price),
                   GetFieldBuilder.Map(x=>x.Text).IsSearchable(),
                   GetFieldBuilder.Map(x=>x.Color),
                   GetFieldBuilder.Map(x=>x.Subject).IsFilterable().IsFacetable(),
                   GetFieldBuilder.Map(x=>x.SubjectText).IsFilterable().IsFacetable(),

                }
            };
        }
    }
}
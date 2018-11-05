using Cloudents.Infrastructure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using Document = Cloudents.Core.Entities.Search.Document;

namespace Cloudents.Infrastructure.Write
{
    public class DocumentSearchWrite : SearchServiceWrite<Document>
    {
        internal const string IndexName = "document3";

        internal const string TagsCourseParameter = "Course";
        internal const string TagsUniversityParameter = "University";
        internal const string TagsTagsParameter = "Tag";
        internal const string ScoringProfile = "ScoringProfile";

        public DocumentSearchWrite(SearchService client) : base(client, client.GetClient(IndexName))
        {
        }



        protected override Index GetIndexStructure(string indexName)
        {
            var fieldBuilder = new FluentSearchFieldBuilder<Document>();

            return new Index()
            {
                Name = indexName,
                Fields = new List<Field>
                {

                   fieldBuilder.Map(x=>x.Id).IsKey(),
                   fieldBuilder.Map(x=>x.DateTime).IsSortable().IsFilterable(),
                   fieldBuilder.Map(x=>x.Content).IsSearchable(),
                   fieldBuilder.Map(x=>x.Name).IsSearchable(),
                   fieldBuilder.Map(x=>x.MetaContent),

                    fieldBuilder.Map(x=>x.Tags).IsSearchable().IsFilterable(),
                    fieldBuilder.Map(x=>x.Course).IsFilterable(),
                    fieldBuilder.Map(x=>x.Country).IsFilterable().IsFacetable(),
                    fieldBuilder.Map(x=>x.Language).IsFilterable(),
                    fieldBuilder.Map(x=>x.University).IsFilterable(),
                    fieldBuilder.Map(x=>x.Type).IsFilterable().IsFacetable()
                },
                ScoringProfiles = new List<ScoringProfile>
                {
                    new ScoringProfile(ScoringProfile)
                    {
                        TextWeights = new TextWeights(new Dictionary<string, double>
                        {
                            [nameof(Document.Name)] = 3,
                            [nameof(Document.Tags)] = 2,
                            [nameof(Document.Content)] = 1,
                        }),
                        FunctionAggregation = ScoringFunctionAggregation.Sum,
                        Functions = new List<ScoringFunction>
                        {
                            new TagScoringFunction(nameof(Document.Course),8, new TagScoringParameters(TagsCourseParameter)),
                            new TagScoringFunction(nameof(Document.University),6, new TagScoringParameters(TagsUniversityParameter)),
                            new TagScoringFunction(nameof(Document.Tags),4, new TagScoringParameters(TagsTagsParameter)),
                        }
                    }
                },
            };
        }
    }
}
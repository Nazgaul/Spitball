using Cloudents.Core.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;

namespace Cloudents.Search.Document
{
    public class DocumentSearchWrite : SearchServiceWrite<Entities.Document>
    {
        public const string IndexName = "document3";

        internal const string TagsCourseParameter = "Course";
        internal const string TagsUniversityParameter = "University";
        internal const string TagsCountryParameter = "Country";
        //internal const string TagsTagsParameter = "Tag";
        internal const string ScoringProfile = "ScoringProfile2";

        public DocumentSearchWrite(SearchService client, ILogger logger) : base(client, client.GetClient(IndexName), logger)
        {
        }



        protected override Index GetIndexStructure(string indexName)
        {
            var index = new Index
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<Entities.Document>(new SearchIndexEnumToIntContractResolver()),
                ScoringProfiles = new List<ScoringProfile>
                {
                    //new ScoringProfile("ScoringProfile")
                    //{
                    //    TextWeights = new TextWeights(new Dictionary<string, double>
                    //    {
                    //        [nameof(Entities.Document.Name)] = 4,
                    //        [nameof(Entities.Document.Tags)] = 3.5,
                    //        [nameof(Entities.Document.Content)] = 3,
                    //    }),
                    //    FunctionAggregation = ScoringFunctionAggregation.Sum,
                    //    Functions = new List<ScoringFunction>
                    //    {
                    //        new TagScoringFunction(Entities.Document.CourseNameField,3.2, new TagScoringParameters(TagsCourseParameter)),
                    //        new TagScoringFunction(Entities.Document.UniversityIdFieldName,3, new TagScoringParameters(TagsUniversityParameter)),
                    //        new TagScoringFunction(nameof(Entities.Document.Tags),1.5, new TagScoringParameters("Tag")),
                    //    }
                    //},
                    new ScoringProfile(ScoringProfile)
                    {
                    TextWeights = new TextWeights(new Dictionary<string, double>
                    {
                    [nameof(Entities.Document.Name)] = 4,
                    [Entities.Document.CourseNameField] = 3.2,
                    [nameof(Entities.Document.Content)] = 3,
                }),
                FunctionAggregation = ScoringFunctionAggregation.Sum,
                Functions = new List<ScoringFunction>
                {
                    new TagScoringFunction(Entities.Document.CourseNameField,3.2, new TagScoringParameters(TagsCourseParameter)),
                    new TagScoringFunction("University",3, new TagScoringParameters(TagsUniversityParameter)),
                    new TagScoringFunction(nameof(Entities.Document.Country),1.5, new TagScoringParameters(TagsCountryParameter)),
                }
            }
                },
            };
            index.Fields.Add(new Field("MetaContent", DataType.String));
            index.Fields.Add(new Field("Language", DataType.String)
            {
                IsFilterable = true
            });
            index.Fields.Add(new Field("Course", DataType.String)
            {
                IsFilterable = true
            });
            index.Fields.Add(new Field("Type", DataType.Int32)
            {
                IsFilterable = true,
                IsFacetable = true
            });
            return index;
        }
    }
}
//using System;
//using Cloudents.Core.Interfaces;
//using Microsoft.Azure.Search;
//using Microsoft.Azure.Search.Models;
//using System.Collections.Generic;
//using Index = Microsoft.Azure.Search.Models.Index;

//namespace Cloudents.Search.Document
//{
//    public class DocumentSearchWrite : SearchServiceWrite<Entities.Document>
//    {
//        public const string IndexName = "document3";

//        internal const string TagsCourseParameter = "Course";
//        internal const string TagsVideoParameter = "Video";
//        internal const string ScoringProfile = "ScoringProfile3";

//        public DocumentSearchWrite(SearchService client, ILogger logger) : base(client, client.GetClient(IndexName), logger)
//        {
//        }



//        protected override Index GetIndexStructure(string indexName)
//        {
//            var index = new Index
//            {
//                Name = indexName,
//                //Field builder return ToArray and we add more data.
//                Fields = new List<Field>( FieldBuilder.BuildForType<Entities.Document>(new SearchIndexEnumToIntContractResolver())),
//                ScoringProfiles = new List<ScoringProfile>
//                {

//                    new ScoringProfile("ScoringProfile2")
//                    {
//                        TextWeights = new TextWeights(new Dictionary<string, double>
//                        {
//                            [nameof(Entities.Document.Name)] = 4,
//                            [Entities.Document.CourseNameField] = 3.2,
//                            [nameof(Entities.Document.Content)] = 3,
//                        }),
//                        FunctionAggregation = ScoringFunctionAggregation.Sum,
//                        Functions = new List<ScoringFunction>
//                        {
//                            new TagScoringFunction(Entities.Document.CourseNameField, 3.2,
//                                new TagScoringParameters(TagsCourseParameter)),
//                            new TagScoringFunction("University", 3, new TagScoringParameters("University")),
//                            new TagScoringFunction("Country", 1.5,
//                                new TagScoringParameters("Country")),
//                        }
//                    },
//                    new ScoringProfile(ScoringProfile)
//                    {
//                        TextWeights = new TextWeights(new Dictionary<string, double>
//                        {
//                            [nameof(Entities.Document.Name)] = 4,
//                            [Entities.Document.CourseNameField] = 3.2,
//                            [nameof(Entities.Document.Content)] = 3,
//                        }),
//                        FunctionAggregation = ScoringFunctionAggregation.Sum,
//                        Functions = new List<ScoringFunction>
//                        {
//                            new FreshnessScoringFunction(nameof(Entities.Document.DateTime),20,TimeSpan.FromDays(365)),
//                            new TagScoringFunction(Entities.Document.CourseNameField, 3.2,
//                                new TagScoringParameters(TagsCourseParameter)),

//                            new TagScoringFunction(Entities.Document.TypeFieldName, 2, new TagScoringParameters(TagsVideoParameter))
//                        }
//                    }
//                }
//            };
//            index.Fields.Add(new Field("MetaContent", DataType.String));
//            index.Fields.Add(new Field("Language", DataType.String)
//            {
//                IsFilterable = true
//            });
//            index.Fields.Add(new Field("Course", DataType.String)
//            {
//                IsFilterable = true
//            });
//            index.Fields.Add(new Field("University",DataType.String)
//            {
//                IsFilterable = true
//            });
//            index.Fields.Add(new Field("University2",DataType.String)
//            {
//                IsFilterable = true,
//                IsSearchable = true
//            });
//            index.Fields.Add(new Field("Type", DataType.Int32)
//            {
//                IsFilterable = true,
//                IsFacetable = true
//            });
//            return index;
//        }
//    }
//}
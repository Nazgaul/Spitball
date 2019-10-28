using System.Collections.Generic;
using Cloudents.Core.Interfaces;
using Cloudents.Search.Document;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Search.Tutor
{
    public class TutorSearchWrite :SearchServiceWrite<Entities.Tutor>
    {

        public const string IndexName = "tutor";
        internal const string ScoringProfile = "ScoringProfile2";

        public TutorSearchWrite(SearchService client, ILogger logger) : base(client, client.GetClient(IndexName), logger)
        {
        }

        protected override Index GetIndexStructure(string indexName)
        {
            var fields = new List<Field>();
            var index = new Index
            {
                Name = indexName,
               // Fields = FieldBuilder.BuildForType<Entities.Tutor>(new SearchIndexEnumToIntContractResolver()),
                Analyzers = new List<Analyzer>
                {
                    new CustomAnalyzer("prefix",TokenizerName.Standard,new List<TokenFilterName>
                    {
                        TokenFilterName.Lowercase,
                        TokenFilterName.Stopwords,
                        "my_edgeNGram"
                        //TokenFilterName.Create("my_edgeNGram")
                    }),
                },
                TokenFilters = new List<TokenFilter>
                {
                    new EdgeNGramTokenFilterV2("my_edgeNGram",2,20)
                },
                ScoringProfiles = new List<ScoringProfile>
                {
                    new ScoringProfile("ScoringProfile")
                    {
                        TextWeights = new TextWeights(new Dictionary<string, double>
                        {
                            [nameof(Entities.Tutor.Courses)] =2,
                            [nameof(Entities.Tutor.Subjects)] =1.2,
                            [nameof(Entities.Tutor.Prefix)] = 0.9,
                        }),
                        Functions = new List<ScoringFunction>
                        {
                            new MagnitudeScoringFunction(Entities.Tutor.RateFieldName,100,0,5)
                        }
                    },
                    new ScoringProfile(ScoringProfile)
                    {
                        TextWeights = new TextWeights(new Dictionary<string, double>
                        {
                            [nameof(Entities.Tutor.Courses)] =2,
                            [nameof(Entities.Tutor.Subjects)] =1.2,
                            [nameof(Entities.Tutor.Prefix)] = 0.9,
                        }),
                        Functions = new List<ScoringFunction>
                        {
                            new MagnitudeScoringFunction(nameof(Entities.Tutor.OverAllRating),100,0,5)
                        }
                    },
                },

            };
            fields.Add(new Field("Rate", DataType.Double));
            fields.Add(new Field("Price", DataType.Double));
            fields.Add(new Field("Image", DataType.String));
            fields.Add(new Field("Bio", DataType.String)
            {
                IsSearchable = true
            });
            fields.Add(new Field("ReviewCount", DataType.Int32));
            fields.AddRange(FieldBuilder.BuildForType<Entities.Tutor>(new SearchIndexEnumToIntContractResolver()));
            index.Fields = fields;
            return index;
        }
    }
}
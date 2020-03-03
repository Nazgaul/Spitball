using Cloudents.Core.Interfaces;
using Cloudents.Search.Document;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;

namespace Cloudents.Search.University
{
    public class UniversitySearchWrite : SearchServiceWrite<Entities.University>
    {
        public const string IndexName = "universities5";
        public const string ScoringProfile = "university-default";

        public const string CountryTagScoringParameters = "country";

        internal const string SuggesterName = "sg";
        //public const string DistanceScoringParameter = "currentLocation";

        public UniversitySearchWrite(SearchService client, ILogger logger)
            : base(client, client.GetClient(IndexName), logger)
        {
        }

        protected override Index GetIndexStructure(string indexName)
        {
            //var stopWordsList = new[]{ "university",
            //        "of",
            //        "college",
            //        "school",
            //        "the",
            //        "a",
            //    "המכללה","אוניברסיטת","מכללת","אוניברסיטה","ה"
            //};
            var index = new Index
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<Entities.University>(new SearchIndexEnumToIntContractResolver()),
                Analyzers = new List<Analyzer>
                {
                    new CustomAnalyzer("prefix",TokenizerName.Standard,new List<TokenFilterName>
                    {
                        TokenFilterName.Lowercase,
                        TokenFilterName.Stopwords,
                        //TokenFilterName.Create("my_stopWords"),
                        "my_edgeNGram"
                        //TokenFilterName.Create("my_edgeNGram")
                    }),
                   // new StandardAnalyzer("stopWords",stopwords:stopWordsList)
                },
                TokenFilters = new List<TokenFilter>
                {
                   // new StopwordsTokenFilter("my_stopWords",stopWordsList,ignoreCase:true),
                    new EdgeNGramTokenFilterV2("my_edgeNGram",2,20)
                },
                ScoringProfiles = new List<ScoringProfile>
                {
                    new ScoringProfile(ScoringProfile)
                    {
                        TextWeights = new TextWeights(new Dictionary<string, double>
                        {
                            [nameof(Entities.University.Extra)] = 3,
                            [nameof(Entities.University.Name)] = 2.5,
                            [nameof(Entities.University.Prefix)] = 2,

                        }),

                        FunctionAggregation = ScoringFunctionAggregation.Sum,
                        Functions = new List<ScoringFunction>
                        {
                            new TagScoringFunction(nameof(Entities.University.Country),1.5, new TagScoringParameters(CountryTagScoringParameters))
                        //    new DistanceScoringFunction(
                        //        nameof(Entities.University.GeographyPoint),
                        //        5,DistanceScoringParameter,10,ScoringFunctionInterpolation.Linear),

                        }
                    }
                },
                Suggesters = new List<Suggester>
                {
                    new Suggester(SuggesterName,
                        nameof(Entities.University.Extra),
                        nameof(Entities.University.Name))
                }
            };

            index.Fields.Add(new Field("UsersCount", DataType.Int32));
            return index;
        }


        //public  Task DeleteOldDataAsync(DateTime timeToDelete, CancellationToken token)
        //{
        //    return DeleteOldDataAsync(nameof(Entities.University.InsertDate), timeToDelete, token);

        //}

    }
}
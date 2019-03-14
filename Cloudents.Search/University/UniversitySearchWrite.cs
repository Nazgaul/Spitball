using System.Collections.Generic;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Search.University
{
    [UsedImplicitly]
    public class UniversitySearchWrite : SearchServiceWrite<Entities.University>
    {
        public const string IndexName = "universities5";
        public const string ScoringProfile = "university-default";

        public const string CountryTagScoringParameters = "country";

        internal const string SuggesterName = "sg";
        //public const string DistanceScoringParameter = "currentLocation";

        public UniversitySearchWrite(SearchService client, ILogger logger)
            : base(client, client.GetClient(IndexName),logger)
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
            return new Index
            {
                Name = indexName,
                Fields = new List<Field>
                {
                    new Field(nameof(Entities.University.Id), DataType.String)
                    {
                        IsKey = true
                    },
                    new Field(nameof(Entities.University.Name), DataType.String)
                    {
                        IsSearchable = true
                        
                        //SearchAnalyzer = AnalyzerName.StandardLucene,
                        //IndexAnalyzer =  AnalyzerName.Create("stopWords")
                    },
                    new Field(nameof(Entities.University.DisplayName), DataType.String)
                    {
                        IsRetrievable = true,
                        IsSortable = true

                        //IndexAnalyzer =  AnalyzerName.Create("stopWords")
                    },
                    new Field(nameof(Entities.University.Prefix), DataType.Collection(DataType.String))
                    {
                        IsSearchable = true,
                        SearchAnalyzer = AnalyzerName.StandardLucene,
                        IndexAnalyzer = AnalyzerName.Create("prefix")
                        
                    },
                    new Field(nameof(Entities.University.Extra), DataType.String)
                    {
                        IsSearchable = true
                       
                        //SearchAnalyzer = AnalyzerName.StandardLucene,
                        //IndexAnalyzer =  AnalyzerName.Create("stopWords"),

                    },
                    new Field(nameof(Entities.University.Country), DataType.String)
                    {
                        IsFilterable = true
                    }
                },
                Analyzers = new List<Analyzer>
                {
                    new CustomAnalyzer("prefix",TokenizerName.Standard,new List<TokenFilterName>
                    {
                        TokenFilterName.Lowercase,
                        TokenFilterName.Stopwords,
                        //TokenFilterName.Create("my_stopWords"),
                        TokenFilterName.Create("my_edgeNGram")
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
        }
    }
}
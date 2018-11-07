﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Search;
using JetBrains.Annotations;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write
{
    [UsedImplicitly]
    public class UniversitySearchWrite : SearchServiceWrite<Core.Entities.Search.University>
    {
        public const string IndexName = "universities5";
        public const string ScoringProfile = "university-default";

        public const string CountryTagScoringParameters = "country";
        //public const string DistanceScoringParameter = "currentLocation";

        public UniversitySearchWrite(SearchService client, ILogger logger)
            : base(client, client.GetClient(IndexName),logger)
        {
        }


      

        public override async Task CreateOrUpdateAsync(CancellationToken token)
        {
            // _synonymWrite.CreateEmpty(SynonymName);
            //await Client.Indexes.DeleteAsync(IndexClient.IndexName, cancellationToken: token).ConfigureAwait(false);
            //var synonymMap = new SynonymMap()
            //{
            //    Name = "university-synonymmap",
            //    Format = "solr",
            //    Synonyms = @"
            //    university, of, college, school, the, a, המכללה,אוניברסיטת,מכללת,אוניברסיטה,ה=> "
            //};

            //Client.SynonymMaps.CreateOrUpdate(synonymMap);

            await base.CreateOrUpdateAsync(token).ConfigureAwait(false);
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
                    new Field(nameof(Core.Entities.Search.University.Id), DataType.String)
                    {
                        IsKey = true
                    },
                    new Field(nameof(Core.Entities.Search.University.Name), DataType.String)
                    {
                        IsSearchable = true
                        
                        //SearchAnalyzer = AnalyzerName.StandardLucene,
                        //IndexAnalyzer =  AnalyzerName.Create("stopWords")
                    },
                    new Field(nameof(Core.Entities.Search.University.DisplayName), DataType.String)
                    {
                        IsRetrievable = true,
                        IsSortable = true

                        //IndexAnalyzer =  AnalyzerName.Create("stopWords")
                    },
                    new Field(nameof(Core.Entities.Search.University.Prefix), DataType.Collection(DataType.String))
                    {
                        IsSearchable = true,
                        SearchAnalyzer = AnalyzerName.StandardLucene,
                        IndexAnalyzer = AnalyzerName.Create("prefix")
                        
                    },
                    new Field(nameof(Core.Entities.Search.University.Extra), DataType.String)
                    {
                        IsSearchable = true
                       
                        //SearchAnalyzer = AnalyzerName.StandardLucene,
                        //IndexAnalyzer =  AnalyzerName.Create("stopWords"),

                    },
                    new Field(nameof(Core.Entities.Search.University.Country), DataType.String)
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
                            [nameof(Core.Entities.Search.University.Extra)] = 3,
                            [nameof(Core.Entities.Search.University.Name)] = 2,
                            [nameof(Core.Entities.Search.University.Prefix)] = 1,

                        }),

                        FunctionAggregation = ScoringFunctionAggregation.Sum,
                        Functions = new List<ScoringFunction>
                        {
                            new TagScoringFunction(nameof(Core.Entities.Search.University.Country),5, new TagScoringParameters(CountryTagScoringParameters))
                        //    new DistanceScoringFunction(
                        //        nameof(Core.Entities.Search.University.GeographyPoint),
                        //        5,DistanceScoringParameter,10,ScoringFunctionInterpolation.Linear),

                        }
                    }
                },
                Suggesters = new List<Suggester>
                {
                    new Suggester("sg",
                        nameof(Core.Entities.Search.University.Extra),
                        nameof(Core.Entities.Search.University.Name))
                }
            };
        }
    }
}
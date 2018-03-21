using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write
{
    [UsedImplicitly]
    public class UniversitySearchWrite : SearchServiceWrite<Core.Entities.Search.University>
    {
        public const string IndexName = "universities3";
        public const string ScoringProfile = "university-default";
        public const string DistanceScoringParameter = "currentLocation";

        public UniversitySearchWrite(SearchServiceClient client)
            : base(client, IndexName)
        {
        }

        public override async Task CreateOrUpdateAsync(CancellationToken token)
        {
            // _synonymWrite.CreateEmpty(SynonymName);
            await Client.Indexes.DeleteAsync(IndexName, cancellationToken: token).ConfigureAwait(false);
            await base.CreateOrUpdateAsync(token).ConfigureAwait(false);
        }

        protected override Index GetIndexStructure(string indexName)
        {
            var stopWordsList = new[]{ "university",
                    "of",
                    "college",
                    "school",
                    "the",
                    "a"};
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
                        IsSearchable = true,
                        IsSortable = true,
                        SearchAnalyzer = AnalyzerName.StandardLucene,
                        IndexAnalyzer =  AnalyzerName.Create("stopWords")
                    },
                    new Field(nameof(Core.Entities.Search.University.Prefix), DataType.Collection(DataType.String))
                    {
                        IsSearchable = true,
                        SearchAnalyzer = AnalyzerName.StandardLucene,
                        IndexAnalyzer = AnalyzerName.Create("prefix"),
                    },
                    new Field(nameof(Core.Entities.Search.University.Extra), DataType.String)
                    {
                        IsSearchable = true,
                        SearchAnalyzer = AnalyzerName.StandardLucene,
                        IndexAnalyzer =  AnalyzerName.Create("stopWords")
                    },
                    new Field(nameof(Core.Entities.Search.University.Image), DataType.String),

                    new Field(nameof(Core.Entities.Search.University.GeographyPoint), DataType.GeographyPoint)
                    {
                        IsSortable = true,
                        IsFilterable = true
                    }
                },
                Analyzers = new List<Analyzer>
                {
                    new CustomAnalyzer("prefix",TokenizerName.Standard,new List<TokenFilterName>
                    {
                        TokenFilterName.Lowercase,
                        TokenFilterName.Stopwords,
                        TokenFilterName.Create("my_stopWords"),
                        TokenFilterName.Create("my_edgeNGram")
                    }),
                    new StandardAnalyzer("stopWords",stopwords:stopWordsList)
                },
                TokenFilters = new List<TokenFilter>
                {
                    new StopwordsTokenFilter("my_stopWords",stopWordsList,ignoreCase:true),
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
                            new DistanceScoringFunction(
                                nameof(Core.Entities.Search.University.GeographyPoint),
                                5,DistanceScoringParameter,10,ScoringFunctionInterpolation.Linear),

                        }
                    }
                }
                //Suggesters = new List<Suggester>
                //{
                //    new Suggester("sg",
                //        nameof(Core.Entities.Search.University.Extra),
                //        nameof(Core.Entities.Search.University.Name))
                //}
            };
        }
    }
}
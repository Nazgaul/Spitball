using System.Collections.Generic;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write
{
    public class UniversitySearchWrite : SearchServiceWrite<Core.Entities.Search.University>
    {
        public const string IndexName = "universities3";
        public const string ScoringProfile = "university-default";
        public const string SynonymName = "university-synonym";
        private readonly ISynonymWrite _synonymWrite;
        public UniversitySearchWrite(SearchServiceClient client, ISynonymWrite synonymWrite)
            : base(client, IndexName)
        {
            _synonymWrite = synonymWrite;

        }

        public override void Start()
        {
            // _synonymWrite.CreateEmpty(SynonymName);
            Client.Indexes.Delete(IndexName);
            base.Start();
        }

        protected override Index GetIndexStructure(string indexName)
        {
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
                        Analyzer =  AnalyzerName.Create("stopWords")
                        //SynonymMaps = new []{ SynonymName }
                    },
                    new Field(nameof(Core.Entities.Search.University.Prefix), DataType.String)
                    {
                        IsSearchable = true,
                        //Analyzer = AnalyzerName.EnMicrosoft,
                        SearchAnalyzer = AnalyzerName.StandardLucene,
                        IndexAnalyzer = AnalyzerName.Create("prefix"),
                        //SynonymMaps = new []{ SynonymName }
                    },
                    new Field(nameof(Core.Entities.Search.University.Extra), DataType.String)
                    {
                        IsSearchable = true,
                        Analyzer = AnalyzerName.Create("stopWords")
                        //SynonymMaps = new []{ SynonymName }
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
                    new CustomAnalyzer("prefix",TokenizerName.MicrosoftLanguageTokenizer,new List<TokenFilterName>
                    {
                        TokenFilterName.Create("my_edgeNGram")
                    }),
                    new StandardAnalyzer("stopWords",stopwords:new []
                    {
                        "university",
                        "of",
                        "college",
                        "school"
                    })
                },
                TokenFilters = new List<TokenFilter>
                {
                    new EdgeNGramTokenFilterV2("my_edgeNGram",2,20)
                },
                ScoringProfiles = new List<ScoringProfile>
                {
                    new ScoringProfile("university-default")
                    {
                        TextWeights = new TextWeights(new Dictionary<string, double>()
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
                                5,"currentLocation",50,ScoringFunctionInterpolation.Linear),

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
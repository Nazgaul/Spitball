using System.Collections.Generic;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write
{
    public class AutoCompleteSearchWrite : SearchServiceWrite<Core.Entities.Search.AutoComplete>
    {
        public const string IndexName = "auto-complete";
        public const string ScoringProfile = "auto-complete-default";
        public AutoCompleteSearchWrite(SearchServiceClient client)
            : base(client, IndexName)
        {
        }

        protected override Index GetIndexStructure(string indexName)
        {
            return new Index
            {
                Name = indexName,
                Fields = new List<Field>
                {
                    new Field(nameof(Core.Entities.Search.AutoComplete.Id), DataType.String)
                    {
                        IsKey = true
                    },
                    new Field(nameof(Core.Entities.Search.AutoComplete.Key), DataType.String)
                    {
                        IsSearchable = true,
                        Analyzer = AnalyzerName.StandardLucene,
                    },
                    new Field(nameof(Core.Entities.Search.AutoComplete.Prefix), DataType.String)
                    {
                        IsSearchable = true,
                        SearchAnalyzer = AnalyzerName.StandardLucene,
                        IndexAnalyzer = AnalyzerName.Create("prefix"),
                    },
                    new Field(nameof(Core.Entities.Search.AutoComplete.Value), DataType.String)
                    {
                        IsSearchable = true,
                        Analyzer = AnalyzerName.StandardLucene,
                    },
                    new Field(nameof(Core.Entities.Search.AutoComplete.Vertical), DataType.Int32)
                    {
                        IsFilterable = true
                    }
                },
                Analyzers = new List<Analyzer>
                {
                    new CustomAnalyzer("prefix",TokenizerName.Standard,new List<TokenFilterName>
                    {
                        TokenFilterName.Lowercase,
                        TokenFilterName.Create("my_edgeNGram")
                    }),
                },
                TokenFilters = new List<TokenFilter>
                {
                    new EdgeNGramTokenFilterV2("my_edgeNGram",2,20)
                },
                ScoringProfiles = new List<ScoringProfile>
                {
                    new ScoringProfile(ScoringProfile)
                    {
                        TextWeights = new TextWeights(new Dictionary<string, double>
                        {
                            [nameof(Core.Entities.Search.AutoComplete.Key)] = 2,
                            [nameof(Core.Entities.Search.AutoComplete.Prefix)] = 1,
                        })
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
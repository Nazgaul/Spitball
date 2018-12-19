using System.Collections.Generic;
using Cloudents.Application.Entities.Search;
using Cloudents.Application.Interfaces;
using Cloudents.Infrastructure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write
{
    public class AutoCompleteSearchWrite : SearchServiceWrite<AutoComplete>
    {
        public const string IndexName = "auto-complete";
        public const string ScoringProfile = "auto-complete-default";

        public AutoCompleteSearchWrite(SearchService client, ILogger logger)
            : base(client, IndexName,logger)
        {
        }

        protected override Index GetIndexStructure(string indexName)
        {
            return new Index
            {
                Name = indexName,
                Fields = new List<Field>
                {
                    new Field(nameof(AutoComplete.Id), DataType.String)
                    {
                        IsKey = true
                    },
                    new Field(nameof(AutoComplete.Key), DataType.String)
                    {
                        IsSearchable = true,
                        Analyzer = AnalyzerName.StandardLucene,
                    },
                    new Field(nameof(AutoComplete.Prefix), DataType.String)
                    {
                        IsSearchable = true,
                        SearchAnalyzer = AnalyzerName.StandardLucene,
                        IndexAnalyzer = AnalyzerName.Create("prefix"),
                    },
                    new Field(nameof(AutoComplete.Value), DataType.String)
                    {
                        IsSearchable = true,
                        Analyzer = AnalyzerName.StandardLucene,
                    },
                    new Field(nameof(AutoComplete.Vertical), DataType.Int32)
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
                            [nameof(AutoComplete.Key)] = 2,
                            [nameof(AutoComplete.Prefix)] = 1,
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
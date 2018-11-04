using Cloudents.Core.Entities.Search;
using Cloudents.Infrastructure.Search;
using JetBrains.Annotations;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;

namespace Cloudents.Infrastructure.Write
{
    [UsedImplicitly]
    public class QuestionSearchWrite : SearchServiceWrite<Question>
    {
        internal const string IndexName = "question2";
        internal const string TagsCountryParameter = "Country";
        internal const string ScoringProfile = "ScoringProfile";


        public QuestionSearchWrite(SearchService client) : base(client, client.GetClient(IndexName))
        {
        }

        protected override Index GetIndexStructure(string indexName)
        {
            //_fieldBuilder.Name(indexName)
            //    .Fields().Map(x => x.Id).IsKey();
            var fieldBuilder = new FluentSearchFieldBuilder<Question>();

            return new Index()
            {
                Name = indexName,
                Fields = new List<Field>
                {

                   fieldBuilder.Map(x=>x.Id).IsKey(),
                   fieldBuilder.Map(x=>x.DateTime).IsSortable().IsFilterable(),
                   fieldBuilder.Map(x=>x.Text).IsSearchable(),
                   fieldBuilder.Map(x=>x.Subject).IsFilterable().IsFacetable(),

                   fieldBuilder.Map(x=>x.Country).IsFilterable(),
                   fieldBuilder.Map(x=>x.Language).IsFilterable(),


                   fieldBuilder.Map(x=>x.State).IsFilterable().IsFacetable(),
                   fieldBuilder.Map(x=>x.Prefix).IsSearchable().WithIndexAnalyzer(AnalyzerName.Create("prefix"))
                       .WithSearchAnalyzer(AnalyzerName.StandardLucene),

                },
                Analyzers = new List<Analyzer>
                {
                    new CustomAnalyzer("prefix",TokenizerName.Standard,new List<TokenFilterName>
                    {
                        TokenFilterName.Lowercase,
                        TokenFilterName.Stopwords,
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
                            [nameof(Question.Text)] = 2,
                            [nameof(Question.Prefix)] = 1,
                        }),
                        FunctionAggregation = ScoringFunctionAggregation.Sum,
                        Functions = new List<ScoringFunction>
                        {
                            new FreshnessScoringFunction(nameof(Question.DateTime),4,TimeSpan.FromHours(6)),
                            new TagScoringFunction(nameof(Question.Country),4, new TagScoringParameters(TagsCountryParameter)),
                        }
                    }
                },
            };
        }
    }
}
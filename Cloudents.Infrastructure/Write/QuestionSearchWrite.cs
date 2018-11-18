using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
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


        public QuestionSearchWrite(SearchService client, ILogger logger) : base(client, client.GetClient(IndexName), logger)
        {
        }

        protected override Index GetIndexStructure(string indexName)
        {
            var fieldBuilder = new FluentSearchFieldBuilder<Question>();

            return new Index
            {
                Name = indexName,
                Fields = new List<Field>
                {
                    new Field("UserId",DataType.Int64)
                    {
                        IsFilterable = true
                    },
                    new Field("UserName",DataType.String),
                    new Field("UserImage",DataType.String),
                    new Field("AnswerCount",DataType.Int32),
                    new Field("FilesCount",DataType.Int32),
                    new Field("HasCorrectAnswer",DataType.Boolean),
                    new Field("Price",DataType.Double),
                    new Field("Color",DataType.Int32),
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
                            [nameof(Question.Text)] = 3,
                            [nameof(Question.Prefix)] = 2,
                        }),
                        FunctionAggregation = ScoringFunctionAggregation.Sum,
                        Functions = new List<ScoringFunction>
                        {
                            new FreshnessScoringFunction(nameof(Question.DateTime),1.5,TimeSpan.FromHours(6)),
                            new TagScoringFunction(nameof(Question.Country),1.5, new TagScoringParameters(TagsCountryParameter)),
                        }
                    }
                },
            };
        }
    }
}
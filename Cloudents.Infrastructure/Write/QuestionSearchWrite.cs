using System;
using Cloudents.Core.Entities.Search;
using Cloudents.Infrastructure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Write
{
    [UsedImplicitly]
    public class QuestionSearchWrite : SearchServiceWrite<Question>
    {
        private readonly FluentSearchFieldBuilder<Question> _fieldBuilder = new FluentSearchFieldBuilder<Question>();
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

            return new Index()
            {
                Name = indexName,
                Fields = new List<Field>
                {

                   _fieldBuilder.Map(x=>x.Id).IsKey(),
                   //_fieldBuilder.Map(x=>x.UserId).IsFilterable(),
                   //_fieldBuilder.Map(x=>x.UserName),
                   //_fieldBuilder.Map(x=>x.UserImage),
                   //_fieldBuilder.Map(x=>x.AnswerCount),//.IsFilterable().IsFacetable(),
                   //_fieldBuilder.Map(x=>x.FilesCount),
                   _fieldBuilder.Map(x=>x.DateTime).IsSortable().IsFilterable(),
                   //_fieldBuilder.Map(x=>x.HasCorrectAnswer),//.IsFilterable(),
                   //_fieldBuilder.Map(x=>x.Price),
                   _fieldBuilder.Map(x=>x.Text).IsSearchable(),
                   //_fieldBuilder.Map(x=>x.Color),
                   _fieldBuilder.Map(x=>x.Subject).IsFilterable().IsFacetable(),

                   _fieldBuilder.Map(x=>x.Country).IsFilterable(),
                   _fieldBuilder.Map(x=>x.Language).IsFilterable(),
                    

                   _fieldBuilder.Map(x=>x.State).IsFilterable().IsFacetable(),
                   _fieldBuilder.Map(x=>x.Prefix).IsSearchable().WithIndexAnalyzer(AnalyzerName.Create("prefix"))
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
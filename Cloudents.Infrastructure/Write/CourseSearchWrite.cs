﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Infrastructure.Search;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write
{
    [UsedImplicitly]
    public class CourseSearchWrite : SearchServiceWrite<Core.Entities.Search.Course>
    {
        public const string IndexName = "course2";
        public const string ScoringProfile = "course-default";

        public CourseSearchWrite(SearchService client) : base(client, client.GetClient(IndexName))
        {
        }

        public override async Task CreateOrUpdateAsync(CancellationToken token)
        {
            await base.CreateOrUpdateAsync(token).ConfigureAwait(false);
        }

        protected override Index GetIndexStructure(string indexName)
        {
            return new Index
            {
                Name = indexName,
                Fields = new List<Field>
                {
                    new Field(nameof(Core.Entities.Search.Course.Id), DataType.String)
                    {
                        IsKey = true
                    },
                    new Field(nameof(Core.Entities.Search.Course.Name), DataType.String)
                    {
                        IsSearchable = true,
                        IsSortable = true,
                        //SearchAnalyzer = AnalyzerName.StandardLucene,
                    },
                    new Field(nameof(Core.Entities.Search.Course.Code), DataType.String)
                    {
                        IsSearchable = true,
                        IsSortable = true,
                        //SearchAnalyzer = AnalyzerName.StandardLucene,
                    },
                    new Field(nameof(Core.Entities.Search.Course.Prefix), DataType.Collection(DataType.String))
                    {
                        IsSearchable = true,
                        SearchAnalyzer = AnalyzerName.StandardLucene,
                        IndexAnalyzer = AnalyzerName.Create("prefix"),
                    },
                    new Field(nameof(Core.Entities.Search.Course.UniversityId), DataType.Int64)
                    {
                        //SearchAnalyzer = AnalyzerName.StandardLucene,
                        //IndexAnalyzer =  AnalyzerName.Create("stopWords")
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
                            [nameof(Core.Entities.Search.Course.Code)] = 3,
                            [nameof(Core.Entities.Search.Course.Name)] = 2,
                            [nameof(Core.Entities.Search.Course.Prefix)] = 1,

                        }),

                        FunctionAggregation = ScoringFunctionAggregation.Sum
                    }
                }
                //Suggesters = new List<Suggester>
                //{
                //    new Suggester("sg",
                //        nameof(Core.Entities.Search.Course.Extra),
                //        nameof(Core.Entities.Search.Course.Name))
                //}
            };
        }
    }
}

﻿using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using Cloudents.Search.Document;
using Microsoft.Azure.Search;

namespace Cloudents.Search.Question
{
    [UsedImplicitly]
    public class QuestionSearchWrite : SearchServiceWrite<Entities.Question>
    {
        public const string IndexName = "question2";
        internal const string TagsCountryParameter = "Country";
        //internal const string TagsCourseParameter = "Course";
        //internal const string TagsUniversityParameter = "University";
        internal const string TagsTagsParameter = "Tag";
        internal const string ScoringProfile = "ScoringProfile2";


        public QuestionSearchWrite(SearchService client, ILogger logger) : base(client, client.GetClient(IndexName), logger)
        {
        }

        protected override Index GetIndexStructure(string indexName)
        {

            var index = new Index
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<Entities.Question>(new SearchIndexEnumToIntContractResolver()),
                Analyzers = new List<Analyzer>
                {
                    new CustomAnalyzer("prefix",TokenizerName.Standard,new List<TokenFilterName>
                    {
                        TokenFilterName.Lowercase,
                        TokenFilterName.Stopwords,
                        "my_edgeNGram"
                        //TokenFilterName.Create("my_edgeNGram")
                    }),
                },
                TokenFilters = new List<TokenFilter>
                {
                    new EdgeNGramTokenFilterV2("my_edgeNGram",2,20)
                },
                ScoringProfiles = new List<ScoringProfile>
                {
                    new ScoringProfile("ScoringProfile2")
                    {
                        TextWeights = new TextWeights(new Dictionary<string, double>
                        {
                            [nameof(Entities.Question.Text)] = 185,
                            [nameof(Entities.Question.Prefix)] = 180,
                        }),
                        FunctionAggregation = ScoringFunctionAggregation.Sum,
                        Functions = new List<ScoringFunction>
                        {
                            new FreshnessScoringFunction(nameof(Entities.Question.DateTime),169.68,TimeSpan.FromDays(7*3),ScoringFunctionInterpolation.Linear),
                            new TagScoringFunction(nameof(Entities.Question.Country),1.01, new TagScoringParameters(TagsCountryParameter)),
                            new TagScoringFunction(nameof(Entities.Question.Tags),170, new TagScoringParameters("Tag")),
                        }
                    },
                    //new ScoringProfile(ScoringProfile)
                    //{
                    //    TextWeights = new TextWeights(new Dictionary<string, double>
                    //    {
                    //        [nameof(Entities.Question.Text)] = 185,
                    //        [nameof(Entities.Question.Prefix)] = 180,
                    //    }),
                    //    FunctionAggregation = ScoringFunctionAggregation.Sum,
                    //    Functions = new List<ScoringFunction>
                    //    {
                    //        new FreshnessScoringFunction(nameof(Entities.Question.DateTime),169.68,TimeSpan.FromDays(7*3),ScoringFunctionInterpolation.Linear),
                    //        new TagScoringFunction(nameof(Entities.Question.Country),1.01, new TagScoringParameters(TagsCountryParameter)),
                    //        new TagScoringFunction(nameof(Entities.Question.Course),170, new TagScoringParameters(TagsCourseParameter)),
                    //        new TagScoringFunction(nameof(Entities.Question.UniversityName),120, new TagScoringParameters(TagsUniversityParameter)),
                    //    }
                    //}
                },
            };

            index.Fields.Add(new Field("UserName", DataType.String));
            index.Fields.Add(new Field("UserImage", DataType.String));
            index.Fields.Add(new Field("AnswerCount", DataType.Int32));
            index.Fields.Add(new Field("FilesCount", DataType.Int32));
            index.Fields.Add(new Field("HasCorrectAnswer", DataType.Boolean));
            index.Fields.Add(new Field("Price", DataType.Double));
            index.Fields.Add(new Field("Color", DataType.Int32));
            index.Fields.Add(new Field("UserId", DataType.Int64)
                {
                    IsFilterable = true
                });
            return index;
        }
    }
}
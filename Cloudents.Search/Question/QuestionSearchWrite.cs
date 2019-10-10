using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.Azure.Search.Models;
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
            };

            index.Fields.Add(new Field("UserName", DataType.String));
            index.Fields.Add(new Field("UserImage", DataType.String));
            index.Fields.Add(new Field("AnswerCount", DataType.Int32));
            index.Fields.Add(new Field("FilesCount", DataType.Int32));
            index.Fields.Add(new Field("HasCorrectAnswer", DataType.Boolean));
            index.Fields.Add(new Field("Price", DataType.Double));
            index.Fields.Add(new Field("Color", DataType.Int32));
            index.Fields.Add(new Field("Subject", DataType.Int32)
            {
                IsFacetable = true,
                IsFilterable = true
            });
            index.Fields.Add(new Field("UserId", DataType.Int64)
                {
                    IsFilterable = true
                });
            index.Fields.Add(new Field("Tags", DataType.Collection(DataType.String))
            {
                IsFilterable = true,
                IsSearchable = true
            });
            return index;
        }
    }
}
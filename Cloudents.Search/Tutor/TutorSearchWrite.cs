using System.Collections.Generic;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Search.Tutor
{
    public class TutorSearchWrite :SearchServiceWrite<Entities.Tutor>
    {

        public const string IndexName = "tutor";
        private const string ScoringProfile = "ScoringProfile";

        public TutorSearchWrite(SearchService client, ILogger logger) : base(client, client.GetClient(IndexName), logger)
        {
        }

        protected override Index GetIndexStructure(string indexName)
        {
            var index = new Index
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<Entities.Tutor>(),
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
                    new ScoringProfile(ScoringProfile)
                    {
                        TextWeights = new TextWeights(new Dictionary<string, double>
                        {
                            [nameof(Entities.Tutor.Prefix)] = 0.9,
                        }),
                       
                    },
                },
            };
            return index;
        }
    }
}
using System.Collections.Generic;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write
{
    public class UniversitySearchWrite : SearchServiceWrite<Core.Entities.Search.University>
    {
        private const string IndexName = "universities3";
        public const string SynonymName = "university-synonym";
        private readonly ISynonymWrite _synonymWrite;
        public UniversitySearchWrite(SearchServiceClient client, ISynonymWrite synonymWrite)
            : base(client, IndexName)
        {
            _synonymWrite = synonymWrite;

            //client.SynonymMaps.Create()
        }

        //private SynonymMap CreateSynonym()
        //{

        //    return new SynonymMap("University-Synonym",SynonymMapFormat.Solr, );
        //}

        public override void Start()
        {
           // _synonymWrite.CreateEmpty(SynonymName);
           // base.Start();
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
                        Analyzer = AnalyzerName.EnMicrosoft,
                        SynonymMaps = new []{ SynonymName }
                    },
                    new Field(nameof(Core.Entities.Search.University.Image), DataType.String),
                    new Field(nameof(Core.Entities.Search.University.Extra), DataType.Collection(DataType.String))
                    {
                        IsSearchable = true,
                        SynonymMaps = new []{ SynonymName }
                    },
                    new Field(nameof(Core.Entities.Search.University.GeographyPoint), DataType.GeographyPoint)
                    {
                        IsSortable = true,
                        IsFilterable = true
                    }
                },
                ScoringProfiles = new List<ScoringProfile>
                {
                    new ScoringProfile("university-score-location")
                    {
                        FunctionAggregation = ScoringFunctionAggregation.Sum,
                        Functions = new List<ScoringFunction>
                        {
                            new DistanceScoringFunction(
                                nameof(Core.Entities.Search.University.GeographyPoint),
                                5,"currentLocation",50,ScoringFunctionInterpolation.Linear)
                        }
                    }
                },
                Suggesters = new List<Suggester>
                {
                    new Suggester("sg",
                        nameof(Core.Entities.Search.University.Extra),
                        nameof(Core.Entities.Search.University.Name))
                }
            };
        }
    }
}
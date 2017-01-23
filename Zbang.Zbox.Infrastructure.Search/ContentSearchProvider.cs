using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.Infrastructure.Search
{
    class ContentSearchProvider
    {
        private readonly ISearchConnection m_Connection;
        private readonly ISearchIndexClient m_IndexClient;
        private readonly string m_IndexName = "items";
        private bool m_CheckIndexExists;

        public ContentSearchProvider(ISearchConnection connection, ISearchIndexClient indexClient)
        {
            m_Connection = connection;
            if (m_Connection.IsDevelop)
            {
                m_IndexName = m_IndexName + "-dev";
            }
            m_IndexClient = connection.SearchClient.Indexes.GetClient(m_IndexName);
        }

        public async Task UpdateDataAsync(ItemSearchDto itemToUpload, IEnumerable<long> itemToDelete, CancellationToken token)
        {
            if (!m_CheckIndexExists)
            {
                await BuildIndexAsync();

            }
            if (itemToUpload != null)
            {
                var uploadBatch = new Item
                {
                    Id = itemToUpload.Id.ToString(),
                    Name = Path.GetFileNameWithoutExtension(itemToUpload.Name),
                    Course = itemToUpload.BoxName,
                    Professor = itemToUpload.BoxProfessor,
                    Code = itemToUpload.BoxCode,

                };
                switch (itemToUpload.Language)
                {
                    case Language.Undefined:
                        uploadBatch.Content = itemToUpload.Content;
                        break;
                    case Language.EnglishUs:
                        uploadBatch.ContentEn = itemToUpload.Content;
                        break;
                    case Language.Hebrew:
                        uploadBatch.ContentHe = itemToUpload.Content;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                var batch = IndexBatch.MergeOrUpload(new[] {uploadBatch});
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
            }
            if (itemToDelete != null)
            {
                var deleteBatch = itemToDelete.Select(s => new ItemSearch
                {
                    Id = s.ToString(CultureInfo.InvariantCulture)
                });
                var batch = IndexBatch.Delete(deleteBatch);
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
            }
        }

        private async Task BuildIndexAsync()
        {
            try
            {
                await m_Connection.SearchClient.Indexes.CreateOrUpdateAsync(GetIndexStructure());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on item build index", ex);
            }
            m_CheckIndexExists = true;
        }

        private Index GetIndexStructure()
        {
            var definition = new Index
            {
                Name = "items", Fields = FieldBuilder.BuildForType<Item>(), Suggesters = new List<Suggester>
                {
                    new Suggester
                    {
                        Name = "sg", SourceFields = new List<string>
                        {
                            nameof(Item.Course), nameof(Item.Code), nameof(Item.Professor)
                        }
                    }
                }
            };

            var weightProfile = new ScoringProfile("weight");
            var d = new Dictionary<string, double>
            {
                {nameof(Item.Tags), 10}, {nameof(Item.Content), 2}, {nameof(Item.ContentEn), 4}, {nameof(Item.ContentHe), 4},
            };
            var tagProfile = new ScoringProfile("tag");
            var tagFunction = new TagScoringFunction
            {
                Boost = 10,
                FieldName = nameof(Item.Tags)
            };
            tagProfile.Functions = new List<ScoringFunction> {tagFunction};

            weightProfile.TextWeights = new TextWeights(d);
            definition.ScoringProfiles = new List<ScoringProfile> {weightProfile, tagProfile };


            return definition;
        }
    }
}

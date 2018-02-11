using System;
using System.Net;
using System.Threading.Tasks;
using Cloudents.Core.Entities.DocumentDb;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Cloudents.Infrastructure.Data
{
    public class DocumentDbRepository<T> : IDocumentDbRepository<T> where T : class
    {
        private readonly DocumentClient _client;

        public DocumentDbRepository(DocumentDbRepositoryUnitOfWork client)
        {
            _client = client.Client;
        }
        
        private static string GetCollectionId()
        {
            return GetCollectionId(typeof(T));
        }

        private static string GetCollectionId(Type tt)
        {
            if (tt == null) throw new ArgumentNullException(nameof(tt));
            foreach (var attribute in tt.GetCustomAttributes(typeof(CollectionIdAttribute), true))
            {
                if (attribute is CollectionIdAttribute docAttribute)
                {
                    return docAttribute.CollectionId;
                }
            }
            throw new ArgumentException("no collection Id");
        }

        //public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        //{
        //    IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
        //            UriFactory.CreateDocumentCollectionUri(DocumentDbRepository.DatabaseId, GetCollectionId()))
        //        .Where(predicate)
        //        .AsDocumentQuery();

        //    List<T> results = new List<T>();
        //    while (query.HasMoreResults)
        //    {
        //        results.AddRange(await query.ExecuteNextAsync<T>().ConfigureAwait(false));
        //    }

        //    return results;
        //}

        //public async Task<Document> CreateItemAsync(T item)
        //{
        //    return await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, GetCollectionId()), item).ConfigureAwait(false);
        //}

        //public async Task<Document> UpdateItemAsync(string id, T item)
        //{
        //    return await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, GetCollectionId(), id), item).ConfigureAwait(false);
        //}

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DocumentDbRepositoryUnitOfWork.DatabaseId, GetCollectionId(), id)).ConfigureAwait(false);
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}

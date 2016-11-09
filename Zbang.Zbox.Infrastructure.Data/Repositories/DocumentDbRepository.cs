using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Data.Repositories
{
    
    public class DocumentDbRepository<T> : IDocumentDbRepository<T> where T : class
    {

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await DocumentDbUnitOfWork.Client.ReadDocumentAsync(DocumentDbUnitOfWork.BuildDocumentUri(GetCollectionId(), id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

        private static string GetCollectionId()
        {
            foreach (var attribute in typeof(T).GetCustomAttributes(true))
            {
                var docAttribute = attribute as DocumentDbModelAttribute;
                if (docAttribute != null)
                {
                    return docAttribute.CollectionId;
                }
            }
            throw new ArgumentException("no collection Id");

        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = DocumentDbUnitOfWork.Client.CreateDocumentQuery<T>(
                DocumentDbUnitOfWork.BuildCollectionUri(GetCollectionId()),
                //UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public Task CreateItemAsync(T item)
        {
            return DocumentDbUnitOfWork.Client.CreateDocumentAsync(
                DocumentDbUnitOfWork.BuildCollectionUri(GetCollectionId()), item);
        }

        public Task UpdateItemAsync(string id, T item)
        {
           return DocumentDbUnitOfWork.Client.ReplaceDocumentAsync(
               DocumentDbUnitOfWork.BuildDocumentUri(GetCollectionId(), id), item);
            //UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
        }

        public Task DeleteItemAsync(string id)
        {
            return DocumentDbUnitOfWork.Client.DeleteDocumentAsync(
                DocumentDbUnitOfWork.BuildDocumentUri(GetCollectionId(), id));
            //UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
        }

        //public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        //{
        //    IDocumentQuery<T> query = DocumentDbUnitOfWork.Client.CreateDocumentQuery<T>(
        //        DocumentDbUnitOfWork.BuildCollectionUri(typeof(T).Name))
        //        .Where(predicate)
        //        .AsDocumentQuery();
        //    List<T> results = new List<T>();
        //    while (query.HasMoreResults)
        //    {
        //        results.AddRange(await query.ExecuteNextAsync<T>());
        //    }
        //    //
        //    return results;
        //}

        //public async Task<IEnumerable<T>> GetItemsAsync(string sql)
        //{

        //    var query =
        //        DocumentDbUnitOfWork.Client.CreateDocumentQuery<T>(
        //            DocumentDbUnitOfWork.BuildCollectionUri(typeof (T).Name), sql).AsDocumentQuery();

        //    //return query.ToList();
        //    //IDocumentQuery<T> query = DocumentDbUnitOfWork.Client.CreateDocumentQuery<T>(
        //    //    DocumentDbUnitOfWork.BuildCollectionUri(typeof(T).Name))
        //    //    .Where(predicate)
        //    //    .AsDocumentQuery();
        //    List<T> results = new List<T>();
        //    while (query.HasMoreResults)
        //    {
        //        results.AddRange(await query.ExecuteNextAsync<T>());
        //    }
        //    //
        //    return results;
        //}

        //public Task CreateItemAsync(T item)
        //{
        //    return DocumentDbUnitOfWork.Client.CreateDocumentAsync(
        //        DocumentDbUnitOfWork.BuildCollectionUri(typeof(T).Name)
        //        //UriFactory.CreateDocumentCollectionUri(DatabaseId, nameof(T))
        //        , item);
        //}


        //public Task UpdateItemAsync(string id, T item)
        //{
        //    return DocumentDbUnitOfWork.Client.ReplaceDocumentAsync(
        //        DocumentDbUnitOfWork.BuildDocumentUri(typeof(T).Name, id),
        //        //UriFactory.CreateDocumentUri(DatabaseId, nameof(T), id),
        //        item);
        //}

        //public async Task<T> GetItemAsync(string id)
        //{
        //    try
        //    {
        //        Document document = await DocumentDbUnitOfWork.Client.ReadDocumentAsync(
        //            DocumentDbUnitOfWork.BuildDocumentUri(typeof(T).Name, id));
        //        return (T)(dynamic)document;
        //    }
        //    catch (DocumentClientException e)
        //    {
        //        if (e.StatusCode == HttpStatusCode.NotFound)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}
    }

    public static class DocumentDbUnitOfWork
    {
        private static readonly string DatabaseId = "Zbox";
        private static readonly string CollectionIds = "Flashcard";//ConfigurationManager.AppSettings["collection"];
        private static readonly DocumentClient _client;
        //private const bool NeedUpdate = false;
        private const string DevPrefix = "-dev";


        static DocumentDbUnitOfWork()
        {
            try
            {
                _client = new DocumentClient(new Uri("https://zboxnew.documents.azure.com:443/"),
                    "y2v1XQ6WIg81Soasz5YBA7R8fAp52XhJJufNmHy1t7y3YQzpBqbgRnlRPlatGhyGegKdsLq0qFChzOkyQVYdLQ==");
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse - for further need
                //if (NeedUpdate)
                //{
                CreateDatabaseIfNotExistsAsync().Wait();
                foreach (var collectionId in CollectionIds.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                    )
                {
                    CreateCollectionIfNotExistsAsync(collectionId).Wait();
                    CreateCollectionIfNotExistsAsync(collectionId + DevPrefix).Wait();
                }
                //}
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(ex);
            }
        }

        internal static DocumentClient Client => _client;

        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await _client.CreateDatabaseAsync(new Database { Id = DatabaseId }).ConfigureAwait(false);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.Conflict)
                {
                    return;
                }
                throw;
            }
        }

        public static Uri BuildCollectionUri(string collectionId)
        {
            if (!ConfigFetcher.IsRunningOnCloud || ConfigFetcher.IsEmulated)
            {
                collectionId += DevPrefix;
            }
            return UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionId);
        }

        public static Uri BuildDocumentUri(string collectionId, string documentId)
        {
            if (!ConfigFetcher.IsRunningOnCloud || ConfigFetcher.IsEmulated)
            {
                collectionId += DevPrefix;
            }
            return UriFactory.CreateDocumentUri(DatabaseId, collectionId, documentId);
        }

        private static async Task CreateCollectionIfNotExistsAsync(string collectionId)
        {
            try
            {
                await _client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = collectionId },
                        new RequestOptions { OfferThroughput = 1000 }).ConfigureAwait(false);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.Conflict)
                {
                    return;
                }
                throw;
            }
        }
    }
}

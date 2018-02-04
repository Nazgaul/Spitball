using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
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
                Document document = await DocumentDbUnitOfWork.Client.ReadDocumentAsync(DocumentDbUnitOfWork.BuildDocumentUri(GetCollectionId(), id)).ConfigureAwait(false);
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
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
                if (attribute is DocumentDbModelAttribute docAttribute)
                {
                    return docAttribute.CollectionId;
                }
            }
            throw new ArgumentException("no collection Id");
        }

        //public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        //{
        //    var query = DocumentDbUnitOfWork.Client.CreateDocumentQuery<T>(
        //        DocumentDbUnitOfWork.BuildCollectionUri(GetCollectionId()),
        //        new FeedOptions { MaxItemCount = -1 })
        //        .Where(predicate)
        //        .AsDocumentQuery();

        //    var results = new List<T>();
        //    while (query.HasMoreResults)
        //    {
        //        results.AddRange(await query.ExecuteNextAsync<T>().ConfigureAwait(false));
        //    }

        //    return results;
        //}

        public Task CreateItemAsync(T item)
        {
            return DocumentDbUnitOfWork.Client.CreateDocumentAsync(
                DocumentDbUnitOfWork.BuildCollectionUri(GetCollectionId()), item);
        }

        public Task UpdateItemAsync(string id, T item)
        {
            return DocumentDbUnitOfWork.Client.ReplaceDocumentAsync(
                DocumentDbUnitOfWork.BuildDocumentUri(GetCollectionId(), id), item);
        }
    }

    public static class DocumentDbUnitOfWork
    {
        private const string DatabaseId = "Zbox";
        private const string CollectionIds = "Flashcard";//ConfigurationManager.AppSettings["collection"];
        //private const bool NeedUpdate = false;
        private const string DevPrefix = "-dev";

        static DocumentDbUnitOfWork()
        {
            try
            {
                DocumentDbUnitOfWork.Client = new DocumentClient(new Uri("https://zboxnew.documents.azure.com:443/"),
                    "y2v1XQ6WIg81Soasz5YBA7R8fAp52XhJJufNmHy1t7y3YQzpBqbgRnlRPlatGhyGegKdsLq0qFChzOkyQVYdLQ==");
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse - for further need
                //if (NeedUpdate)
                //{
              //  CreateDatabaseIfNotExistsAsync().Wait();
                foreach (var collectionId in CollectionIds.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                    )
                {
                   // CreateCollectionIfNotExistsAsync(collectionId).Wait();
                   // CreateCollectionIfNotExistsAsync(collectionId + DevPrefix).Wait();
                }
                //}
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(ex);
            }
        }

        internal static DocumentClient Client { get; }

        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await DocumentDbUnitOfWork.Client.CreateDatabaseAsync(new Database { Id = DatabaseId }).ConfigureAwait(false);
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
            collectionId = BuildCollectionName(collectionId);
            return UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionId);
        }

        private static string BuildCollectionName(string collectionId)
        {
            bool.TryParse(ConfigFetcher.Fetch("documentDbProduction"), out bool isProduction);
            if (isProduction)
            {
                return collectionId;
            }
            if (!ConfigFetcher.IsRunningOnCloud || ConfigFetcher.IsEmulated)
            {
                collectionId += DevPrefix;
            }
            return collectionId;
        }

        public static Uri BuildDocumentUri(string collectionId, string documentId)
        {
            collectionId = BuildCollectionName(collectionId);
            return UriFactory.CreateDocumentUri(DatabaseId, collectionId, documentId);
        }

        private static async Task CreateCollectionIfNotExistsAsync(string collectionId)
        {
            try
            {
                await DocumentDbUnitOfWork.Client.CreateDocumentCollectionAsync(
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

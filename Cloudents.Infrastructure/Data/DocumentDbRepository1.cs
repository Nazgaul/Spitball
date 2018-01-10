using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Entities.DocumentDb;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Cloudents.Infrastructure.Data
{
    public class DocumentDbRepositoryUnitOfWork : IStartable
    {
        internal const string DatabaseId = "Zbox";
        internal DocumentClient Client;

        public void Start()
        {
            Client = new DocumentClient(new Uri("https://zboxnew.documents.azure.com:443/"), "y2v1XQ6WIg81Soasz5YBA7R8fAp52XhJJufNmHy1t7y3YQzpBqbgRnlRPlatGhyGegKdsLq0qFChzOkyQVYdLQ==");

            var t1 = CreateDatabaseIfNotExistsAsync();
            var t2 = CreateCollectionsIfNotExistsAsync();

            Task.WhenAll(t1, t2).Wait();
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await Client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId)).ConfigureAwait(false);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    await Client.CreateDatabaseAsync(new Database { Id = DatabaseId }).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }
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

        private Task CreateCollectionsIfNotExistsAsync()
        {
            var assembly = Assembly.Load("Cloudents.Core");
            var list = new List<Task>();
            foreach (var type in assembly.GetTypes().Where(w => w.GetCustomAttributes(typeof(CollectionIdAttribute), true).Length > 0))
            {
                var collection = GetCollectionId(type);
                list.Add(CreateCollectionIfNotExistsAsync(collection));
            }

            return Task.WhenAll(list);
        }

        private async Task CreateCollectionIfNotExistsAsync(string collectionId)
        {
            try
            {
                await Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionId)).ConfigureAwait(false);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    await Client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = collectionId },
                        new RequestOptions { OfferThroughput = 1000 }).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}

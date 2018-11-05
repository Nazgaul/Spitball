//using Microsoft.Azure.Search;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Host.Config;

//namespace Cloudents.FunctionsV2.Binders
//{
//    //public class AzureSearchSyncProvider : IExtensionConfigProvider
//    //{

//    //    private static SearchServiceClient _client;
//    //    public AzureSearchSyncProvider()
//    //    {

//    //    }
//    //    public void Initialize(ExtensionConfigContext context)
//    //    {
//    //        Client = new SearchServiceClient(key.Name, new SearchCredentials(key.Key));
//    //        // context.AddConverter<JObject, RedisOutput>(input => input.ToObject<RedisOutput>());
//    //        // Redis output binding
//    //        context
//    //            .AddBindingRule<AzureSearchSyncAttribute>()
//    //            .BindToCollector<AzureSearchSyncOutput>(CreateCollector);
//    //        // Redis database (input) binding
//    //        //context
//    //        //    .AddBindingRule<RedisDatabaseAttribute>()
//    //        //    .BindToInput<IDatabase>(ResolveRedisDatabase);
//    //    }


//    //    private IAsyncCollector<AzureSearchSyncOutput> CreateCollector(AzureSearchSyncAttribute attribute)
//    //    {
//    //        var key = attribute.Key;
//    //        var name = attribute.Name;
//    //        _client = new SearchServiceClient(key, new SearchCredentials(name));

//    //        var indexName = attribute.IndexName;
//    //        if (attribute.IsDevelop)
//    //        {
//    //            indexName += "-dev";
//    //        }

//    //        var indexClient = _client.Indexes.GetClient(indexName);
//    //        return new AzureSearchSyncAsyncCollector(indexClient);
//    //    }
//    //}
//}
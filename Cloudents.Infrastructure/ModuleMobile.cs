using Autofac;

namespace Cloudents.Infrastructure
{
    public class ModuleMobile : Module
    {
        //private readonly string _sqlConnectionString;
        //private readonly SearchServiceCredentials _searchServiceCredentials;
        //private readonly string _redisConnectionString;
        //private readonly string _storageConnectionString;

        //public ModuleMobile(string sqlConnectionString,
        //    SearchServiceCredentials searchServiceCredentials,
        //    string redisConnectionString,
        //    string storageConnectionString)
        //{
        //    _sqlConnectionString = sqlConnectionString;
        //    _searchServiceCredentials = searchServiceCredentials;
        //    _redisConnectionString = redisConnectionString;
        //    _storageConnectionString = storageConnectionString;
        //}

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule<ModuleRead>();
            //builder.RegisterModule(new ModuleRead(_sqlConnectionString, _redisConnectionString,
            //    _storageConnectionString, _searchServiceCredentials));
        }
    }
}
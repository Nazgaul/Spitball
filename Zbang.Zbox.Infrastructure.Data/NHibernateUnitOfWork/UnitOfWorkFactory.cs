using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Event;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.Dialect;
using Zbang.Zbox.Infrastructure.Data.Driver;
using Zbang.Zbox.Infrastructure.Data.Events;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.UnitsOfWork;
using Environment = NHibernate.Cfg.Environment;

namespace Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        //private static ISession _currentSession;

        private const string SerializationFile = "nHibernateConfig";

        public const string CurrentSessionKey = "CurrentSession.Key";

        internal UnitOfWorkFactory()
        {
            ConfigureNHibernate();

        }

        private void ConfigureNHibernate()
        {
            Configuration = LoadConfigurationFromFile();

            if (Configuration == null)
            {
                Configuration = new Configuration();

                Configuration.DataBaseIntegration(dbi =>
                {
                    dbi.ConnectionProvider<DriverConnectionProvider>();

                    dbi.Dialect<ZboxDialect>();
                    dbi.Driver<ZboxDriver>();
                    dbi.ConnectionString = ConfigFetcher.Fetch(DapperConnection.ConnectionStringKey);
                    dbi.SchemaAction = SchemaAutoAction.Validate;
                    dbi.BatchSize = 100;
#if DEBUG
                    dbi.LogSqlInConsole = true;
                    dbi.LogFormattedSql = true;
#endif
                });
                Configuration.SetListener(ListenerType.Delete, new ZboxDeleteEventListener());
                //m_Configuration.SetListener(ListenerType.Save, new ZboxUpdateEventListener());

                Configuration.SetListener(ListenerType.PreInsert, new AuditEventListener());
                Configuration.SetListener(ListenerType.PreUpdate, new AuditEventListener());

                //m_Configuration.SetProperty(Environment.ConnectionDriver, typeof(MiniProfiler.NHibernate.MiniProfilerSql2008ClientDriver).AssemblyQualifiedName)
                Configuration.SetProperty(Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
                Configuration.SetProperty(Environment.UseProxyValidator, bool.FalseString);
                Configuration.SetProperty(Environment.DefaultSchema, "Zbox");

                Configuration.AddAssembly("Zbang.Zbox.Domain");
                Configuration.AddAssembly("Zbang.Zbox.ViewModel");

                //if (HttpContext.Current != null)
                //{
                //    m_Configuration.CurrentSessionContext<NHibernate.Context.WebSessionContext>();
                //}
                //else
                //{
                //    m_Configuration.CurrentSessionContext<NHibernate.Context.CallSessionContext>();
                //}

                SaveConfiguration();
            }

            SessionFactory = Configuration.BuildSessionFactory();

        }
        private void SaveConfiguration()
        {
            try
            {
                //TODO: this is for now - Jared is not using IOC factory
                var wrapper = IocFactory.IocWrapper;
                if (wrapper == null)
                {
                    return;
                }
                var storage = wrapper.Resolve<ICache>();
                using (var ms = new MemoryStream())
                {
                    IFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, Configuration);


                    storage.AddToCache("nhibernate", GetConfigurationFileName(), ms.ToArray(),
                        TimeSpan.FromDays(90));
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("nhibernate SaveConfiguration", ex);
            }
        }


        private static string GetConfigurationFileName()
        {
            var domain = Assembly.Load("Zbang.Zbox.Domain");
            var viewModel = Assembly.Load("Zbang.Zbox.ViewModel");

            var buildVersion = Assembly.GetExecutingAssembly().GetName().Version.Revision;
            var domainBuildVersion = domain.GetName().Version.Revision;
            var viewModelBuildVersion = viewModel.GetName().Version.Revision;
            var fileInfo = $"{SerializationFile}{buildVersion}{domainBuildVersion}{viewModelBuildVersion}";
            return fileInfo;
        }

        private static Configuration LoadConfigurationFromFile()
        {

            try
            {
                var wrapper = IocFactory.IocWrapper;
                if (wrapper == null)
                {
                    return null;
                }
                var storage = wrapper.Resolve<ICache>();
                var file = storage.GetFromCache<byte[]>("nhibernate", GetConfigurationFileName());
                if (file == null)
                {
                    return null;
                }
                using (var ms = new MemoryStream(file))
                {
                    var bf = new BinaryFormatter();
                    return (Configuration)bf.Deserialize(ms);
                }

            }
            catch (Exception) { return null; }
        }




        public IUnitOfWork Create()
        {
            var session = CreateSession();
            session.FlushMode = FlushMode.Commit;
            CurrentSession = session;
            return new UnitOfWorkImplementor(this, session);
        }

        public Configuration Configuration { get; private set; }

        public ISessionFactory SessionFactory { get; private set; }

        public ISession CurrentSession
        {
            get
            {
                //if (_currentSession == null)
                //    throw new InvalidOperationException("You are not in a unit of work.");
                //return _currentSession;
                if (Local.Data[CurrentSessionKey] == null)
                {
                    throw new InvalidOperationException("You are not in a unit of work.");
                }
                return (ISession)Local.Data[CurrentSessionKey];
            }
            set
            {
                Local.Data[CurrentSessionKey] = value;
                //_currentSession = value;
            }
        }



        public void DisposeUnitOfWork(IUnitOfWorkImplementor adapter)
        {
            CurrentSession = null;
            UnitOfWork.DisposeUnitOfWork(adapter);
        }

        private ISession CreateSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}

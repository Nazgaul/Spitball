using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Event;
using Zbang.Zbox.Infrastructure.Data.Dialect;
using Zbang.Zbox.Infrastructure.Data.Driver;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.UnitsOfWork;
using Environment = NHibernate.Cfg.Environment;

namespace Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        //private static ISession _currentSession;
        private ISessionFactory m_SessionFactory;
        private Configuration m_Configuration;

        private const string SerializationFile = "nHibernateConfig";

        public const string CurrentSessionKey = "CurrentSession.Key";

        internal UnitOfWorkFactory()
        {

            ConfigureNHibernate();

            //  Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.ILocalStorageProvider>();
        }

        private void ConfigureNHibernate()
        {
            m_Configuration = LoadConfigurationFromFile();

            if (m_Configuration == null)
            {
                m_Configuration = new Configuration();

                m_Configuration.DataBaseIntegration(dbi =>
                {
                    dbi.ConnectionProvider<DriverConnectionProvider>();
                    
                    dbi.Dialect<ZboxDialect>();
                    dbi.Driver<ZboxDriver>();
                    dbi.ConnectionString = ConfigFetcher.Fetch("Zbox");
                    dbi.SchemaAction = SchemaAutoAction.Update;
                    
#if DEBUG
                    dbi.LogSqlInConsole = true;
                    dbi.LogFormattedSql = true;
#endif
                });
                m_Configuration.SetListener(ListenerType.Delete, new ZboxDeleteEventListener());
                m_Configuration.SetProperty(Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
                m_Configuration.SetProperty(Environment.UseProxyValidator, bool.FalseString);
                m_Configuration.SetProperty(Environment.DefaultSchema, "Zbox");

                m_Configuration.AddAssembly("Zbang.Zbox.Domain");
                m_Configuration.AddAssembly("Zbang.Zbox.ViewModel");
                //m_Configuration.AddAssembly("Zbang.Zbox.ApiViewModel");


                SaveConfiguration();
            }

            m_SessionFactory = m_Configuration.BuildSessionFactory();

        }
        private void SaveConfiguration()
        {
            var storage = IocFactory.Unity.Resolve<ILocalStorageProvider>();

            using (var ms = new MemoryStream())
            {
                IFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, m_Configuration);
                storage.SaveFileToStorage(ms, GetConfigurationFileName());
            }
        }

        //private bool IsConfigurationFileValid()
        //{
        //    var storage = Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.ILocalStorageProvider>();
        //    try
        //    {
        //        Assembly assembly = Assembly.GetExecutingAssembly();
        //        FileInfo asmInfo = new FileInfo(assembly.Location);

        //        //FileInfo configInfo = new FileInfo(SerializedConfiguration);
        //        var fileDate = storage.GetFileLastModified(SerializationFile);
        //        var asmblyDate = asmInfo.LastWriteTime;
        //        return fileDate >= asmblyDate;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        private string GetConfigurationFileName()
        {
            var domain = Assembly.Load("Zbang.Zbox.Domain");
            var viewModel = Assembly.Load("Zbang.Zbox.ViewModel");

            var buildVersion = Assembly.GetExecutingAssembly().GetName().Version.Revision;
            var domainBuildVersion = domain.GetName().Version.Revision;
            var viewModelBuildVersion = viewModel.GetName().Version.Revision;
            
            return string.Format("{0}{1}{2}{3}", SerializationFile, buildVersion, domainBuildVersion, viewModelBuildVersion);
        }

        private Configuration LoadConfigurationFromFile()
        {
            //if (!IsConfigurationFileValid()) return null;
            try
            {
                var storage = IocFactory.Unity.Resolve<ILocalStorageProvider>();
                var file = storage.ReadFileFromStorage(GetConfigurationFileName());
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

        public Configuration Configuration
        {
            get
            {
                return m_Configuration;
            }
        }

        public ISessionFactory SessionFactory
        {
            get
            {
                return m_SessionFactory;
            }
        }

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

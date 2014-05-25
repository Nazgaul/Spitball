using NHibernate;
using NHibernate.Cfg;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.UnitsOfWork;


namespace Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork
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
                    dbi.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                    
                    dbi.Dialect<Dialect.ZboxDialect>();
                    dbi.Driver<Driver.ZboxDriver>();
                    dbi.ConnectionString = ConfigFetcher.Fetch("Zbox");
                    //dbi.ConnectionStringName = "Zbox";
                    dbi.SchemaAction = SchemaAutoAction.Validate;
                    
#if DEBUG
                    dbi.LogSqlInConsole = true;
                    dbi.LogFormattedSql = true;
#endif
                });
                m_Configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
                m_Configuration.SetProperty(NHibernate.Cfg.Environment.UseProxyValidator, bool.FalseString);
                m_Configuration.SetProperty(NHibernate.Cfg.Environment.DefaultSchema, "Zbox");

                m_Configuration.AddAssembly("Zbang.Zbox.Domain");
                m_Configuration.AddAssembly("Zbang.Zbox.ViewModel");
                //m_Configuration.AddAssembly("Zbang.Zbox.ApiViewModel");



                SaveConfiguration();
            }

            m_SessionFactory = m_Configuration.BuildSessionFactory();

        }
        private void SaveConfiguration()
        {
            var storage = Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.ILocalStorageProvider>();

            using (MemoryStream ms = new MemoryStream())
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

        private NHibernate.Cfg.Configuration LoadConfigurationFromFile()
        {
            //if (!IsConfigurationFileValid()) return null;
            try
            {
                var storage = Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.ILocalStorageProvider>();
                var file = storage.ReadFileFromStorage(GetConfigurationFileName());
                if (file == null)
                {
                    return null;
                }
                using (var ms = new MemoryStream(file))
                {
                    BinaryFormatter bf = new BinaryFormatter();


                    return (NHibernate.Cfg.Configuration)bf.Deserialize(ms);
                }

            }
            catch (Exception) { return null; }
        }




        public IUnitOfWork Create()
        {
            ISession session = CreateSession();
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

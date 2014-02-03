using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel;
using Microsoft.Practices.Unity;
using System.IO;
using System.Configuration;
using Microsoft.Practices.Unity.Configuration;

namespace Zbang.Zbox.Infrastructure.ServiceModel.RestIoc
{
    public class UnityServiceHostFactory : WebServiceHostFactory
    {
        private IUnityContainer m_IoCContainer;

        public UnityServiceHostFactory()
        {
            m_IoCContainer = new UnityContainer();
            LoadConfigurations();
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new UnityWebServiceHost(m_IoCContainer, serviceType, baseAddresses);
        }

        private void LoadConfigurations()
        {
            LoadConfigurations("~");
            LoadConfigurations("~/bin");
        }

        private void LoadConfigurations(string path)
        {
            string root = System.Web.HttpContext.Current.Server.MapPath(path);

            var unityConfigFiles = from f in Directory.GetFiles(root, "*.Unity.config")
                                   select f;

            foreach (var file in unityConfigFiles)
            {
                this.LoadConfigurationsFromFile(file);
            }
        }

        private void LoadConfigurationsFromFile(string fileName)
        {
            ExeConfigurationFileMap executionFileMap = new ExeConfigurationFileMap();
            executionFileMap.ExeConfigFilename = fileName;

            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(executionFileMap, ConfigurationUserLevel.None);



            UnityConfigurationSection section = (UnityConfigurationSection)config.GetSection("unity");

            section.Configure(this.m_IoCContainer);
        }
        public T Resolve<T>()
        {
            return m_IoCContainer.Resolve<T>();
        }
    }
}

using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Zbang.Zbox.Infrastructure.ServiceModel.Ioc
{
    public class UnityServiceHostFactory : ServiceHostFactory
    {
        //Fields
        private readonly IUnityContainer m_IoCContainer;
        private Type m_ServiceType;

        //Ctor
        public UnityServiceHostFactory()
        {
            m_IoCContainer = new UnityContainer();           
        }

        //Methods
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            m_ServiceType = serviceType;

            LoadConfigurations();

            return new UnityServiceHost(m_IoCContainer, serviceType, baseAddresses);
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
                LoadConfigurationsFromFile(file);
            }
        }

        private void LoadConfigurationsFromFile(string fileName)
        {
            var executionFileMap = new ExeConfigurationFileMap {ExeConfigFilename = fileName};

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(executionFileMap, ConfigurationUserLevel.None);

            var section = (UnityConfigurationSection)config.GetSection("unity");

            string configurationName = GetConfigurationName(m_ServiceType);

            ContainerElement unityContainerElement = section.Containers[configurationName];

            if (unityContainerElement != null)
            {
                section.Configure(m_IoCContainer, unityContainerElement.Name);
            }
            else
            {
                section.Configure(m_IoCContainer);
            }
        }

        private string GetConfigurationName(Type serviceType)
        {
            ServiceBehaviorAttribute serviceBehaviorAttribute = serviceType.GetCustomAttributes(true).OfType<ServiceBehaviorAttribute>().FirstOrDefault();

            return (serviceBehaviorAttribute != null && !string.IsNullOrEmpty(serviceBehaviorAttribute.ConfigurationName))
                    ? serviceBehaviorAttribute.ConfigurationName
                    : serviceType.FullName;
        }
    }
}

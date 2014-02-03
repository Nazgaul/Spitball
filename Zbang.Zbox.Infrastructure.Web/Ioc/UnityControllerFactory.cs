using System.Linq;
using System.IO;
using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Zbang.Zbox.Infrastructure.Web.Ioc
{
    public class UnityControllerFactory 
    {
        private readonly UnityContainer m_IoCContainer;

        public UnityControllerFactory()
        {
            
            m_IoCContainer = new UnityContainer();            
            LoadConfigurations();
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
            ExeConfigurationFileMap executionFileMap = new ExeConfigurationFileMap();
            executionFileMap.ExeConfigFilename = fileName;

            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(executionFileMap, ConfigurationUserLevel.None);

            UnityConfigurationSection section = (UnityConfigurationSection)config.GetSection("unity");

            section.Configure(m_IoCContainer);
        }

        public T Resolve<T>()
        {
            return m_IoCContainer.Resolve<T>();
        }

        public IUnityContainer GetContainer()
        {
            return m_IoCContainer;
        }


    }
}

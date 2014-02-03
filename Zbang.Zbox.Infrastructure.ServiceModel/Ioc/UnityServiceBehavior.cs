using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.Collections.ObjectModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

using Microsoft.Practices.Unity;

namespace Zbang.Zbox.Infrastructure.ServiceModel.Ioc
{
    public class UnityServiceBehavior: IServiceBehavior
    {
        //Fields
        private readonly IUnityContainer m_IoCContainer;

        //Ctor
        public UnityServiceBehavior(IUnityContainer container)
        {
            m_IoCContainer = container;
        }

        //Methods
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {

        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {            
            foreach (ChannelDispatcher  channelDispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                {
                    if (endpointDispatcher.ContractName != "IMetadataExchange")
                    {
                        string contractName = endpointDispatcher.ContractName;
                        ServiceEndpoint serviceEndpoint = serviceDescription.Endpoints.FirstOrDefault(ep => ep.Contract.Name.Equals(contractName));

                        if (serviceEndpoint != null)
                        {
                            endpointDispatcher.DispatchRuntime.InstanceProvider = new UnityInstanceProvider(m_IoCContainer, serviceEndpoint.Contract.ContractType);
                        }
                    }
                }
            }    
        }        
    }
}

using System.Linq;
using System.ServiceModel.Description;
using Microsoft.Practices.Unity;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Collections.ObjectModel;

namespace Zbang.Zbox.Infrastructure.ServiceModel.RestIoc
{
    public class UnityServiceBehaviour : IServiceBehavior
    {
        private readonly IUnityContainer m_Container;

        public UnityServiceBehaviour(IUnityContainer container)
        {
            m_Container = container;
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var endpointDispatcher in
                serviceHostBase.ChannelDispatchers.OfType<ChannelDispatcher>().SelectMany(
                    channelDispatcher => channelDispatcher.Endpoints))
            {
                endpointDispatcher.DispatchRuntime.InstanceProvider = new UnityInstanceProvider(m_Container, serviceDescription.ServiceType);
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
                                         Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters)
        {
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}

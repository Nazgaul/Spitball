using System;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.ServiceModel.Channels;

using Microsoft.Practices.Unity;

namespace Zbang.Zbox.Infrastructure.ServiceModel.Ioc
{
    public class UnityInstanceProvider : IInstanceProvider
    {
        //Fields
        private readonly IUnityContainer m_IoCContainer;

        private readonly Type m_ContractType;

        //Ctor
        public UnityInstanceProvider(IUnityContainer container, Type contractType)
        {
            m_IoCContainer = container;
            m_ContractType = contractType;
        }

        //Methods
        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return m_IoCContainer.Resolve(m_ContractType);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            m_IoCContainer.Teardown(instance);
        }
    }
}

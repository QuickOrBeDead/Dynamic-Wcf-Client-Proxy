using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Labo.ServiceModel.DynamicProxy
{
    [Serializable]
    public sealed class ServiceClientProxy
    {
        private readonly Type m_ProxyType;
        private readonly Binding m_Binding;
        private readonly EndpointAddress m_Address;

        public ServiceClientProxy(Type proxyType, Binding binding, EndpointAddress address)
        {
            m_ProxyType = proxyType;
            m_Binding = binding;
            m_Address = address;
        }

        public object CreateInstance()
        {
            return Activator.CreateInstance(m_ProxyType, m_Binding, m_Address);
        }
    }
}
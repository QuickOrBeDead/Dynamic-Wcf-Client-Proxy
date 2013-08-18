using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;
using Labo.ServiceModel.Core.Utils.Reflection;

namespace Labo.ServiceModel.DynamicProxy
{
    [Serializable]
    public sealed class ServiceClientProxyFactory
    {
        private readonly ServiceClientProxyCompileResult m_ClientProxyCompileResult;

        public Collection<Binding> Bindings
        {
            get { return m_ClientProxyCompileResult.ServiceMetadataInformation.Bindings; }
        }

        public Collection<ContractDescription> Contracts
        {
            get { return m_ClientProxyCompileResult.ServiceMetadataInformation.Contracts; }
        }

        public Collection<ServiceEndpoint> Endpoints
        {
            get { return m_ClientProxyCompileResult.ServiceMetadataInformation.Endpoints; }
        }

        public string Config { get { return m_ClientProxyCompileResult.Config; } }

        public ServiceClientProxyFactory(ServiceClientProxyCompileResult clientProxyCompileResult)
        {
            m_ClientProxyCompileResult = clientProxyCompileResult;
        }

        public ServiceClientProxy CreateProxy(string contractName)
        {
            return CreateProxy(contractName, null);
        }

        public ServiceClientProxy CreateProxy(string contractName, string contractNamespace)
        {
            ServiceEndpoint endpoint = GetEndpoint(contractName, contractNamespace);
            return CreateProxy(endpoint);
        }

        public ServiceClientProxy CreateProxy(ServiceEndpoint endpoint)
        {
            Type[] assemblyTypes = m_ClientProxyCompileResult.CompiledAssembly.GetTypes();
            Type contractType = GetServiceContractType(endpoint.Contract.Name, endpoint.Contract.Namespace, assemblyTypes);
            Type proxyType = GetProxyType(contractType, assemblyTypes);
            return new ServiceClientProxy(proxyType, endpoint.Binding, endpoint.Address);
        }

        private ServiceEndpoint GetEndpoint(string contractName, string contractNamespace)
        {
            for (int i = 0; i < Endpoints.Count; i++)
            {
                ServiceEndpoint endpoint = Endpoints[i];
                if (string.Compare(endpoint.Contract.Name, contractName, StringComparison.OrdinalIgnoreCase) == 0 &&
                    string.Compare(endpoint.Contract.Namespace, contractNamespace, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return endpoint;
                }
            }
            return null;
        }

        private static Type GetProxyType(Type contractType, IList<Type> assemblyTypes)
        {
            Type clientBaseType = typeof(ClientBase<>).MakeGenericType(contractType);
            for (int i = 0; i < assemblyTypes.Count; i++)
            {
                Type type = assemblyTypes[i];
                if (type.IsClass && contractType.IsAssignableFrom(type) && type.IsSubclassOf(clientBaseType))
                {
                    return type;
                }
            }
            return null;
        }

        private static Type GetServiceContractType(string contractName, string contractNamespace, Type[] assemblyTypes)
        {
            for (int i = 0; i < assemblyTypes.Length; i++)
            {
                Type type = assemblyTypes[i];
                if (!type.IsInterface)
                {
                    continue;
                }

                ServiceContractAttribute serviceContractAttribute = ReflectionUtils.GetCustomAttribute<ServiceContractAttribute>(type);
                if (serviceContractAttribute == null)
                {
                    continue;
                }

                XmlQualifiedName xmlQualifiedServiceContractName = GetServiceContractName(type, serviceContractAttribute.Name, serviceContractAttribute.Namespace);

                if (string.Compare(xmlQualifiedServiceContractName.Name, contractName, StringComparison.OrdinalIgnoreCase) == 0 &&
                    string.Compare(xmlQualifiedServiceContractName.Namespace, contractNamespace, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return type;
                }
            }

            return null;
        }

        private static XmlQualifiedName GetServiceContractName(Type contractType, string name, string nameSpace)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = contractType.Name;
            }

            nameSpace = string.IsNullOrWhiteSpace(nameSpace) ? "http://tempuri.org/" : Uri.EscapeUriString(nameSpace);

            return new XmlQualifiedName(name, nameSpace);
        }
    }
}
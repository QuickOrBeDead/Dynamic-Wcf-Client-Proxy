using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Labo.ServiceModel.DynamicProxy
{
    [Serializable]
    public sealed class ServiceMetadataInformation
    {
        private Collection<Binding> m_Bindings;
        public Collection<Binding> Bindings
        {
            get { return m_Bindings ?? (m_Bindings = new Collection<Binding>()); }
            set { m_Bindings = value; }
        }

        private Collection<ContractDescription> m_Contracts;
        public Collection<ContractDescription> Contracts
        {
            get { return m_Contracts ?? (m_Contracts = new Collection<ContractDescription>()); }
            set { m_Contracts = value; }
        }

        private Collection<ServiceEndpoint> m_Endpoints;
        public Collection<ServiceEndpoint> Endpoints
        {
            get { return m_Endpoints ?? (m_Endpoints = new Collection<ServiceEndpoint>()); }
            set { m_Endpoints = value; }
        }

        public CodeCompileUnit CodeCompileUnit { get; private set; }

        public CodeDomProvider CodeDomProvider { get; private set; }

        public ServiceMetadataInformation(CodeCompileUnit codeCompileUnit, CodeDomProvider codeDomProvider)
        {
            CodeDomProvider = codeDomProvider;
            CodeCompileUnit = codeCompileUnit;
        }
    }
}
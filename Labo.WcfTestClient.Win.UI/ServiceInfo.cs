using System;
using System.Collections.Generic;

namespace Labo.WcfTestClient.Win.UI
{
    [Serializable]
    public sealed class ServiceInfo
    {
        public string Wsdl { get; set; }

        private List<ContractInfo> m_Contracts;
        public List<ContractInfo> Contracts
        {
            get
            {
                return m_Contracts ?? (m_Contracts = new List<ContractInfo>());
            }
        }

        public string Config { get; set; }
    }
}
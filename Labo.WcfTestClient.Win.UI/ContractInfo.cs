using System.Collections.Generic;
using Labo.ServiceModel.DynamicProxy;

namespace Labo.WcfTestClient.Win.UI
{
    public sealed class ContractInfo
    {
        private List<OperationInfo> m_Operations;
        public List<OperationInfo> Operations
        {
            get
            {
                return m_Operations ?? (m_Operations = new List<OperationInfo>());
            }
        }

        public string ContractName { get; set; }

        public ServiceClientProxy Proxy { get; set; }
    }
}

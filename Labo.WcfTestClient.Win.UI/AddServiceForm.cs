using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Description;
using System.Windows.Forms;
using Labo.ServiceModel.Core.Utils.Reflection;
using Labo.ServiceModel.DynamicProxy;

namespace Labo.WcfTestClient.Win.UI
{
    public partial class AddServiceForm : Form
    {
        private string Wsdl
        {
            get
            {
                return txtServiceWsdl.Text.Trim();
            }
        }

        private IList<ServiceInfo> m_Services;
        public IList<ServiceInfo> Services
        {
            get
            {
                return m_Services ?? (m_Services = new ReadOnlyCollection<ServiceInfo>(new List<ServiceInfo>(0)));
            }
        }

        public AddServiceForm(Form owner)
        {
            InitializeComponent();

            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            Owner = owner;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceClientProxyFactoryGenerator proxyFactoryGenerator = new ServiceClientProxyFactoryGenerator(new ServiceMetadataDownloader(), new ServiceMetadataImporter(new CSharpCodeDomProviderFactory()), new ServiceClientProxyCompiler());
                ServiceClientProxyFactory proxyFactory = proxyFactoryGenerator.GenerateProxyFactory(Wsdl);
                List<ServiceInfo> serviceInfos = new List<ServiceInfo>();
                ServiceInfo serviceInfo = new ServiceInfo { Wsdl = Wsdl, Config = proxyFactory.Config };
                for (int index = 0; index < proxyFactory.Contracts.Count; index++)
                {
                    ContractDescription contractDescription = proxyFactory.Contracts[index];
                    string contractName = contractDescription.Name;
                    ServiceClientProxy proxy = proxyFactory.CreateProxy(contractName, contractDescription.Namespace);
                    string[] operationNames = contractDescription.Operations.Select(x => x.Name).ToArray();
                    ContractInfo contractInfo = new ContractInfo {Proxy = proxy, ContractName = contractName};

                    for (int i = 0; i < operationNames.Length; i++)
                    {
                        string operationName = operationNames[i];
                        object instance = proxy.CreateInstance();
                        using (instance as IDisposable)
                        {
                            Method method = ReflectionUtils.GetMethodDefinition(instance, operationName);
                            contractInfo.Operations.Add(new OperationInfo {Contract = contractInfo, Method = method});
                        }
                    }
                    serviceInfo.Contracts.Add(contractInfo);
                }
                serviceInfos.Add(serviceInfo);

                m_Services = serviceInfos.AsReadOnly();
            }
            catch (Exception exception)
            {
                ShowErrorForm.ShowDialog(this, exception);
            }
        }
    }
}

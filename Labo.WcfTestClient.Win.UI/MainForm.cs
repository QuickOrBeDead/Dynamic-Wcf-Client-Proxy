using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Labo.WcfTestClient.Win.UI
{
    public partial class MainForm : Form
    {
        private List<ServiceInfo> m_ServiceInfos;
        private List<ServiceInfo> ServiceInfos
        {
            get
            {
                return m_ServiceInfos ?? (m_ServiceInfos = new List<ServiceInfo>());
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void AddServiceToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddServiceForm addServiceForm = new AddServiceForm(this);
            DialogResult dialogResult = addServiceForm.ShowDialog(this);
            if(dialogResult == DialogResult.OK)
            {
                ServiceInfos.AddRange(addServiceForm.Services);

                FillTreeView();
            }
        }

        private void FillTreeView()
        {
            tvwServices.BeginUpdate();

            tvwServices.Nodes.Clear();

            for (int i = 0; i < ServiceInfos.Count; i++)
            {
                ServiceInfo serviceInfo = ServiceInfos[i];
                TreeNode serviceNode = new TreeNode(serviceInfo.Wsdl);

                List<ContractInfo> contractInfos = serviceInfo.Contracts;
                for (int j = 0; j < contractInfos.Count; j++)
                {
                    ContractInfo contractInfo = contractInfos[j];
                    TreeNode contractNode = new TreeNode(contractInfo.ContractName);

                    List<OperationInfo> operationInfos = contractInfo.Operations;
                    for (int k = 0; k < operationInfos.Count; k++)
                    {
                        OperationInfo operationInfo = operationInfos[k];
                        TreeNode operationNode = new TreeNode(operationInfo.Method.Name);
                        operationNode.Tag = operationInfo;
                        contractNode.Nodes.Add(operationNode);
                    }

                    serviceNode.Nodes.Add(contractNode);
                }

                TreeNode configNode = new TreeNode("Config");
                configNode.Name = "ConfigNode";
                configNode.Tag = serviceInfo.Config;
                configNode.ToolTipText = serviceInfo.Wsdl + " Config";
                serviceNode.Nodes.Add(configNode);

                tvwServices.Nodes.Add(serviceNode);
            }

            tvwServices.ExpandAll();
            tvwServices.EndUpdate();
        }

        private void tvwServices_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TreeNode selectedNode = tvwServices.SelectedNode;
            if (selectedNode.Name == "ConfigNode")
            {
                TabPage tabPage = new TabPage(selectedNode.ToolTipText);
                ServiceConfigUserControl serviceConfigUserControl = new ServiceConfigUserControl(selectedNode.Tag.ToString());
                serviceConfigUserControl.Dock = DockStyle.Fill;
                tabPage.Controls.Add(serviceConfigUserControl);
                tbOperations.TabPages.Add(tabPage);
            }
            else
            {
                OperationInfo operationInfo = selectedNode.Tag as OperationInfo;
                if (operationInfo != null)
                {
                    TabPage tabPage = new TabPage(operationInfo.Method.Name);
                    OperationInvokerUserControl operationInvokerUserControl = new OperationInvokerUserControl(operationInfo);
                    operationInvokerUserControl.Dock = DockStyle.Fill;
                    tabPage.Controls.Add(operationInvokerUserControl);
                    tbOperations.TabPages.Add(tabPage);
                }
            }
        }
    }
}

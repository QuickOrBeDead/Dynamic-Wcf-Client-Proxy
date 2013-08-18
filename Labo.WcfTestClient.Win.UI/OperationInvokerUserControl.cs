using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Labo.ServiceModel.Core.Utils.Reflection;

using Microsoft.VisualStudio.VirtualTreeGrid;

namespace Labo.WcfTestClient.Win.UI
{
    public partial class OperationInvokerUserControl : UserControl
    {
        private readonly OperationInfo m_Operation;

        public OperationInvokerUserControl(OperationInfo operationInfo)
        {
            InitializeComponent();

            m_Operation = operationInfo;

            VirtualTreeColumnHeader[] columnHeaders = new[]
                {
                    new VirtualTreeColumnHeader("Name"),
                    new VirtualTreeColumnHeader("Value"),
                    new VirtualTreeColumnHeader("Type")
                };
            inputControl.SetColumnHeaders(columnHeaders, true);
            outputControl.SetColumnHeaders(columnHeaders, true);

            PopulateTree(operationInfo.Method.Parameters, inputControl, false);
        }

        private static void PopulateTree(IEnumerable<Parameter> parameters, VirtualTreeControl parameterTreeView, bool readOnly)
        {
            PopulateTree(parameters.Cast<Member>().ToList(), parameterTreeView, readOnly);
        }

        private static void PopulateTree(IEnumerable<Member> parameters, VirtualTreeControl parameterTreeView, bool readOnly)
        {
            PopulateTree(parameters.Select(x => new MemberInfo { Member = x }).ToList(), parameterTreeView, readOnly);
        }

        private static void PopulateTree(IList<MemberInfo> parameters, VirtualTreeControl parameterTreeView, bool readOnly)
        {
            CreateRootTree(parameters, parameterTreeView, readOnly);

            ExpandTree(parameters, parameterTreeView);
        }

        private static void ExpandTree(IList<MemberInfo> parameters, VirtualTreeControl parameterTreeView)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                int row = parameters.Count - i - 1;
                if (parameterTreeView.Tree.IsExpandable(row, 0))
                {
                    parameterTreeView.Tree.ToggleExpansion(row, 0);
                }
            }
        }

        private static void CreateRootTree(IList<MemberInfo> parameters, VirtualTreeControl parameterTreeView, bool readOnly)
        {
            parameterTreeView.MultiColumnTree = new MultiColumnTree(3);
            ITree tree = (ITree)parameterTreeView.MultiColumnTree;
            OperationParameterTree operationParameterTree = new OperationParameterTree(3, tree, parameterTreeView, parameters, readOnly, null);
            tree.Root = operationParameterTree;
        }

        private void btnInvoke_Click(object sender, EventArgs e)
        {
            SetControlsEnabled(false);

            try
            {
                IDictionary<string, ReflectionUtils.Parameter> parameters = GetParameters();
                object instance = m_Operation.Contract.Proxy.CreateInstance();
                object result;
                using (instance as IDisposable)
                {
                    result = ReflectionUtils.InvokeMethod(instance, m_Operation.Method.Name, parameters);
                }
              
                if (result == null)
                {
                    Instance returnValue = m_Operation.Method.ReturnValue;
                    PopulateTree(new List<Member> { new Property { Definition = returnValue.Definition, Type = returnValue.Type } }, outputControl, true);
                }
                else
                {
                    Class @class = ReflectionUtils.GetClassDefinition(result.GetType());
                    Parameter parameter;
                    if(@class.IsClass)
                    {
                        parameter = new Parameter { Definition = @class, Name = "(return)", Type = @class.Type };
                    }
                    else
                    {
                        parameter = new Parameter { Name = "(return)", Type = @class.Type };
                    }
                    List<MemberInfo> memberInfos = new List<MemberInfo> { new MemberInfo { Member = parameter, Value = result } };
                    PopulateTree(memberInfos, outputControl, true);
                }
            }
            catch (Exception exception)
            {
                ShowErrorForm.ShowDialog(ParentForm, exception);
            }

            SetControlsEnabled(true);
        }

        private void SetControlsEnabled(bool enable)
        {
            btnInvoke.Enabled = enable;
            inputControl.Enabled = enable;
            outputControl.Enabled = enable;
        }

        private IDictionary<string, ReflectionUtils.Parameter> GetParameters()
        {
            OperationParameterTree operationParameterTree = ((OperationParameterTree)((ITree)inputControl.MultiColumnTree).Root);
            
            IDictionary<string, ReflectionUtils.Parameter> dictionary = new Dictionary<string, ReflectionUtils.Parameter>();
            for (int i = 0; i < operationParameterTree.Parameters.Count; i++)
            {
                MemberInfo memberInfo = operationParameterTree.Parameters[i];
                object value = memberInfo.Value;
                dictionary.Add(memberInfo.Member.Name, new ReflectionUtils.Parameter { Value = value, Type = memberInfo.Member.Type });

                SetProperties(value, i, operationParameterTree);
            }
            return dictionary;
        }

        private static void SetProperties(object value, int row, OperationParameterTree operationParameterTree)
        {
            OperationParameterTree childOperationParameterTree = operationParameterTree.Children[row];
            if (childOperationParameterTree != null && value != null)
            {
                for (int i = 0; i < childOperationParameterTree.Parameters.Count; i++)
                {
                    MemberInfo parameter = childOperationParameterTree.Parameters[i];
                    DynamicMethodCompilerCache.SetPropertyValue(value, parameter.Member.Name, parameter.Value);

                    SetProperties(parameter.Value, i, childOperationParameterTree);
                }
            }
        }
    }
}

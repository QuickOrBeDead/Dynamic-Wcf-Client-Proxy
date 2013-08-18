using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Labo.ServiceModel.Core.Utils.Conversion;
using Labo.ServiceModel.Core.Utils.Reflection;
using Microsoft.VisualStudio.VirtualTreeGrid;

namespace Labo.WcfTestClient.Win.UI
{
    public sealed class OperationParameterTree :  IMultiColumnBranch, IBranch
    {
        private const string NULL_VALUE_TEXT = "(null)";

        private sealed class RowCol
        {
            private readonly int m_Col;
            private readonly int m_Row;
            public int Col
            {
                get
                {
                    return m_Col;
                }
            }
            public int Row
            {
                get
                {
                    return m_Row;
                }
            }
            public RowCol(int row, int col)
            {
                m_Row = row;
                m_Col = col;
            }
        }
        private sealed class ChoiceContainer
        {
            private readonly OperationParameterTree m_Branch;
            private readonly int m_Column;
            private readonly int m_Row;
            [TypeConverter(typeof(ChoiceConverter))]
            public string Choice
            {
                get
                {
                    return m_Branch.GetText(m_Row, m_Column);
                }
                set
                {
                    if (new List<string>(ChoiceConverter.StaticChoices).Contains(value))
                    {
                        m_Branch.CommitLabelEdit(m_Row, m_Column, value);
                        return;
                    }
                    m_Branch.CommitLabelEdit(m_Row, m_Column, null);
                }
            }
            internal ChoiceContainer(OperationParameterTree branch, int row, int column)
            {
                m_Branch = branch;
                m_Row = row;
                m_Column = column;
            }
        }
        private sealed class ChoiceConverter : StringConverter
        {
            public static string[] StaticChoices;

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(StaticChoices);
            }
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return false;
            }
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
        }

        private readonly int m_ColumnCount;
        private readonly ITree m_VirtualTree;
        private readonly VirtualTreeControl m_VirtualTreeControl;
        private readonly IList<MemberInfo> m_Parameters;
        private readonly bool m_ReadOnly;
        private readonly OperationParameterTree m_Parent;
        private readonly OperationParameterTree[] m_Children;
        private int m_RelativeRow;

        public IList<MemberInfo> Parameters
        {
            get
            {
                return m_Parameters;
            }
        }

        public OperationParameterTree[] Children
        {
            get
            {
                return m_Children;
            }
        }

        public int ColumnCount
        {
            get { return m_ColumnCount; }
        }

        public OperationParameterTree(int columnCount, ITree virtualTree, VirtualTreeControl virtualTreeControl, IList<MemberInfo> parameters, bool readOnly, OperationParameterTree parent)
        {
            m_ColumnCount = columnCount;
            m_VirtualTree = virtualTree;
            m_VirtualTreeControl = virtualTreeControl;
            m_Parameters = parameters;
            m_ReadOnly = readOnly;
            m_Parent = parent;
            m_Children = new OperationParameterTree[parameters.Count];
        }

        public SubItemCellStyles ColumnStyles(int column)
        {
            if (column != 0)
            {
                return SubItemCellStyles.Simple;
            }
            return SubItemCellStyles.Expandable;
        }

        public int GetJaggedColumnCount(int row)
        {
            return 0;
        }

        public VirtualTreeLabelEditData BeginLabelEdit(int row, int column, VirtualTreeLabelEditActivationStyles activationStyle)
        {
            if (column != 1)
            {
                return VirtualTreeLabelEditData.Invalid;
            }
            if (m_ReadOnly)
            {
                return VirtualTreeLabelEditData.Invalid;
            }
            MemberInfo memberInfo = m_Parameters[row];
            VirtualTreeLabelEditData @default = VirtualTreeLabelEditData.Default;
            if (memberInfo.Member.IsClass)
            {
                ChoiceContainer choiceContainer = new ChoiceContainer(this, row, column);
                ChoiceConverter.StaticChoices = new[] { NULL_VALUE_TEXT, memberInfo.Member.Type.FullName };
                PropertyDescriptor propertyDescriptor2 = TypeDescriptor.GetProperties(choiceContainer)["Choice"];
                @default.CustomInPlaceEdit = TypeEditorHost.Create(propertyDescriptor2, choiceContainer);
            }

            return @default;
        }

        public LabelEditResult CommitLabelEdit(int row, int column, string newText)
        {
            MemberInfo memberInfo = m_Parameters[row];
            if (m_ReadOnly)
            {
                m_VirtualTreeControl.EndLabelEdit(true);
                return LabelEditResult.CancelEdit;
            }

            bool refreshTree = false;
            if(memberInfo.Member.IsClass)
            {
                refreshTree = true;

                if (newText == NULL_VALUE_TEXT)
                {
                    memberInfo.Value = null;
                }
                else
                {
                    memberInfo.Value = DynamicMethodCompilerCache.CreateInstance(memberInfo.Member.Type);
                }
            }
            else
            {
                try
                {
                    memberInfo.Value = ConversionUtils.ChangeType(newText, memberInfo.Member.Type);
                }
                catch
                {
                    m_VirtualTreeControl.EndLabelEdit(true);
                    return LabelEditResult.CancelEdit;
                }
            }

            if (refreshTree)
            {
                m_VirtualTreeControl.BeginUpdate();
                m_RelativeRow = row;
                m_VirtualTree.ListShuffle = true;
                m_VirtualTree.Realign(this);
                m_VirtualTreeControl.EndUpdate();
                m_VirtualTree.ListShuffle = false;
            }
            PropagateValueUpdateEvent();

            return LabelEditResult.AcceptEdit;
        }

        public void PropagateValueUpdateEvent()
        {
            OperationParameterTree operationParameterTree = this;
            while (operationParameterTree.m_Parent != null)
            {
                operationParameterTree = operationParameterTree.m_Parent;
            }
        }

        public BranchFeatures Features
        {
            get
            {
                const BranchFeatures branchFeatures = BranchFeatures.Expansions | BranchFeatures.BranchRelocation | BranchFeatures.Realigns | BranchFeatures.PositionTracking;
                return branchFeatures | BranchFeatures.ImmediateSelectionLabelEdits;
            }
        }

        public VirtualTreeAccessibilityData GetAccessibilityData(int row, int column)
        {
            return default(VirtualTreeAccessibilityData);
        }

        public VirtualTreeDisplayData GetDisplayData(int row, int column, VirtualTreeDisplayDataMasks requiredData)
        {
            return VirtualTreeDisplayData.Empty;
        }

        public object GetObject(int row, int column, ObjectStyle style, ref int options)
        {
            if (style == ObjectStyle.TrackingObject)
            {
                return new RowCol(row, column);
            }
            if (IsExpandable(row, column))
            {
                MemberInfo memberInfo = m_Parameters[row];
                Class @class = memberInfo.Member.Definition as Class;
                return AddChild(row, @class, memberInfo.Value);
            }
            return null;
        }

        public OperationParameterTree AddChild(int row, Class @class, object value)
        {
            return AddChild(
                row,
                @class == null
                    ? new List<MemberInfo>(0)
                    : @class.Properties.Select(
                        x =>
                        new MemberInfo
                            {
                                Member = x,
                                Value = value == null ? null : DynamicMethodCompilerCache.GetPropertyValue(value, x.Name)
                            }).ToList());
        }

        public OperationParameterTree AddChild(int row, IList<MemberInfo> memberInfos)
        {
            return
                m_Children[row] =
                new OperationParameterTree(
                    m_ColumnCount, m_VirtualTree, m_VirtualTreeControl, memberInfos, m_ReadOnly, this);
        }

        public string GetText(int row, int column)
        {
            MemberInfo memberInfo = m_Parameters[row];
            if (column == 0)
            {
                return memberInfo.Member.Name;
            }
            if (column == 1)
            {
                if(memberInfo.Member.IsClass)
                {
                    return memberInfo.Value == null ? NULL_VALUE_TEXT : memberInfo.Member.Type.FullName;
                }
                return ConversionUtils.ChangeType<string>(memberInfo.Value);
            }
            if (column == 2)
            {
                return memberInfo.Member.Type.FullName;
            }

            return string.Empty;
        }

        public string GetTipText(int row, int column, ToolTipType tipType)
        {
            return GetText(row, column);
        }

        public bool IsExpandable(int row, int column)
        {
            if (column != 0)
            {
                return false;
            }
            MemberInfo memberInfo = m_Parameters[row];
            Class @class = memberInfo.Member.Definition as Class;
            return @class != null && memberInfo.Value != null && @class.Properties.Count > 0;
        }

        public LocateObjectData LocateObject(object obj, ObjectStyle style, int locateOptions)
        {
            LocateObjectData result = default(LocateObjectData);
            if (style == ObjectStyle.TrackingObject)
            {
                RowCol rowCol = (RowCol)obj;
                result.Row = rowCol.Row;
                result.Column = rowCol.Col;
                result.Options = 1;
            }
            else
            {
                if (style == ObjectStyle.ExpandedBranch)
                {
                    OperationParameterTree parameterTreeAdapter = (OperationParameterTree)obj;
                    OperationParameterTree parameterTreeAdapter2 = parameterTreeAdapter.m_Parent;
                    result.Row = -1;
                    for (int i = 0; i < parameterTreeAdapter2.m_Children.Length; i++)
                    {
                        if (parameterTreeAdapter2.m_Children[i] == parameterTreeAdapter)
                        {
                            result.Row = i;
                        }
                    }

                    result.Column = 0;
                    result.Options = result.Row == m_RelativeRow ? 0 : 1;
                }
            }
            return result;
        }

        public event BranchModificationEventHandler OnBranchModification;

        public void OnDragEvent(object sender, int row, int column, DragEventType eventType, DragEventArgs args)
        {
        }

        public void OnGiveFeedback(GiveFeedbackEventArgs args, int row, int column)
        {
        }

        public void OnQueryContinueDrag(QueryContinueDragEventArgs args, int row, int column)
        {
        }

        public VirtualTreeStartDragData OnStartDrag(object sender, int row, int column, DragReason reason)
        {
            return VirtualTreeStartDragData.Empty;
        }

        public StateRefreshChanges SynchronizeState(int row, int column, IBranch matchBranch, int matchRow, int matchColumn)
        {
            return StateRefreshChanges.None;
        }

        public StateRefreshChanges ToggleState(int row, int column)
        {
            return StateRefreshChanges.None;
        }

        public int UpdateCounter
        {
            get { return 0; }
        }

        public int VisibleItemCount
        {
            get { return m_Parameters.Count; }
        }
    }
}

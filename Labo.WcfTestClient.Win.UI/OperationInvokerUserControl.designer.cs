using Microsoft.VisualStudio.VirtualTreeGrid;

namespace Labo.WcfTestClient.Win.UI
{
    partial class OperationInvokerUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.inputControl = new Microsoft.VisualStudio.VirtualTreeGrid.VirtualTreeControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnInvoke = new System.Windows.Forms.Button();
            this.outputControl = new Microsoft.VisualStudio.VirtualTreeGrid.VirtualTreeControl();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputControl
            // 
            this.inputControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputControl.EnableExplorerTheme = true;
            this.inputControl.HasGridLines = true;
            this.inputControl.HasHorizontalGridLines = true;
            this.inputControl.HasLines = false;
            this.inputControl.HasRootLines = false;
            this.inputControl.HasVerticalGridLines = true;
            this.inputControl.LabelEditSupport = Microsoft.VisualStudio.VirtualTreeGrid.VirtualTreeLabelEditActivationStyles.ImmediateSelection;
            this.inputControl.Location = new System.Drawing.Point(0, 30);
            this.inputControl.Name = "inputControl";
            this.inputControl.Size = new System.Drawing.Size(562, 142);
            this.inputControl.TabIndex = 2;
            this.inputControl.Text = "virtualTreeControl1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.inputControl);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.outputControl);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Size = new System.Drawing.Size(562, 398);
            this.splitContainer1.SplitterDistance = 199;
            this.splitContainer1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(562, 30);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Parameters";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnInvoke);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 172);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(562, 27);
            this.panel2.TabIndex = 4;
            // 
            // btnInvoke
            // 
            this.btnInvoke.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnInvoke.Location = new System.Drawing.Point(487, 0);
            this.btnInvoke.Name = "btnInvoke";
            this.btnInvoke.Size = new System.Drawing.Size(75, 27);
            this.btnInvoke.TabIndex = 0;
            this.btnInvoke.Text = "Invoke";
            this.btnInvoke.UseVisualStyleBackColor = true;
            this.btnInvoke.Click += new System.EventHandler(this.btnInvoke_Click);
            // 
            // outputControl
            // 
            this.outputControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputControl.EnableExplorerTheme = true;
            this.outputControl.HasLines = false;
            this.outputControl.HasRootLines = false;
            this.inputControl.HasGridLines = true;
            this.inputControl.HasVerticalGridLines = true;
            this.inputControl.LabelEditSupport = Microsoft.VisualStudio.VirtualTreeGrid.VirtualTreeLabelEditActivationStyles.ImmediateSelection;
            this.outputControl.Location = new System.Drawing.Point(0, 28);
            this.outputControl.Name = "outputControl";
            this.outputControl.Size = new System.Drawing.Size(562, 167);
            this.outputControl.TabIndex = 1;
            this.outputControl.Text = "virtualTreeControl1";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(562, 28);
            this.panel3.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Result";
            // 
            // OperationInvokerUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "OperationInvokerUserControl";
            this.Size = new System.Drawing.Size(562, 398);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private VirtualTreeControl inputControl;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnInvoke;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private VirtualTreeControl outputControl;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;


    }
}

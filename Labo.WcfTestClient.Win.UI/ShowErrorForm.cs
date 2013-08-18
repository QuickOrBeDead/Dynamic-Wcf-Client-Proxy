using System;
using System.Windows.Forms;

namespace Labo.WcfTestClient.Win.UI
{
    public partial class ShowErrorForm : Form
    {
        public ShowErrorForm(Form owner, Exception exception)
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            Owner = owner;

            //txtError.Text = ExceptionUtils.GetExceptionDetails(exception);
        }

        public static DialogResult ShowDialog(Form owner, Exception exception)
        {
            ShowErrorForm showErrorForm = new ShowErrorForm(owner, exception);
            return showErrorForm.ShowDialog(owner);
        }
    }
}

using System.Windows.Forms;

namespace Labo.WcfTestClient.Win.UI
{
    public partial class ServiceConfigUserControl : UserControl
    {
        public ServiceConfigUserControl(string config)
        {
            InitializeComponent();

            txtConfig.Text = config;
        }
    }
}

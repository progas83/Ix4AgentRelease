using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ix4ServiceConfigurator.Controls
{
    /// <summary>
    /// Interaction logic for CustomerInfoControl.xaml
    /// </summary>
    public partial class CustomerInfoControl : UserControl, ViewModel.IPwd
    {
        public CustomerInfoControl()
        {
            InitializeComponent();
            
        }

        public SecureString PasswordGet
        {
            get
            {
                return this.UIPwdBox.SecurePassword;
            }
        }


        public void PasswordSet(string val)
        {
            this.UIPwdBox.Password = val;
        }

    }
}

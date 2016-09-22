using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for MailSettingsControl.xaml
    /// </summary>
    public partial class MailSettingsControl : UserControl, ViewModel.IPwd
    {
       

        public MailSettingsControl()
        {
            InitializeComponent();
        }

        private void PreviewDigitInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDigit(e.Text);
        }

        private bool IsDigit(string text)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(text);
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

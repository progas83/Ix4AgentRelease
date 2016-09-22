using Ix4ServiceConfigurator.ViewModel;
using System;

using System.Windows;


namespace Ix4ServiceConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();

            UIMainCustomerInfo.PasswordSet(_viewModel.Customer.Password);
            this.DataContext = _viewModel;

            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.Dispose();
            }
        }
    }
}

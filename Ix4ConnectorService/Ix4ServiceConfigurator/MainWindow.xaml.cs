using Ix4ServiceConfigurator.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;

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
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            UIMainCustomerInfo.PasswordSet(_viewModel.Customer.Password);
            this.DataContext = _viewModel;
            InitLanguages();
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("Customer"))
            {
                UIMainCustomerInfo.PasswordSet(_viewModel.Customer.Password);
            }
        }

        private void InitLanguages()
        {
            MenuItem langMenuItem = null;
            foreach(CultureInfo cultureInfo in  Locale.CultureResources.SupportedCultures)
            {
                langMenuItem = new MenuItem();
                langMenuItem.Header = cultureInfo.DisplayName;
                langMenuItem.Click += LangMenuItem_Click;
                langMenuItem.Tag = cultureInfo;
                langMenuItem.IsChecked = IsLanguageSelected(cultureInfo);
                langMenuItem.Template = (ControlTemplate)TryFindResource("MenuItemRadioButtonTemplate");
                this.UILanguages.Items.Add(langMenuItem);
            }
        }

        private bool IsLanguageSelected(CultureInfo culture)
        {
            bool result = false;
            if(Properties.Settings.Default.Language.Equals(culture.Name))
            {
                result = true;
                SetCulture(culture);
            }
            return result;
        }

        private void LangMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            CultureInfo culture = menuItem.Tag as CultureInfo;
            SetCulture(culture);
            Properties.Settings.Default.Language = culture.Name;
            Properties.Settings.Default.Save();
        }

        private void SetCulture(CultureInfo culture)
        {
            Locale.CultureResources.ChangeCulture(culture);
            ((ObjectDataProvider)App.Current.FindResource("Localization")).Refresh();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                _viewModel.Dispose();
            }
            Properties.Settings.Default.Save();
        }
    }
}

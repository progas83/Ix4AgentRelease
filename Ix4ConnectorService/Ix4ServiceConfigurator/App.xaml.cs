using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Ix4ServiceConfigurator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitResources();
            var processExists = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
            if (processExists)
            {
                Process[] prsss = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location));
                MessageBox.Show(Locale.Properties.Resources.CurrentProcessExist);
                base.Shutdown();
            }
        }

        private void InitResources()
        {
            ResourceDictionary resource = new ResourceDictionary();
            Uri url = new Uri("pack://application:,,,/Style/ResourceDictionary.xaml", UriKind.Absolute);
            resource.Source = url;
            Application.Current.Resources.MergedDictionaries.Add(resource);
        }
    }
}

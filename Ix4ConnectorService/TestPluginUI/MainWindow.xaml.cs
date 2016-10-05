using ConnectorWorkflowManager;
using System.Windows;

namespace TestPluginUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WorkflowManager.Instance.Start();

            //MsSqlCustomerDataExtractor test = new MsSqlCustomerDataExtractor();
            //test.GetControlForSettings();
            //test.GetControlForSettings();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WorkflowManager.Instance.Stop();
        }
    }
}

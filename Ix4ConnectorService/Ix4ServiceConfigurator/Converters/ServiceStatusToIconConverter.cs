using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ix4ServiceConfigurator.Converters
{
    public class ServiceStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ServiceControllerStatus serviceStatus = ServiceControllerStatus.Stopped;
            if(value!=null && value is ServiceControllerStatus)
            {
                serviceStatus = (ServiceControllerStatus)value;
            }

            return GetStatusIconPath(serviceStatus);
        }

        private string GetStatusIconPath(ServiceControllerStatus status)
        {
            string statusIcon = @"pack://application:,,,/Icons/red-ball.gif";
            switch (status)
            {
                case (ServiceControllerStatus.Running):
                    statusIcon = @"pack://application:,,,/Icons/green-ball.gif";
                    break;
                case (ServiceControllerStatus.Paused):
                    statusIcon = @"pack://application:,,,/Icons/yellow-ball.gif";
                    break;
                default:
                    break;
            }
            return statusIcon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

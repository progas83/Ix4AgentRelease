using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;


namespace Ix4ServiceConfigurator.Converters
{
    public class BoolToInstallStateConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isServiceExist = (bool)value;
            string result = string.Empty;
            if(isServiceExist)
            {
                result = "Uninstall";
            }
            else
            {
                result = "Install";
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

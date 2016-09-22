using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Locale
{
    public class CultureResources
    {
        private static bool _initInstalledCultures = false;
        private static List<CultureInfo> _supportedCultures = new List<CultureInfo>();

        public static List<CultureInfo> SupportedCultures
        {
            get { return _supportedCultures; }
        }

        public CultureResources()
        {
            CultureInfo cu = new CultureInfo("de-DE");
            if (!_initInstalledCultures)
            {
                try
                {
                    foreach (string dir in Directory.GetDirectories(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(dir);
                        if (dirInfo.GetFiles("Locale.resources.dll").Length > 0)
                        {
                            _supportedCultures.Add(CultureInfo.GetCultureInfo(dirInfo.Name));
                        }
                    }
                    _initInstalledCultures = true;
                }
                catch (Exception ex)
                {

                }

            }
        }
        public Properties.Resources GetResourceInstance()
        {
            return new Properties.Resources();
        }

        public static void ChangeCulture(CultureInfo culture)
        {
            if (SupportedCultures.Contains(culture))
            {
                Properties.Resources.Culture = culture;
            }
        }
    }
}

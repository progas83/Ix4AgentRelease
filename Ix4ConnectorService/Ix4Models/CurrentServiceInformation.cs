namespace Ix4Models
{
    public class CurrentServiceInformation
    {
        private static readonly string _serviceDescription = "Navitel to ix4 adapter service";
        public const string ServiceName = "Ix4-Agent";

        public static string ServiceDescription { get { return _serviceDescription; } }

        public const string NameForPluginMetadata = "ConnectorMetadata";

        public const string PluginsSubdirectory = "Plugins";
        public const string ClientsSubdirectory = "Clients";


        public const string CustomDataSourceTypeMsSql = "MsSql";
        public const string CustomDataSourceTypeCsv = "Csv";
        public const string CustomDataSourceTypeXml = "Xml";

        public const string MsSqlDatabaseConnectionStringWindowsAuth = @"Data Source ={0};Initial Catalog = {1};Integrated Security=SSPI";
        // public const string MsSqlDatabaseConnectionStringWithServerAuth = @"Server={0};Network Library=DBMSSOCN; Database = {1}; User Id= {2}; Password={3};";
        public const string MsSqlDatabaseConnectionStringWithServerAuth = @"Data Source={0};Network Library=DBMSSOCN;Initial Catalog={1};User ID={2};Password={3}";

        private static readonly string _xmlFileName = string.Format("{0}{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "\\configuration.xml");// @"C:\Ilya\ServiceProgram\configuration.xml";// "configuration.xml";
        public static string FileName { get { return _xmlFileName; } }

        private static readonly string _temporaryXmlFileName = string.Format("{0}{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "\\tmp.xml");// @"C:\Ilya\ServiceProgram\configuration.xml";// "configuration.xml";
        public static string TemporaryXmlFileName { get { return _temporaryXmlFileName; } }

        private static readonly string _floatTemporaryXmlFileName = string.Format("{0}{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "\\tmp{0}.xml");// @"C:\Ilya\ServiceProgram\configuration.xml";// "configuration.xml";
        public static string FloatTemporaryXmlFileName { get { return _floatTemporaryXmlFileName; } }



    }
}

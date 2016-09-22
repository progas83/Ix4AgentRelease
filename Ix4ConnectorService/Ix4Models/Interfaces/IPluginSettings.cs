namespace Ix4Models.Interfaces
{
    public interface IPluginSettings
    {
        CustomDataSourceTypes PluginType { get; }
        bool IsActivated { get; set; }
        bool CheckArticles
        {
            get; set;
        }

        bool CheckOrders
        {
            get; set;
        }

        bool CheckDeliveries
        {
            get; set;
        }
    }
}

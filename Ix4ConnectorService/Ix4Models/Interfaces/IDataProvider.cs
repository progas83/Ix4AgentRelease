using Ix4Models.SettingsDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ix4Models.Interfaces
{
   public interface IDataProvider
    {
        List<LICSRequestArticle> GetArticles(BaseDataSourceSettings settings);
        List<LICSRequestDelivery> GetDeliveries(BaseDataSourceSettings settings);
        List<LICSRequestOrder> GetOrders(BaseDataSourceSettings settings);

     //   LICSRequest GetData(BaseDataSourceSettings settings);

    }
}

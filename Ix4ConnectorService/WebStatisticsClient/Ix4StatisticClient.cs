using Ix4Models.Reports;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebStatisticsClient
{
   public class Ix4StatisticClient
    {
        private static HttpClient _client = new HttpClient();
        public Ix4StatisticClient(string baseAddress)
        {
            _client.BaseAddress = new Uri(baseAddress);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async void PostReport(ExportDataReport dataOperationsReport)
        {
            //try
            //{

            //}
            //catch(Exception ex)
            //{

            //}
            if(dataOperationsReport!=null)
            {
                var jsonResult = JsonConvert.SerializeObject(dataOperationsReport);
                using (HttpResponseMessage response = await _client.PostAsJsonAsync("api/operations", jsonResult))
                {

                }
            }
        }
    }
}

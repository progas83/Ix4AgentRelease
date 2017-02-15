using Ix4Models.Reports;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace ConnectorWorkflowManager.WebStatisticsClient
{
    class Ix4StatisticClient
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
            try
            {
                if (dataOperationsReport != null)
                {
                    var jsonResult = JsonConvert.SerializeObject(dataOperationsReport);
                    using (HttpResponseMessage response = await _client.PostAsJsonAsync("api/operations", jsonResult))
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                // throw ex;
            }

        }
    }
}

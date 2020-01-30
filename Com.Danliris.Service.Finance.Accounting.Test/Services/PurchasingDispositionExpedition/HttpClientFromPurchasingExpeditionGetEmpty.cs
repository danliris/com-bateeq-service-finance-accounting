using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.IntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionReport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.PurchasingDispositionExpedition
{
    public class HttpClientFromPurchasingExpeditionGetEmpty : IHttpClientService
    {
        public Task<HttpResponseMessage> GetAsync(string url)
        {
            PurchasingDispositionResponseViewModel response = new PurchasingDispositionResponseViewModel()
            {
                apiVersion = "1.0.0",
                data = new List<PurchasingDispositionViewModel>() {  },
                info = new APIInfo()
                {
                    count = 1,
                    order = new
                    {
                        LastModifiedUtc = "asc"
                    },
                    page = 1,
                    size = 25,
                    total = 1,
                },
                message = "OK",
                statusCode = "200"
            };
            string responseJson = JsonConvert.SerializeObject(response);
            return Task.Run(() => new HttpResponseMessage() { Content = new StringContent(responseJson, Encoding.UTF8, "application/json"), StatusCode = System.Net.HttpStatusCode.OK });
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }

        public Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }
    }
}

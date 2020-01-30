using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.JournalTransaction
{
    public class JournalHttpClientTestService : IHttpClientService
    {
        public static string Token;

        public Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            var datalist = new[] { new { DocumentNo = "ReferenceNo", BankName = "BankName", BGCheckNumber = "CheckNumber" } };
            var json = new
            {
                apiVersion = "1.0.0",
                data = datalist,
                message = "Ok",
                statusCode = "200"
            };
            var jsonString = JsonConvert.SerializeObject(json);
            var jsonContent = new StringContent(jsonString);
            return Task.Run(() => new HttpResponseMessage() { Content = jsonContent });
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            UnitReceiptNoteResult result = new UnitReceiptNoteResult()
            {
                data = new List<UnitReceiptNote>()
            };
            return Task.Run(() => new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(result))});
        }
    }
}

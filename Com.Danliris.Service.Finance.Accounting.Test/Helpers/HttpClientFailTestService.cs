using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.Helpers
{
    public class HttpClientFailTestService : IHttpClientService
    {
        public static string Token;

        public Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError));
        }
        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return Task.Run(() => new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError));
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError));
        }
    }
}

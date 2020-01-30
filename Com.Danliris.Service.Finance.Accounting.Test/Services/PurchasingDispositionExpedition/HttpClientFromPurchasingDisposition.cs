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

    public class HttpClientFromPurchasingDisposition : IHttpClientService
    {
        public Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            return Task.Run(() => new HttpResponseMessage());
        }
        public Task<HttpResponseMessage> GetAsync(string url)
        {
            var vm = GetPurchasingDispositionViewModel();
            string vmJson = JsonConvert.SerializeObject(vm);
            PurchasingDispositionResponseViewModel response = new PurchasingDispositionResponseViewModel()
            {
                apiVersion = "1.0.0",
                data = new List<PurchasingDispositionViewModel>() { vm },
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

        public PurchasingDispositionViewModel GetPurchasingDispositionViewModel()
        {
            return new PurchasingDispositionViewModel()
            {
                Active = true,
                LastModifiedUtc = DateTime.UtcNow,
                Amount = 10000,
                Bank = "Bank",
                Calculation = "Calc",
                ConfirmationOrderNo = "ConfirmationOrderNo",
                CreatedAgent = "createdagent",
                CreatedBy = "dev",
                CreatedUtc = DateTime.UtcNow,
                Currency = new CurrencyViewModel()
                {
                    code = "IDR",
                    description = "IDR",
                    rate = 1,
                    symbol = "RP",
                    _id = "1"
                },
                DispositionNo = "11-11-11-11",
                Id = 1,
                Investation = "Inverst",
                InvoiceNo = "1A1",
                IsDeleted = false,
                LastModifiedAgent = "dev",
                LastModifiedBy = "dev",
                PaymentDueDate = DateTime.UtcNow,
                PaymentMethod = "method",
                Position = 2,
                ProformaNo = "Proforma",
                Remark = "Remark",
                Supplier = new SupplierViewModel()
                {
                    code = "AR",
                    name = "Bengkel",
                    _id = "1"
                },
                Items = new List<PurchasingDispositionItemViewModel>()
                {
                    new PurchasingDispositionItemViewModel()
                    {
                        Active = true,
                        CreatedAgent = "dev",
                        CreatedBy = "dev",
                        CreatedUtc = DateTime.UtcNow,
                        EPOId = "1",
                        EPONo = "11",
                        Id = 1,
                        IncomeTax = new IncomeTaxViewModel()
                        {
                            name = "tax",
                            rate = 1,
                            _id = "1"
                        },
                        IsDeleted = false,
                        LastModifiedAgent = "dev",
                        LastModifiedBy = "dev",
                        LastModifiedUtc = DateTime.UtcNow,
                        UseIncomeTax = true,
                        UseVat = true,
                        Details = new List<PurchasingDispositionDetailViewModel>()
                        {
                            new PurchasingDispositionDetailViewModel()
                            {
                                Active = true,
                                Category = new CategoryViewModel()
                                {
                                    code = "cc",
                                    name = "name",
                                    _id = "1"
                                },
                                CreatedAgent = "dev",
                                CreatedBy = "dev",
                                CreatedUtc = DateTime.UtcNow,
                                DealQuantity = 1,
                                DealUom = new UomViewModel()
                                {
                                    unit = "mtr",
                                    _id ="1"
                                },
                                Id = 1,
                                IsDeleted = false,
                                LastModifiedAgent = "dev",
                                LastModifiedBy = "dev",
                                LastModifiedUtc = DateTime.UtcNow,
                                PaidPrice = 10000,
                                PaidQuantity = 1,
                                PricePerDealUnit = 10000,
                                PriceTotal = 10000,
                                PRId  ="1",
                                PRNo = "11",
                                Product = new ProductViewModel()
                                {
                                    code = "code",
                                    name = "naem",
                                    price = 10000,
                                    uom = new UomViewModel()
                                    {
                                        unit = "mtr",
                                        _id = "1"
                                    },
                                    _id = "1"
                                },
                                Unit = new UnitViewModel()
                                {
                                    _id = "1",
                                    name = "name",
                                    code = "code",
                                    division = new DivisionViewModel()
                                    {
                                        code = "code",
                                        name = "name",
                                        _id = "1"
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}

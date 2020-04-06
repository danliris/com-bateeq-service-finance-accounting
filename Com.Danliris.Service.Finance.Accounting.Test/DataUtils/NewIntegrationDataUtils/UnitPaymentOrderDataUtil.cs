using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.IntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.NewIntegrationDataUtils
{
    public class UnitPaymentOrderDataUtil
    {
        public UnitPaymentOrderViewModel GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;
            var data = new UnitPaymentOrderViewModel {
                Active = true,
                CreatedAgent = null,
                CreatedBy = null,
                CreatedUtc = new DateTime(),
                LastModifiedAgent = null,
                LastModifiedBy = null,
                LastModifiedUtc = new DateTime(),
                Id = 1,
                incomeTaxBy = null,
                IsCorrection = false,
                IsDeleted = false,
                isPaid = false,
                UId = null,
                no = "UPONo",
                division = new DivisionViewModel
                {
                    code = "DivisionCode",
                    name = "DivisionName",
                    _id = "DivisionId"
                },
                supplier = new SupplierViewModel
                {
                    code = "SupplierCode",
                    name = "SupplierName",
                    _id = "SupplierId"
                },

                date = new DateTimeOffset(),
                category = new CategoryViewModel
                {
                    code = "CategoryCode",
                    name = "CategoryName",
                    _id = "CategoryId ",
                },
                currency = new CurrencyViewModel
                {
                    code = "CurrencyCode",
                    rate = 5,
                    _id = "CurrencyId"
                },

                paymentMethod = "CASH",
                incomeTax = new IncomeTaxUPOViewModel
                {
                    name = null,
                    rate = "0",
                    _id = null
                },
                invoiceNo = "INV000111",
                invoiceDate = new DateTimeOffset(),
                pibNo = null,

                useIncomeTax = false,
                incomeTaxNo = null,
                incomeTaxDate = null,

                useVat = false,
                vatNo = null,
                vatDate = new DateTimeOffset(),
                position = 1,
                remark = null,

                dueDate = new DateTimeOffset(),
                
                items = new List<UnitPaymentOrderItemViewModel> {
                    new UnitPaymentOrderItemViewModel{
                        unitReceiptNote = new UnitReceiptNote
                        {
                            _id = 1,
                            no = "URNNo",
                            items = new List<UnitPaymentOrderDetailViewModel>
                            {
                                new UnitPaymentOrderDetailViewModel
                                {
                                    EPONo = "EPONo",
                                    URNItemId = 1,

                                    EPODetailId = 1,
                                    PRId = 1,
                                    PRNo = "1",
                                    PRItemId = 1,
                                    product = new ProductViewModel
                                    {
                                        code = "code",
                                        name = "name",
                                        _id = "1"
                                    },

                                    deliveredQuantity = 1,
                                    deliveredUom = new UomViewModel
                                    {
                                        unit = "unit",
                                        _id = "1"
                                    },

                                    PricePerDealUnitCorrection = 1,
                                    PriceTotal = 1,
                                    QuantityCorrection = 0,

                                    ProductRemark = "Product Remark",
                                    Active = true,
                                    CreatedAgent = null,
                                    CreatedBy = null,
                                    CreatedUtc = new DateTime(),
                                    LastModifiedAgent = null,
                                    LastModifiedBy = null,
                                    LastModifiedUtc = new DateTime(),
                                    Id = 1,
                                    IsDeleted = false,
                                    POItemId = 1,
                                    pricePerDealUnit = 0,
                                    PriceTotalCorrection = 0,
                                }
                            },
                            deliveryOrder = new DeliveyOrder
                            {
                                no = null,
                                _id = 1
                            },
                        },
                        Active = true,
                        CreatedAgent = null,
                        CreatedBy = null,
                        CreatedUtc = new DateTime(),
                        LastModifiedAgent = null,
                        LastModifiedBy = null,
                        LastModifiedUtc = new DateTime(),
                        Id = 1,
                        IsDeleted = false
                    }
                }
            };
            return data;
        }
        public Dictionary<string, object> GetResultFormatterOk()
        {
            var data = GetNewData();
            var dat2 = GetNewData();
            List<UnitPaymentOrderViewModel> upo = new List<UnitPaymentOrderViewModel>();
            upo.Add(data);
            //upo.Add(dat2);

            Dictionary<string, object> result = new Dictionary<string, object>();

            result.Add("apiVersion", "1.0");
            result.Add("statusCode", General.OK_STATUS_CODE);
            result.Add("message", General.OK_MESSAGE);
            result.Add("data", upo);

            return result;
        }
        public string GetResultFormatterOkString()
        {
            var result = GetResultFormatterOk();

            return JsonConvert.SerializeObject(result);
        }
    }
}

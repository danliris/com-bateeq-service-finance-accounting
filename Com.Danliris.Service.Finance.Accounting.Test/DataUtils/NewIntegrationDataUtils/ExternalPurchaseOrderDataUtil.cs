using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.IntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.NewIntegrationDataUtils
{
    public class ExternalPurchaseOrderDataUtil
    {
        public ExternalPurchaseOrderViewModel GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;
            var data = new ExternalPurchaseOrderViewModel
            {

                currency = new CurrencyViewModel
                {
                    code = "CurrencyCode",
                    description = "description",
                    rate = 0.5,
                    _id = "CurrencyId"
                },
                unit = new UnitViewModel
                {
                    code = "UnitCode",
                    division = new DivisionViewModel
                    {
                        code = "DivisionCode",
                        name = "DivisionName",
                        _id = "DivisionId"
                    },
                    name = "UnitName",
                    _id = "UnitId"

                },
                no = "EPONo",
                freightCostBy = "test",
                deliveryDate = DateTime.Now.AddDays(1),
                orderDate = DateTime.Now,
                supplier = new SupplierViewModel
                {
                    code = "sup",
                    name = "Supplier",
                    _id = "supId"
                },
                incomeTax = new IncomeTaxViewModelEPO
                {
                    name = "Final",
                    rate = "1.5",

                },
                useIncomeTax = true,
                incomeTaxBy = "Supplier",
                paymentMethod = "test",
                remark = "Remark",
                Active = true,
                CreatedAgent = null,
                isClosed = false,
                CreatedBy = null,
                CreatedUtc= new DateTime(),
                division = new DivisionViewModel
                {
                    code = null,
                    name =null,
                    _id =null
                },
                Id = 1,
                isCanceled = false,
                IsDeleted =false,
                isPosted =false,
                LastModifiedAgent =null,
                LastModifiedBy = null,
                LastModifiedUtc = new DateTime(),
                paymentDueDays = null,
                UId = null,
                useVat = false,
                items = new List<ExternalPurchaseOrderItemViewModel>
                {
                    new ExternalPurchaseOrderItemViewModel
                    {
                        poId = 1,
                        poNo = $"PONo{nowTicks}",
                        prId = 1,
                        prNo = $"PRNo{nowTicks}",
                        unit = new UnitViewModel
                        {
                            code = "unitcode",
                            name = "unit",
                            _id = "unitId"
                        },
                        Active = true,
                        CreatedAgent = null,
                        CreatedBy = null,
                        CreatedUtc= new DateTime(),
                        LastModifiedAgent =null,
                        LastModifiedBy = null,
                        LastModifiedUtc = new DateTime(),
                        Id = 1,
                        IsDeleted = false,
                        details = new List<ExternalPurchaseOrderDetailViewModel>
                        {
                            new ExternalPurchaseOrderDetailViewModel
                            {
                                Active = true,
                                CreatedAgent = null,
                                CreatedBy = null,
                                CreatedUtc= new DateTime(),
                                LastModifiedAgent =null,
                                LastModifiedBy = null,
                                LastModifiedUtc = new DateTime(),
                                dispositionQuantity = 0,
                                Id = 1,
                                doQuantity = 0,
                                includePpn = false,
                                IsDeleted = false,
                                productPrice = 0,
                                poItemId = 1,
                                prItemId = 1,
                                product = new ProductViewModel
                                {
                                    _id = "ProductId",
                                    code = "ProductCode",
                                    name = "ProductName",
                                },
                                defaultQuantity = 1,
                                dealUom = new UomViewModel
                                {
                                    unit = "Uom",
                                    _id = "UomId"
                                },
                                defaultUom = new UomViewModel
                                {
                                    unit = "Uom",
                                    _id = "UomId"
                                },
                                productRemark = "Remark",
                                priceBeforeTax = 1000,
                                pricePerDealUnit = 200,
                                dealQuantity = 1,
                                conversion=1
                            }
                        }
                    }
                }
            };
            return data;
        }
        public Dictionary<string, object> GetResultFormatterOk()
        {
            var data = GetNewData();

            Dictionary<string, object> result = new Dictionary<string, object>();

            result.Add("apiVersion", "1.0");
            result.Add("statusCode", General.OK_STATUS_CODE);
            result.Add("message", General.OK_MESSAGE);
            result.Add("data", data);

            //new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
            //.Ok(data);

            return result;
        }
        public string GetResultFormatterOkString()
        {
            var result = GetResultFormatterOk();

            return JsonConvert.SerializeObject(result);
        }

    }
}

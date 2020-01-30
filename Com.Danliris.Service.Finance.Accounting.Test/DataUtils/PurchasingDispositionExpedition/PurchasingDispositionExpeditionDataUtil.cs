using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.IntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionExpedition;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.PurchasingDispositionExpedition
{
    public class PurchasingDispositionExpeditionDataUtil
    {
        private readonly PurchasingDispositionExpeditionService Service;

        public PurchasingDispositionExpeditionDataUtil(PurchasingDispositionExpeditionService service)
        {
            Service = service;
        }

        public PurchasingDispositionExpeditionModel GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";
            string nowTicksB = $"{nowTicks}b";
            return new PurchasingDispositionExpeditionModel()
            {
                BankExpenditureNoteNo = nowTicksA,
                CashierDivisionBy = nowTicksA,
                CashierDivisionDate = DateTimeOffset.Now,
                CurrencyId = nowTicksA,
                CurrencyCode = nowTicksA,
                PaymentDueDate = DateTimeOffset.Now,
                ProformaNo = nowTicksA,
                NotVerifiedReason = nowTicksA,
                Position = ExpeditionPosition.CASHIER_DIVISION,
                SendToCashierDivisionBy = nowTicksA,
                SendToCashierDivisionDate = DateTimeOffset.Now,
                SendToPurchasingDivisionBy = nowTicksA,
                SendToPurchasingDivisionDate = DateTimeOffset.Now,
                SupplierCode = nowTicksA,
                SupplierName = nowTicksA,
                TotalPaid = nowTicks,
                DispositionId = nowTicksA,
                DispositionDate = DateTimeOffset.Now,
                DispositionNo = nowTicksA,
                VerificationDivisionBy = nowTicksA,
                VerificationDivisionDate = DateTimeOffset.Now,
                VerifyDate = DateTimeOffset.Now,
                UseIncomeTax = false,
                IncomeTaxId = nowTicksA,
                IncomeTaxName = nowTicksA,
                IncomeTaxRate = nowTicks,
                IsPaid = false,
                IsPaidPPH = false,
                UseVat = false,
                BankExpenditureNoteDate = DateTimeOffset.Now,
                BankExpenditureNotePPHDate = DateTimeOffset.Now,
                BankExpenditureNotePPHNo = nowTicksA,
                PaymentMethod = nowTicksA,
                CategoryId=nowTicksA,
                CategoryCode = nowTicksA,
                CategoryName = nowTicksA,
                DivisionCode=nowTicksA,
                DivisionId=nowTicksA,
                DivisionName=nowTicksA,
                SupplierId=nowTicksA,
                VatValue=1000,
                IncomeTaxValue=100,
                DPP=10000,
                PayToSupplier=10000,

                Items = new List<PurchasingDispositionExpeditionItemModel>
                {
                    new PurchasingDispositionExpeditionItemModel
                    {
                        Price = nowTicks,
                        ProductId = nowTicksA,
                        ProductCode = nowTicksA,
                        ProductName = nowTicksA,
                        Quantity = nowTicks,
                        UnitId = nowTicksA,
                        UnitCode = nowTicksA,
                        UnitName = nowTicksA,
                        UomUnit = nowTicksA,
                        UomId = nowTicksA,
                        PurchasingDispositionDetailId = (int)nowTicks,
                        EPOId=nowTicksA
                    }
                }
            };
        }

        public PurchasingDispositionExpeditionViewModel GetNewViewModel()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;
            string nowTicksA = $"{nowTicks}a";
            string nowTicksB = $"{nowTicks}b";
            return new PurchasingDispositionExpeditionViewModel()
            {
                bankExpenditureNoteNo = nowTicksA,
                cashierDivisionBy = nowTicksA,
                cashierDivisionDate = DateTimeOffset.Now,
                currency = new CurrencyViewModel
                {
                    _id = nowTicksA,
                    code = nowTicksA,
                },
                paymentDueDate = DateTimeOffset.Now,
                proformaNo = nowTicksA,
                notVerifiedReason = nowTicksA,
                position = (int)nowTicks,
                sendToCashierDivisionBy = nowTicksA,
                sendToCashierDivisionDate = DateTimeOffset.Now,
                sendToPurchasingDivisionBy = nowTicksA,
                sendToPurchasingDivisionDate = DateTimeOffset.Now,
                supplier = new SupplierViewModel
                {
                    _id= nowTicksA,
                    code = nowTicksA,
                    name = nowTicksA,
                },
                totalPaid = nowTicks,
                dispositionId = nowTicksA,
                dispositionDate = DateTimeOffset.Now,
                dispositionNo = nowTicksA,
                verificationDivisionBy = nowTicksA,
                verificationDivisionDate = DateTimeOffset.Now,
                verifyDate = DateTimeOffset.Now,
                useIncomeTax = false,
                incomeTax = nowTicks,
                incomeTaxVm = new IncomeTaxViewModel
                {
                    _id = nowTicksA,
                    name = nowTicksA,
                    rate = nowTicks
                },
                isPaid = false,
                isPaidPPH = false,
                useVat = false,
                vat = nowTicks,
                bankExpenditureNoteDate = DateTimeOffset.Now,
                bankExpenditureNotePPHDate = DateTimeOffset.Now,
                bankExpenditureNotePPHNo = nowTicksA,
                paymentMethod = nowTicksA,
                payToSupplier=10000,
                category = new CategoryViewModel
                {
                    _id= nowTicksA,
                    name = nowTicksA,
                    code = nowTicksA,
                },
                division=new DivisionViewModel
                {
                    _id= nowTicksA,
                    code= nowTicksA,
                    name= nowTicksA
                },
                vatValue= 1000,
                incomeTaxValue=100,
                dpp=10000,
                items = new List<PurchasingDispositionExpeditionItemViewModel>
                {
                    new PurchasingDispositionExpeditionItemViewModel
                    {
                        price = nowTicks,
                        product = new ProductViewModel
                        {
                            _id = nowTicksA,
                            code = nowTicksA,
                            name = nowTicksA,
                        },
                        quantity = nowTicks,
                        unit = new UnitViewModel
                        {
                            _id = nowTicksA,
                            code = nowTicksA,
                            name = nowTicksA,
                        },
                        uom = new UomViewModel
                        {
                            _id = nowTicksA,
                            unit=nowTicksA
                        },
                        purchasingDispositionDetailId =(int) nowTicks,
                        epoId=nowTicksA
                    }
                }
            };
        }


        public async Task<PurchasingDispositionExpeditionModel> GetTestData()
        {
            PurchasingDispositionExpeditionModel model = GetNewData();
            await Service.CreateAsync(model);
            return await Service.ReadByIdAsync(model.Id);
        }

        public async Task<PurchasingDispositionExpeditionModel> GetTestData2()
        {
            PurchasingDispositionExpeditionModel model = GetNewData();
            await Service.CreateAsync(model);
            PurchasingDispositionExpeditionModel model2 = GetNewData();
            model2.DispositionNo = model.DispositionNo;
            model2.DispositionId = model.DispositionId;
            await Service.CreateAsync(model2);
            return await Service.ReadByIdAsync(model.Id);
        }
    }
}

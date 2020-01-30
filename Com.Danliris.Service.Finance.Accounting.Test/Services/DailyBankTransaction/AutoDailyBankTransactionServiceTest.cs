using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.OthersExpenditureProofDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Test.Services.OthersExpenditureProofDocument.Helper;
using Moq;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.DailyBankTransaction
{
    public class AutoDailyBankTransactionServiceTest
    {
        [Fact]
        public async Task Should_Success_Auto_Create_Disposition_Payment()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IDailyBankTransactionService))).Returns(new DailyBankTransactionServiceHelper());
            var service = new AutoDailyBankTransactionService(serviceProviderMock.Object);
            
            var dispositionModel = new PaymentDispositionNoteModel()
            {
                PaymentDate = DateTimeOffset.Now,
                Items = new List<PaymentDispositionNoteItemModel>()
                {
                    new PaymentDispositionNoteItemModel()
                    {
                        Details = new List<PaymentDispositionNoteDetailModel>()
                        {
                            new PaymentDispositionNoteDetailModel()
                        }
                    }
                }
            };
            var result = await service.AutoCreateFromPaymentDisposition(dispositionModel);
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Auto_Revert_Disposition_Payment()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IDailyBankTransactionService))).Returns(new DailyBankTransactionServiceHelper());
            var service = new AutoDailyBankTransactionService(serviceProviderMock.Object);
            
            var dispositionModel = new PaymentDispositionNoteModel()
            {
                PaymentDate = DateTimeOffset.Now,
                Items = new List<PaymentDispositionNoteItemModel>()
                {
                    new PaymentDispositionNoteItemModel()
                    {
                        Details = new List<PaymentDispositionNoteDetailModel>()
                        {
                            new PaymentDispositionNoteDetailModel()
                        }
                    }
                }
            };
            var result = await service.AutoRevertFromPaymentDisposition(dispositionModel);
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Auto_Create_Others_Bank_Expenditure()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IDailyBankTransactionService))).Returns(new DailyBankTransactionServiceHelper());
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());
            
            var service = new AutoDailyBankTransactionService(serviceProviderMock.Object);
            
            var model = new OthersExpenditureProofDocumentModel();
            var itemModels = new List<OthersExpenditureProofDocumentItemModel>();
            
            var result = await service.AutoCreateFromOthersExpenditureProofDocument(model, itemModels);
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Auto_Revert_Others_Bank_Expenditure()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IDailyBankTransactionService))).Returns(new DailyBankTransactionServiceHelper());
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());
            
            var service = new AutoDailyBankTransactionService(serviceProviderMock.Object);
            
            var model = new OthersExpenditureProofDocumentModel();
            var itemModels = new List<OthersExpenditureProofDocumentItemModel>();
            
            var result = await service.AutoRevertFromOthersExpenditureProofDocument(model, itemModels);
            Assert.NotEqual(0, result);
        }
    }

    internal class DailyBankTransactionServiceHelper : IDailyBankTransactionService
    {
        public DailyBankTransactionServiceHelper()
        {
        }

        public Task<int> CreateAsync(DailyBankTransactionModel model)
        {
            return Task.FromResult(1);
        }

        public Task<int> CreateInOutTransactionAsync(DailyBankTransactionModel model)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteByReferenceNoAsync(string referenceNo)
        {
            throw new NotImplementedException();
        }

        public MemoryStream GenerateExcel(int bankId, int month, int year, int clientTimeZoneOffset)
        {
            throw new NotImplementedException();
        }

        public MemoryStream GenerateExcelDailyBalance(int bankId, DateTime startDate, DateTime endDate, int clientTimeZoneOffset)
        {
            throw new NotImplementedException();
        }

        public List<DailyBalanceCurrencyReportViewModel> GetDailyBalanceCurrencyReport(int bankId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public List<DailyBalanceReportViewModel> GetDailyBalanceReport(int bankId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public ReadResponse<DailyBankTransactionModel> GetReport(int bankId, int month, int year, int clientTimeZoneOffset)
        {
            throw new NotImplementedException();
        }

        public ReadResponse<DailyBankTransactionModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            throw new NotImplementedException();
        }

        public Task<DailyBankTransactionModel> ReadByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(int id, DailyBankTransactionModel model)
        {
            throw new NotImplementedException();
        }
    }
}

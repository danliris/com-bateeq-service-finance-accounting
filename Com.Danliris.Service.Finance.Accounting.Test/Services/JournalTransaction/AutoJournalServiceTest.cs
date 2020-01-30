using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;
using Com.Danliris.Service.Finance.Accounting.Test.Services.OthersExpenditureProofDocument.Helper;
using Moq;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.JournalTransaction
{
    public class AutoJournalServiceTest
    {
        [Fact]
        public async Task Should_Success_Auto_Journal_From_Others_Bank_Expenditure()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IJournalTransactionService))).Returns(new JournalTransactionServiceTestHelper());
            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IHttpClientService))).Returns(new HttpClientOthersExpenditureServiceHelper());

            var service = new AutoJournalService(serviceProviderMock.Object);

            var viewModel = new OthersExpenditureProofDocumentCreateUpdateViewModel()
            {
                Items = new List<OthersExpenditureProofDocumentCreateUpdateItemViewModel>()
                {
                    new OthersExpenditureProofDocumentCreateUpdateItemViewModel()
                }
            };

            var result = await service.AutoJournalFromOthersExpenditureProof(viewModel, "any");
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Should_Success_Auto_Journal_From_Others_Bank_Expenditure_Reverse()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(IJournalTransactionService))).Returns(new JournalTransactionServiceTestHelper());

            var service = new AutoJournalService(serviceProviderMock.Object);

            var result = await service.AutoJournalReverseFromOthersExpenditureProof("any");
            Assert.NotEqual(0, result);
        }
    }

    internal class JournalTransactionServiceTestHelper : IJournalTransactionService
    {
        public JournalTransactionServiceTestHelper()
        {
        }

        public Task<int> CreateAsync(JournalTransactionModel model)
        {
            return Task.FromResult(1);
        }

        public Task<int> CreateManyAsync(List<JournalTransactionModel> models)
        {
            return Task.FromResult(1);
        }

        public Task<int> DeleteAsync(int id)
        {
            return Task.FromResult(1);
        }

        public MemoryStream GenerateExcel(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offSet)
        {
            throw new NotImplementedException();
        }

        public Task<List<GeneralLedgerWrapperReportViewModel>> GetGeneralLedgerReport(DateTimeOffset startDate, DateTimeOffset endDate, int timezoneoffset)
        {
            throw new NotImplementedException();
        }

        public Task<MemoryStream> GetGeneralLedgerReportXls(DateTimeOffset startDate, DateTimeOffset endDate, int timezoneoffset)
        {
            throw new NotImplementedException();
        }

        public (ReadResponse<JournalTransactionReportHeaderViewModel>, decimal, decimal) GetReport(int page, int size, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offSet)
        {
            throw new NotImplementedException();
        }

        public Task<SubLedgerReportViewModel> GetSubLedgerReport(int? coaId, int month, int year, int timeoffset)
        {
            throw new NotImplementedException();
        }

        public Task<SubLedgerXlsFormat> GetSubLedgerReportXls(int? coaId, int? month, int? year, int timeoffset)
        {
            throw new NotImplementedException();
        }

        public Task<int> PostTransactionAsync(int id)
        {
            return Task.FromResult(1);
        }

        public Task<int> PostTransactionAsync(int id, JournalTransactionModel model)
        {
            return Task.FromResult(1);
        }

        public ReadResponse<JournalTransactionModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            throw new NotImplementedException();
        }

        public Task<JournalTransactionModel> ReadByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public List<JournalTransactionModel> ReadUnPostedTransactionsByPeriod(int month, int year)
        {
            throw new NotImplementedException();
        }

        public Task<int> ReverseJournalTransactionByReferenceNo(string referenceNo)
        {
            return Task.FromResult(1);
        }

        public Task<int> UpdateAsync(int id, JournalTransactionModel model)
        {
            return Task.FromResult(1);
        }
    }
}
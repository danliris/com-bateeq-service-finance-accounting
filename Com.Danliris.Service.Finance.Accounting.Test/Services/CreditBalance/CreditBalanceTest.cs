using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CreditBalance;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.CreditBalance
{
    public class CreditBalanceTest
    {
        private const string ENTITY = "CreditBalance";
        //private PurchasingDocumentAcceptanceDataUtil pdaDataUtil;
        //private readonly IIdentityService identityService;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private FinanceDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<FinanceDbContext> optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            FinanceDbContext dbContext = new FinanceDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private CreditorAccountDataUtil _dataUtil(CreditorAccountService service)
        {

            GetServiceProvider();
            return new CreditorAccountDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });


            return serviceProvider;
        }

        [Fact]
        public async Task Should_Success_Get_Report()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            CreditBalanceService creditBalanceService = new CreditBalanceService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            data.InvoiceNo = unitData.InvoiceNo;
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);

            var reportResponse = creditBalanceService.GetReport(false, 1, 25, data.SupplierName, data.Date.Month, data.Date.Year, 7, false);
            Assert.NotEmpty(reportResponse.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Report_ForeignCurrency()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            CreditBalanceService creditBalanceService = new CreditBalanceService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            unitData.Currency = "USD";
            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            data.InvoiceNo = unitData.InvoiceNo;
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);

            var reportResponse = creditBalanceService.GetReport(false, 1, 25, data.SupplierName, data.Date.Month, data.Date.Year, 7, true);
            Assert.NotEmpty(reportResponse.Data);
        }


        [Fact]
        public async Task Should_Success_Get_Report_January()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            CreditBalanceService creditBalanceService = new CreditBalanceService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            unitData.Date = new DateTimeOffset(unitData.Date.Year, 1, unitData.Date.Day, unitData.Date.Hour, unitData.Date.Minute, unitData.Date.Second, unitData.Date.Offset);
            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            data.InvoiceNo = unitData.InvoiceNo;
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);

            var reportResponse = creditBalanceService.GetReport(true, 1, 25, unitData.SupplierName, unitData.Date.Month, unitData.Date.Year, 7, false);
            Assert.NotEmpty(reportResponse.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Excel_Impor()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            CreditBalanceService creditBalanceService = new CreditBalanceService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            data.InvoiceNo = unitData.InvoiceNo;
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);

            var reportResponse = creditBalanceService.GenerateExcel(true, data.SupplierName, data.Date.Month, data.Date.Year, 7, false);
            Assert.NotNull(reportResponse);
        }

        [Fact]
        public async Task Should_Success_Get_Excel_Lokal()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            CreditBalanceService creditBalanceService = new CreditBalanceService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            unitData.SupplierIsImport = false;
            data.InvoiceNo = unitData.InvoiceNo;
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);

            var reportResponse = creditBalanceService.GenerateExcel(false, data.SupplierName, data.Date.Month, data.Date.Year, 7, false);
            Assert.NotNull(reportResponse);
        }

        [Fact]
        public async Task Should_Success_Get_Excel_Empty_Lokal()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            CreditBalanceService creditBalanceService = new CreditBalanceService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            unitData.SupplierIsImport = false;
            data.InvoiceNo = unitData.InvoiceNo;
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);

            var reportResponse = creditBalanceService.GenerateExcel(false, "", data.Date.Month + 1, data.Date.Year + 1, 7, false);
            Assert.NotNull(reportResponse);
        }

        [Fact]
        public async Task Should_Success_Get_Excel_Empty_Impor()
        {
            CreditorAccountService service = new CreditorAccountService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            CreditBalanceService creditBalanceService = new CreditBalanceService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = _dataUtil(service).GetBankExpenditureNotePostedViewModel();
            var unitData = _dataUtil(service).GetUnitReceiptNotePostedViewModel();
            data.SupplierCode = unitData.SupplierCode;
            data.SupplierName = unitData.SupplierName;
            unitData.SupplierIsImport = false;
            data.InvoiceNo = unitData.InvoiceNo;
            var tempResponse = await service.CreateFromUnitReceiptNoteAsync(unitData);
            var Response = await service.CreateFromBankExpenditureNoteAsync(data);

            var reportResponse = creditBalanceService.GenerateExcel(true, "", data.Date.Month, data.Date.Year, 7, false);
            Assert.NotNull(reportResponse);
        }
    }
}

using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Test.Controller.Utils;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.DailyBankTransaction;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.DailyBankTransaction
{
    public class DailyBankTransactionControllerTest : BaseControllerTest<DailyBankTransactionController, DailyBankTransactionModel, DailyBankTransactionViewModel, IDailyBankTransactionService>
    {
        private async Task<int> GetStatusCodeDeleteByReferenceNo((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IDailyBankTransactionService> Service, Mock<IMapper> Mapper) mocks)
        {
            DailyBankTransactionController controller = GetController(mocks);
            IActionResult response = await controller.DeleteByReferenceNo("1");
            return GetStatusCode(response);
        }

        [Fact]
        public async Task DeleteByReferenceNo_WithoutException_ReturnNoContent()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.DeleteByReferenceNoAsync(It.IsAny<string>())).ReturnsAsync(1);

            int statusCode = await GetStatusCodeDeleteByReferenceNo(mocks);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public async Task DeleteByReferenceNo_ThrowException_ReturnInternalStatusError()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.DeleteByReferenceNoAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

            int statusCode = await GetStatusCodeDeleteByReferenceNo(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void GetReport_Without_Exception()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new ReadResponse<DailyBankTransactionModel>(new List<DailyBankTransactionModel>(), 0, new Dictionary<string, string>(), new List<string>()));
            mocks.Mapper.Setup(f => f.Map<List<DailyBankTransactionViewModel>>(It.IsAny<List<DailyBankTransactionModel>>())).Returns(ViewModels);

            int statusCode = GetStatusCodeGetReport(mocks);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void GetReportDailyBankBalance_Without_Exception()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetDailyBalanceReport(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(
                 new List<DailyBalanceReportViewModel>() { new DailyBalanceReportViewModel() { AccountNumber = "", Balance = 0, BankName = "", Credit = 0, CurrencyCode = "", Debit = 0 }

                    }
            );
            //mocks.Mapper.Setup(f => f.Map<List<DailyBankTransactionViewModel>>(It.IsAny<List<DailyBankTransactionModel>>())).Returns(ViewModels);

            var controller = GetController(mocks);
            IActionResult response = controller.GetDailyBalanceAccountBankReport(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetReportDailyBankBalanceCurrency_Without_Exception()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetDailyBalanceCurrencyReport(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(
                 new List<DailyBalanceCurrencyReportViewModel>() { new DailyBalanceCurrencyReportViewModel() {  Balance = 0, Credit = 0, CurrencyCode = "", Debit = 0 }

                    }
            );
            //mocks.Mapper.Setup(f => f.Map<List<DailyBankTransactionViewModel>>(It.IsAny<List<DailyBankTransactionModel>>())).Returns(ViewModels);

            var controller = GetController(mocks);
            IActionResult response = controller.GetDailyBalanceCurrencyReport(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetReportDailyBankBalanceExcel_Without_Exception()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcelDailyBalance(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>())).Returns(new MemoryStream());
            //mocks.Mapper.Setup(f => f.Map<List<DailyBankTransactionViewModel>>(It.IsAny<List<DailyBankTransactionModel>>())).Returns(ViewModels);

            var controller = GetController(mocks);
            IActionResult response = controller.GetDailyBalanceReportXls(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.NotNull(response);
        }

        [Fact]
        public void GetReportDailyBankBalanceExcel_With_Exception()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcelDailyBalance(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>())).Throws(new Exception());
            //mocks.Mapper.Setup(f => f.Map<List<DailyBankTransactionViewModel>>(It.IsAny<List<DailyBankTransactionModel>>())).Returns(ViewModels);

            var controller = GetController(mocks);
            IActionResult response = controller.GetDailyBalanceReportXls(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetReportXls_Without_Exception()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcelDailyBalance(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>())).Returns(new MemoryStream());
            //mocks.Mapper.Setup(f => f.Map<List<DailyBankTransactionViewModel>>(It.IsAny<List<DailyBankTransactionModel>>())).Returns(ViewModels);

            var controller = GetController(mocks);
            IActionResult response = controller.GetReportXls(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.NotNull(response);
        }

        [Fact]
        public void GetReportXls_With_Exception()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcelDailyBalance(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>())).Throws(new Exception());
            //mocks.Mapper.Setup(f => f.Map<List<DailyBankTransactionViewModel>>(It.IsAny<List<DailyBankTransactionModel>>())).Returns(ViewModels);

            var controller = GetController(mocks);
            IActionResult response = controller.GetReportXls(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }


        public override async Task Post_WithoutException_ReturnCreated()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<DailyBankTransactionViewModel>())).Verifiable();
            mocks.Service.Setup(s => s.CreateAsync(It.IsAny<DailyBankTransactionModel>())).ReturnsAsync(1);
            mocks.Service.Setup(s => s.CreateInOutTransactionAsync(It.IsAny<DailyBankTransactionModel>())).ReturnsAsync(1);
            var vm = new DailyBankTransactionViewModel()
            {
                SourceType = "Pendanaan",
                Status = "IN"
            };
            DailyBankTransactionController controller = GetController(mocks);
            IActionResult response = await controller.Post(vm);

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);

            var vm2 = new DailyBankTransactionViewModel()
            {
                SourceType = "Pendanaan",
                Status = "OUT"
            };

            IActionResult response2 = await controller.Post(vm2);

            int statusCode2 = GetStatusCode(response2);
            Assert.Equal((int)HttpStatusCode.Created, statusCode2);
        }

        private int GetStatusCodeGetReport((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IDailyBankTransactionService> Service, Mock<IMapper> Mapper) mocks)
        {
            var controller = GetController(mocks);
            IActionResult response = controller.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            return GetStatusCode(response);
        }

        [Fact]
        public void GetReportExcel_ReturnFile()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcelDailyBalance(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>())).Returns(new MemoryStream());

            var response = GetController(mocks).GetReportXls(1, 1, 2018);
            Assert.NotNull(response);
        }

        [Fact]
        public void GetReportExcel_ThrowException()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcelDailyBalance(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>())).Returns(new MemoryStream());

            var response = GetController(mocks).GetReportXls(1, 1, 2018);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetReportExcelNull_ThrowException()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcelDailyBalance(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>())).Returns(new MemoryStream());

            var response = GetController(mocks).GetReportXls(1, 8, 2030);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}

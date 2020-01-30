using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.PaymentDispositionNotVerifiedReport;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PaymentDispositionNotVerifiedReportViewModel;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.PaymentDispositionNotVerifiedReport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.PaymentDispositionNotVerifiedReport
{
    public class PaymentDispositionNotVerifiedReportControllerTest
    {
        public (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPaymentDispositionNotVerifiedReport> Service, Mock<IMapper> Mapper) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IPaymentDispositionNotVerifiedReport>(), Mapper: new Mock<IMapper>());
        }

        protected PaymentDispositionNotVerifiedReportController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPaymentDispositionNotVerifiedReport> Service, Mock<IMapper> Mapper) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                    new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            PaymentDispositionNotVerifiedReportController controller = new PaymentDispositionNotVerifiedReportController(mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Mapper.Object, mocks.Service.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        private PaymentDispositionNotVerifiedReportViewModel ViewModel
        {
            get
            {
                return new PaymentDispositionNotVerifiedReportViewModel()
                {
                    DispositionNo = It.IsAny<string>(),
                    DispositionDate = It.IsAny<DateTimeOffset>(),
                    DivisionName=It.IsAny<string>(),
                    Currency= It.IsAny<string>(),
                    VerifyDate = DateTimeOffset.UtcNow,
                    SupplierName= It.IsAny<string>(),
                    PayToSupplier= It.IsAny<double>(),
                };
            }
        }

        [Fact]
        public void GetReport_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), "notHistory")).Returns(Tuple.Create(new List<PaymentDispositionNotVerifiedReportViewModel> { ViewModel }, 25));

            var response = GetController(mocks).Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetReport_ThrowException()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), "notHistory")).Throws(new Exception());

            var response = GetController(mocks).Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetReportExcel_ReturnFile()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), "notHistory")).Returns(new MemoryStream());

            var response = GetController(mocks).GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, null);
            Assert.NotNull(response);
        }

        [Fact]
        public void GetReportExcel_ThrowException()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), "notHistory")).Throws(new Exception());

            var response = GetController(mocks).GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetReport_History_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), "history")).Returns(Tuple.Create(new List<PaymentDispositionNotVerifiedReportViewModel> { ViewModel }, 25));

            var response = GetController(mocks).GetHistoryReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetReport_History_ThrowException()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), "history")).Throws(new Exception());

            var response = GetController(mocks).GetHistoryReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetReportExcel_History_ReturnFile()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), "history")).Returns(new MemoryStream());

            var response = GetController(mocks).GetXlsHistoryReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, null);
            Assert.NotNull(response);
        }

        [Fact]
        public void GetReportExcel_History_ThrowException()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), "history")).Throws(new Exception());

            var response = GetController(mocks).GetXlsHistoryReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}

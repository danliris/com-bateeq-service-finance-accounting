using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditBalance;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditBalance;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.CreditBalance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.CreditBalance
{
    public class CreditBalanceControllerTest
    {
        public (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<ICreditBalanceService> Service, Mock<IMapper> Mapper) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<ICreditBalanceService>(), Mapper: new Mock<IMapper>());
        }

        protected CreditBalanceReportController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<ICreditBalanceService> Service, Mock<IMapper> Mapper) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            CreditBalanceReportController controller = new CreditBalanceReportController(mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Mapper.Object, mocks.Service.Object);
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

        [Fact]
        public void GetReport_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<bool>(),It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new ReadResponse<CreditBalanceViewModel>(new List<CreditBalanceViewModel>(), 1, new Dictionary<string, string>(), new List<string>()));

            var response = GetController(mocks).GetReport(false, 1, 2018);
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetReport_ThrowException()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception());

            var response = GetController(mocks).GetReport(false, 1, 2018);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetReportExcelLokal_ReturnFile()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new MemoryStream());

            var response = GetController(mocks).GetXls(false, 1, 2018);
            Assert.NotNull(response);
        }

        [Fact]
        public void GetReportExcelImpor_ReturnFile()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new MemoryStream());

            var response = GetController(mocks).GetXls(true, 1, 2018);
            Assert.NotNull(response);
        }

        [Fact]
        public void GetReportExcel_ThrowException()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception());

            var response = GetController(mocks).GetXls(false, 1, 2018);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetReportExcelNull_ThrowException()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception());

            var response = GetController(mocks).GetXls(false, 8, 2030);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}

using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionVerification;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.PurchasingDispositionVerification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.PurchasingDIspositionVerification
{
    public class PurchasingDispositionVerificationControllerTest
    {
        public (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPurchasingDispositionExpeditionService> Service, Mock<IMapper> Mapper) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IPurchasingDispositionExpeditionService>(), Mapper: new Mock<IMapper>());
        }

        protected PurchasingDispositionVerificationController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPurchasingDispositionExpeditionService> Service, Mock<IMapper> Mapper) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                    new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            PurchasingDispositionVerificationController controller = new PurchasingDispositionVerificationController(mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Mapper.Object, mocks.Service.Object);
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

        private ServiceValidationException GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(this.ViewModel, serviceProvider.Object, null);
            return new ServiceValidationException(validationContext, validationResults);
        }

        private PurchasingDispositionVerificationViewModel ViewModel
        {
            get
            {
                return new PurchasingDispositionVerificationViewModel()
                {
                    DispositionNo = "DispositionNo",
                    Id = 1,
                    Reason = "Reason",
                    SubmitPosition = ExpeditionPosition.SEND_TO_CASHIER_DIVISION,
                    VerifyDate = DateTimeOffset.UtcNow
                };
            }
        }

        [Fact]
        public void Post_Success()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.PurchasingDispositionVerification(It.IsAny<PurchasingDispositionVerificationViewModel>())).ReturnsAsync(1);

            var response = GetController(mocks).Post(ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public void Post_Throws_Validation_Exception()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PurchasingDispositionVerificationViewModel>())).Throws(GetServiceValidationExeption());
            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IPurchasingDispositionExpeditionService>();
            var mockIdentity = new Mock<IIdentityService>();
            var response = GetController((mockIdentity, validateMock, mockFacade, mockMapper)).Post(ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public void Post_Throws_Internal_Error()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.PurchasingDispositionVerification(It.IsAny<PurchasingDispositionVerificationViewModel>())).Throws(new Exception());

            var response = GetController(mocks).Post(ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Post_Throws_Internal_Error_Aggregate()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.PurchasingDispositionVerification(It.IsAny<PurchasingDispositionVerificationViewModel>())).Throws(new AggregateException());

            var response = GetController(mocks).Post(ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}

using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.LockTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.LockTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.LockTransaction;
using Com.Danliris.Service.Finance.Accounting.Test.Controller.Utils;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.LockTransaction;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.LockTransaction
{
    public class LockTransactionControllerTest : BaseControllerTest<LockTransactionController, LockTransactionModel, LockTransactionViewModel, ILockTransactionService>
    {
        [Fact]
        public async Task GetByLastActive_NotNullModel_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Mapper.Setup(f => f.Map<LockTransactionViewModel>(It.IsAny<LockTransactionModel>())).Returns(ViewModel);
            mocks.Service.Setup(f => f.GetLastActiveLockTransaction(It.IsAny<string>())).ReturnsAsync(Model);

            var controller = GetController(mocks);
            var response = await controller.GetLatestActiveLockTransactionByType(It.IsAny<string>());

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async Task GetByLastActive_NullModel_ReturnNotFound()
        {
            var mocks = GetMocks();
            mocks.Mapper.Setup(f => f.Map<LockTransactionViewModel>(It.IsAny<LockTransactionModel>())).Returns(ViewModel);
            mocks.Service.Setup(f => f.GetLastActiveLockTransaction(It.IsAny<string>())).ReturnsAsync((LockTransactionModel)null);

            var controller = GetController(mocks);
            var response = await controller.GetLatestActiveLockTransactionByType(It.IsAny<string>());

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public async Task GetByLastActive_ThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetLastActiveLockTransaction(It.IsAny<string>())).ThrowsAsync(new Exception());

            var controller = GetController(mocks);
            var response = await controller.GetLatestActiveLockTransactionByType(It.IsAny<string>());

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void GetTypeOptions_ReturnSuccess()
        {
            var mocks = GetMocks();

            var controller = GetController(mocks);
            var response = controller.GetLockTypeOptions();

            int statusCode = GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }
    }
}

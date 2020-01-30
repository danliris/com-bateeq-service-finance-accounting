using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.Masters.COADataUtils;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.Masters.COATest
{
    public class COATest
    {
        private const string ENTITY = "MasterCOA";
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

        private COADataUtil _dataUtil(COAService service)
        {

            GetServiceProvider();
            return new COADataUtil(service);
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
        public async Task Should_Success_Get_Data()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            await _dataUtil(service).GetTestData();
            var Response = service.Read(1, 25, "{}", null, null, "{}");
            Assert.NotEmpty(Response.Data);
        }

        [Fact]
        public async Task Should_Success_Get_Data_All()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            await _dataUtil(service).GetTestData();
            var Response = service.GetAll();
            Assert.NotEmpty(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAModel model = await _dataUtil(service).GetTestData();
            var Response = await service.ReadByIdAsync(model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAModel model = _dataUtil(service).GetNewData();
            var Response = await service.CreateAsync(model);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public void Should_Success_Validate_All_Null_Data()
        {
            COAViewModel vm = new COAViewModel();

            Assert.True(vm.Validate(null).Count() > 0);
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAModel model = await _dataUtil(service).GetTestData();
            var newModel = await service.ReadByIdAsync(model.Id);
            newModel.Name += "[Updated]";
            var Response = await service.UpdateAsync(newModel.Id, newModel);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAModel model = await _dataUtil(service).GetTestData();
            var newModel = await service.ReadByIdAsync(model.Id);

            var Response = await service.DeleteAsync(newModel.Id);
            var deletedModel = await service.ReadByIdAsync(newModel.Id);
            Assert.Null(deletedModel);
        }

        [Fact]
        public async Task Should_Success_Upload_Data()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAModel model = _dataUtil(service).GetNewData();

            List<COAModel> coa = new List<COAModel>() { model };

            await service.UploadData(coa);
            Assert.True(true);
        }

        [Fact]
        public void Should_Success_Upload_Validate_Data()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAViewModel viewModel = _dataUtil(service).GetNewViewModel();

            List<COAViewModel> coa = new List<COAViewModel>() { viewModel };
            var Response = service.UploadValidate(ref coa, null);
            Assert.True(Response.Item1);
        }

        [Fact]
        public void Should_Fail_Upload_Validate_Count_Code_Data()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAViewModel viewModel = _dataUtil(service).GetNewViewModel();
            viewModel.Code = "11.1.1";
            List<COAViewModel> coa = new List<COAViewModel>() { viewModel };
            var Response = service.UploadValidate(ref coa, null);
            Assert.False(Response.Item1);
        }

        [Fact]
        public void Should_Fail_Upload_Validate_Not_Digit_Data()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAViewModel viewModel = _dataUtil(service).GetNewViewModel();
            viewModel.Code = "1a.1.1.1";
            List<COAViewModel> coa = new List<COAViewModel>() { viewModel };
            var Response = service.UploadValidate(ref coa, null);
            Assert.False(Response.Item1);
        }

        [Fact]
        public void Should_Fail_Upload_Validate_Too_long_Data()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAViewModel viewModel = _dataUtil(service).GetNewViewModel();
            viewModel.Code = "11111.1111.1111.1111";
            List<COAViewModel> coa = new List<COAViewModel>() { viewModel };
            var Response = service.UploadValidate(ref coa, null);
            Assert.False(Response.Item1);
        }
        [Fact]
        public void Should_Success_Upload_Short_Code_Data()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAViewModel viewModel = _dataUtil(service).GetNewViewModel();
            viewModel.Code = "1.1.1.1";
            List<COAViewModel> coa = new List<COAViewModel>() { viewModel };
            var Response = service.UploadValidate(ref coa, null);
            Assert.True(Response.Item1);
        }

        [Fact]
        public async Task Should_Fail_Upload_Existed_Data()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAViewModel viewModel = _dataUtil(service).GetNewViewModel();

            List<COAViewModel> coa = new List<COAViewModel>() { viewModel };
            COAModel model = _dataUtil(service).GetNewData();

            List<COAModel> coaModel = new List<COAModel>() { model };
            await service.UploadData(coaModel);
            var Response2 = service.UploadValidate(ref coa, null);
            Assert.False(Response2.Item1);
        }

        [Fact]
        public void Should_Fail_Double_Upload_Validate_Data()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            
            COAViewModel viewModel = _dataUtil(service).GetNewViewModel();
            COAViewModel viewModel2 = _dataUtil(service).GetNewViewModel();
            List<COAViewModel> coa = new List<COAViewModel>() { viewModel, viewModel2 };
            var Response = service.UploadValidate(ref coa, null);
            Assert.False(Response.Item1);
        }

        [Fact]
        public void Should_Fail_Empty_Code_Upload_Validate_Data()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            COAViewModel viewModel = _dataUtil(service).GetNewViewModel();
            viewModel.Name = null;
            viewModel.Code = null;
            List<COAViewModel> coa = new List<COAViewModel>() { viewModel };
            var Response = service.UploadValidate(ref coa, null);
            Assert.False(Response.Item1);
        }

        [Fact]
        public void Should_Success_Get_CSV()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            
            var reportResponse = service.DownloadTemplate();
            Assert.NotNull(reportResponse);
        }

        [Fact]
        public async Task Should_Success_Get_Empty_Name_Coa()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAModel model = _dataUtil(service).GetNewData();
            model.Name = null;
            await service.CreateAsync(model);
            var reportResponse = await service.GetEmptyNames();
            Assert.NotEmpty(reportResponse);
        }

        [Fact]
        public async Task Should_Success_Revise_Empty_Name_Coa()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAModel model = _dataUtil(service).GetNewData();
            model.Name = null;
            await service.CreateAsync(model);
            var createdModel = await service.ReadByIdAsync(model.Id);
            createdModel.Name = "a";
            List<COAModel> models = new List<COAModel>()
            {
                createdModel
            };
            
            var reportResponse = await service.ReviseEmptyNamesCoa(models);
            Assert.NotEqual(0, reportResponse);
        }

        [Fact]
        public async Task Should_Success_Revise_Empty_Name_Coa_But_Empty_List()
        {
            COAService service = new COAService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            COAModel model = _dataUtil(service).GetNewData();
            await service.CreateAsync(model);
            var createdModel = await service.ReadByIdAsync(model.Id);
            createdModel.Name = null;
            List<COAModel> models = new List<COAModel>()
            {
                createdModel
            };

            var reportResponse = await service.ReviseEmptyNamesCoa(models);
            Assert.NotEqual(0, reportResponse);
        }
    }
}

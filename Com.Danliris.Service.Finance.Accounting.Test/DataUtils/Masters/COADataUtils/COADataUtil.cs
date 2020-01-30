using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MasterCOA;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.Masters.COADataUtils
{
    public class COADataUtil
    {
        private readonly COAService Service;

        public COADataUtil(COAService service)
        {
            Service = service;
        }

        public COAModel GetNewData()
        {
            return new COAModel()
            {
                CashAccount = "CashAccount",
                Name = "Name",
                Code = "1111.11.1.11",
                Code1 = "1111",
                Code2 = "11",
                Code3 = "1",
                Code4 = "11",
                Nature = "Nature",
                Path = "/",
                ReportType = "ReportType"
                
            };
        }

        public COAViewModel GetNewViewModel()
        {
            return new COAViewModel()
            {
                CashAccount = "CashAccount",
                Name = "Name",
                Code = "1111.11.1.11",
                Nature = "Nature",
                Path = "/",
                ReportType = "ReportType"

            };
        }

        public async Task<COAModel> GetTestData()
        {
            COAModel model = GetNewData();
            await Service.CreateAsync(model);
            return await Service.ReadByIdAsync(model.Id);
        }

        public async Task<COAModel> GetTestData2()
        {
            COAModel model = GetNewData();
            model.Code = "1111.11.1.00";
            model.Code4 = "00";
            await Service.CreateAsync(model);
            return await Service.ReadByIdAsync(model.Id);
        }

        public async Task<COAModel> GetTestData3()
        {
            COAModel model = GetNewData();
            model.Code = "1111.11.0.00";
            model.Code4 = "00";
            model.Code3 = "0";
            await Service.CreateAsync(model);
            return await Service.ReadByIdAsync(model.Id);
        }

        public async Task<COAModel> GetTestData4()
        {
            COAModel model = GetNewData();
            model.Code = "1111.00.0.00";
            model.Code4 = "00";
            model.Code3 = "0";
            model.Code2 = "00";
            await Service.CreateAsync(model);
            return await Service.ReadByIdAsync(model.Id);
        }
    }
}

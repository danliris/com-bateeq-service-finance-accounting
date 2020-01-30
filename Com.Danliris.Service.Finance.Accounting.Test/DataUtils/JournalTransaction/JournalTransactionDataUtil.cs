using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Enums.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.Masters.COADataUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.JournalTransaction
{
    public class JournalTransactionDataUtil
    {
        private readonly JournalTransactionService _Service;
        private readonly COADataUtil _COADataUtil;

        public JournalTransactionDataUtil(JournalTransactionService service, COADataUtil coaDataUtil)
        {
            _Service = service;
            _COADataUtil = coaDataUtil;
        }

        public JournalTransactionModel GetNewData()
        {
            var COA1 = Task.Run(() => _COADataUtil.GetTestData()).Result;
            var COA2 = Task.Run(() => _COADataUtil.GetTestData2()).Result;
            var COA3 = Task.Run(() => _COADataUtil.GetTestData3()).Result;
            var COA4 = Task.Run(() => _COADataUtil.GetTestData4()).Result;
            JournalTransactionModel TestData = new JournalTransactionModel()
            {
                DocumentNo = Guid.NewGuid().ToString(),
                Description = "Description",
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = Guid.NewGuid().ToString(),
                Items = new List<JournalTransactionItemModel>()
                {
                    new JournalTransactionItemModel()
                    {
                        COAId = COA1.Id,
                        COA = COA1,
                        Remark = "Remark",
                        Debit = 10000.00m,
                        Credit = 10000.00m
                    }
                }
            };

            return TestData;
        }

        public JournalTransactionModel GetNewPostedData()
        {
            var COA1 = Task.Run(() => _COADataUtil.GetTestData()).Result;
            JournalTransactionModel TestData = new JournalTransactionModel()
            {
                DocumentNo = Guid.NewGuid().ToString(),
                Description = "Description",
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = "aa-aa-aaI-aa",
                Status = JournalTransactionStatus.Posted,
                Items = new List<JournalTransactionItemModel>()
                {
                    new JournalTransactionItemModel()
                    {
                        COAId = COA1.Id,
                        COA = COA1,
                        Remark = "Remark",
                        Debit = 10000.00m,
                        Credit = 10000.00m
                    }
                }
            };

            return TestData;
        }

        public JournalTransactionModel GetNewPostedManualData()
        {
            var COA1 = Task.Run(() => _COADataUtil.GetTestData()).Result;
            JournalTransactionModel TestData = new JournalTransactionModel()
            {
                DocumentNo = Guid.NewGuid().ToString(),
                Description = "Description",
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = "aa-aa-aaL-aa",
                Status = JournalTransactionStatus.Posted,
                Items = new List<JournalTransactionItemModel>()
                {
                    new JournalTransactionItemModel()
                    {
                        COAId = COA1.Id,
                        COA = COA1,
                        Remark = "Remark",
                        Debit = 10000.00m,
                        Credit = 10000.00m
                    }
                }
            };

            return TestData;
        }

        public JournalTransactionModel GetPostedGarmenLtData()
        {
            var COA1 = Task.Run(() => _COADataUtil.GetTestData()).Result;
            COA1.Code = "1111.11.4.11";
            JournalTransactionModel TestData = new JournalTransactionModel()
            {
                DocumentNo = Guid.NewGuid().ToString(),
                Description = "Description",
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = "aa-aa-aaL-aa",
                Status = JournalTransactionStatus.Posted,
                Items = new List<JournalTransactionItemModel>()
                {
                    new JournalTransactionItemModel()
                    {
                        COAId = COA1.Id,
                        COA = COA1,
                        Remark = "Remark",
                        Debit = 10000.00m,
                        Credit = 10000.00m
                    }
                }
            };

            return TestData;
        }

        public JournalTransactionModel GetPostedGarmenItData()
        {
            var COA1 = Task.Run(() => _COADataUtil.GetTestData()).Result;
            COA1.Code = "1111.11.4.11";
            JournalTransactionModel TestData = new JournalTransactionModel()
            {
                DocumentNo = Guid.NewGuid().ToString(),
                Description = "Description",
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = "aa-aa-aaL-aa",
                Status = JournalTransactionStatus.Posted,
                Items = new List<JournalTransactionItemModel>()
                {
                    new JournalTransactionItemModel()
                    {
                        COAId = COA1.Id,
                        COA = COA1,
                        Remark = "Remark",
                        Debit = 10000.00m,
                        Credit = 10000.00m
                    }
                }
            };

            return TestData;
        }

        public JournalTransactionViewModel GetDataToValidate()
        {
            var COA1 = Task.Run(() => _COADataUtil.GetTestData()).Result;
            JournalTransactionViewModel TestData = new JournalTransactionViewModel()
            {
                DocumentNo = Guid.NewGuid().ToString(),
                Description = "Description",
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = Guid.NewGuid().ToString(),
                Items = new List<JournalTransactionItemViewModel>()
                {
                    new JournalTransactionItemViewModel()
                    {
                        COA = new COAViewModel()
                        {
                            Id = COA1.Id,
                            Code = COA1.Code,
                            CashAccount = COA1.CashAccount
                        },
                        Remark = "Remark",
                        Debit = 10000.00m,
                        Credit = 0
                    },
                    new JournalTransactionItemViewModel()
                    {
                        COA = new COAViewModel()
                        {
                            Id = COA1.Id,
                            Code = COA1.Code,
                            CashAccount = COA1.CashAccount
                        },
                        Remark = "Remark",
                        Debit = 0,
                        Credit = 10000.00m
                    },
                }
            };

            return TestData;
        }

        public async Task<JournalTransactionModel> GetTestData()
        {
            JournalTransactionModel model = GetNewData();
            await _Service.CreateAsync(model);
            return await _Service.ReadByIdAsync(model.Id);
        }

        public async Task<JournalTransactionModel> GetTestPostedData()
        {
            JournalTransactionModel model = GetNewPostedData();
            await _Service.CreateAsync(model);
            return await _Service.ReadByIdAsync(model.Id);
        }

        public async Task<JournalTransactionModel> GetTestPostedManualData()
        {
            JournalTransactionModel model = GetNewPostedManualData();
            await _Service.CreateAsync(model);
            return await _Service.ReadByIdAsync(model.Id);
        }

        public async Task<JournalTransactionModel> GetTestPostedGarmentLData()
        {
            JournalTransactionModel model = GetPostedGarmenLtData();
            await _Service.CreateAsync(model);
            return await _Service.ReadByIdAsync(model.Id);
        }

        public async Task<JournalTransactionModel> GetTestPostedGarmentIData()
        {
            JournalTransactionModel model = GetPostedGarmenItData();
            await _Service.CreateAsync(model);
            return await _Service.ReadByIdAsync(model.Id);
        }
    }
}

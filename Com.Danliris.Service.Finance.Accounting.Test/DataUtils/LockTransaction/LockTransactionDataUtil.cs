using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.LockTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.LockTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.LockTransaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.LockTransaction
{
    public class LockTransactionDataUtil
    {
        private readonly LockTransactionService _service;

        public LockTransactionDataUtil(LockTransactionService service)
        {
            _service = service;
        }

        public LockTransactionModel GetNewData()
        {
            return new LockTransactionModel()
            {
                Description = "Description",
                IsActiveStatus = false,
                //BeginLockDate = DateTimeOffset.Now.AddDays(-2),
                LockDate = DateTimeOffset.Now,
                Type = "Type"
            };
        }

        public LockTransactionViewModel GetNewViewModel()
        {
            return new LockTransactionViewModel()
            {
                Description = "Description",
                IsActiveStatus = false,
                //BeginLockDate = DateTimeOffset.Now.AddDays(-2),
                LockDate = DateTimeOffset.Now,
                Type = "Type"

            };
        }

        public async Task<LockTransactionModel> GetTestData()
        {
            var model = GetNewData();
            await _service.CreateAsync(model);
            return await _service.ReadByIdAsync(model.Id);
        }
    }
}

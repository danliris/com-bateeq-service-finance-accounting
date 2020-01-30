using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction
{
    public interface IDailyBankTransactionService : IBaseService<DailyBankTransactionModel>
    {
        ReadResponse<DailyBankTransactionModel> GetReport(int bankId, int month, int year, int clientTimeZoneOffset);
        List<DailyBalanceReportViewModel> GetDailyBalanceReport(int bankId, DateTime startDate, DateTime endDate);
        List<DailyBalanceCurrencyReportViewModel> GetDailyBalanceCurrencyReport(int bankId, DateTime startDate, DateTime endDate);
        MemoryStream GenerateExcel(int bankId, int month, int year, int clientTimeZoneOffset);
        Task<int> DeleteByReferenceNoAsync(string referenceNo);
        Task<int> CreateInOutTransactionAsync(DailyBankTransactionModel model);
        MemoryStream GenerateExcelDailyBalance(int bankId, DateTime startDate, DateTime endDate, int clientTimeZoneOffset);
    }
}

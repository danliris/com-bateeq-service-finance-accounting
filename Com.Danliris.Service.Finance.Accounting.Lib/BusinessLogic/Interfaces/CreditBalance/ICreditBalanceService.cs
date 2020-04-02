using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditBalance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditBalance
{
    public interface ICreditBalanceService
    {
        ReadResponse<CreditBalanceViewModel> GetReport(bool isImport, int page, int size, string suplierName, int month, int year, int offSet, bool isForeignCurrency);
        MemoryStream GenerateExcel(bool isImport, string suplierName, int month, int year, int offSet, bool isForeignCurrency);
    }
}

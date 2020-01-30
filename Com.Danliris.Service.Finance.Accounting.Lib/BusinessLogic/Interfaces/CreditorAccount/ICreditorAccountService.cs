
using Com.Danliris.Service.Finance.Accounting.Lib.Enums;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount;
using System.IO;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditorAccount
{
    public interface ICreditorAccountService
    {
        (ReadResponse<CreditorAccountViewModel>, decimal) GetReport(int page, int size, string suplierName, int month, int year, int offSet);
        MemoryStream GenerateExcel(string suplierName, int month, int year, int offSet);
        Task<int> UpdateFromUnitPaymentOrderAsync(CreditorAccountUnitPaymentOrderPostedViewModel viewModel);
        Task<int> CreateFromUnitReceiptNoteAsync(CreditorAccountUnitReceiptNotePostedViewModel viewModel);
        Task<int> UpdateFromUnitReceiptNoteAsync(CreditorAccountUnitReceiptNotePostedViewModel viewModel);
        Task<int> DeleteFromUnitReceiptNoteAsync(CreditorAccountUnitReceiptNotePostedViewModel viewModel);
        Task<int> DeleteFromUnitReceiptNoteAsync(int id);
        Task<int> CreateFromBankExpenditureNoteAsync(CreditorAccountBankExpenditureNotePostedViewModel viewModel);
        Task<int> UpdateFromBankExpenditureNoteAsync(CreditorAccountBankExpenditureNotePostedViewModel viewModel);
        Task<int> DeleteFromBankExpenditureNoteAsync(int id);
        Task<int> DeleteFromBankExpenditureNoteListAsync(string code);
        Task<CreditorAccountUnitReceiptNotePostedViewModel> GetByUnitReceiptNote(string supplierCode, string unitReceiptNote, string invoiceNo);
        Task<CreditorAccountBankExpenditureNotePostedViewModel> GetByBankExpenditureNote(string supplierCode, string bankExpenditureNote, string invoiceNo);
        //Task<CreditorAccountMemoPostedViewModel> GetByMemo(string supplierCode, string memoNo, string invoiceNo);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Services.OthersExpenditureProofDocument
{
    public interface IOthersExpenditureProofDocumentService
    {
        Task<int> CreateAsync(OthersExpenditureProofDocumentCreateUpdateViewModel viewModel);
        Task<int> UpdateAsync(int id, OthersExpenditureProofDocumentCreateUpdateViewModel viewModel);
        Task<int> DeleteAsync(int id);
        Task<OthersExpenditureProofPagedListViewModel> GetPagedListAsync(int page, int size, string order, string keyword, string filter);
        Task<OthersExpenditureProofDocumentViewModel> GetSingleByIdAsync(int id);
    }
}
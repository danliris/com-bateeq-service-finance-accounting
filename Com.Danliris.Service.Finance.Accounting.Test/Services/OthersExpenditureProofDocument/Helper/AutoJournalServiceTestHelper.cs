using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.OthersExpenditureProofDocument.Helper
{
    public class AutoJournalServiceTestHelper : IAutoJournalService
    {
        public async Task<int> AutoJournalFromOthersExpenditureProof(OthersExpenditureProofDocumentCreateUpdateViewModel viewModel, string documentNo)
        {
            await Task.Delay(1000);
            return await Task.FromResult(1);
        }

        public async Task<int> AutoJournalReverseFromOthersExpenditureProof(string documentNo)
        {
            await Task.Delay(1000);
            return await Task.FromResult(1);
        }
    }
}

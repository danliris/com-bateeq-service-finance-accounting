using Com.Danliris.Service.Finance.Accounting.Lib.Models.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.PaymentDispositionNote
{
    public interface IPaymentDispositionNoteService : IBaseService<PaymentDispositionNoteModel>
    {
        ReadResponse<PaymentDispositionNoteDetailModel> ReadDetailsByEPOId(string epoId);
    }
}

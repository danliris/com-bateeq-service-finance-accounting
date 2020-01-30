using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount
{
    public class CreditorAccountUnitReceiptNotePostedViewModel : CreditorAccountPostedViewModel
    {
        public decimal DPP { get; set; }

        public decimal PPN { get; set; }

        public string Currency { get; set; }

        public string Products { get; set; }

        public bool UseIncomeTax { get; set; }

    }
}

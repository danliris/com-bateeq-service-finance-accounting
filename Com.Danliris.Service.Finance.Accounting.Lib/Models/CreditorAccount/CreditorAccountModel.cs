using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount
{
    public class CreditorAccountModel : StandardEntity, IValidatableObject
    {
        #region Supplier
        public string SupplierName { get; set; }

        public string SupplierCode { get; set; }

        public bool SupplierIsImport { get; set; }
        #endregion


        #region UnitReceiptNote
        public string UnitReceiptNoteNo { get; set; }

        public string Products { get; set; }

        public DateTimeOffset? UnitReceiptNoteDate { get; set; }

        public decimal UnitReceiptNoteDPP { get; set; }

        public decimal UnitReceiptNotePPN { get; set; }

        public decimal UnitReceiptMutation { get; set; }
        #endregion

        #region BankExpenditureNote
        public int BankExpenditureNoteId { get; set; }

        public string BankExpenditureNoteNo { get; set; }

        public DateTimeOffset? BankExpenditureNoteDate { get; set; }

        public decimal BankExpenditureNoteDPP { get; set; }

        public decimal BankExpenditureNotePPN { get; set; }

        public decimal BankExpenditureNoteMutation { get; set; }
        #endregion

        #region Memo has been changed to SPBNo or NI No
        public string MemoNo { get; set; }

        public DateTimeOffset? MemoDate { get; set; }

        public decimal MemoDPP { get; set; }

        public decimal MemoPPN { get; set; }

        public decimal MemoMutation { get; set; }
        public string PaymentDuration { get; set; }
        #endregion

        public string InvoiceNo { get; set; }

        public decimal FinalBalance { get; set; }

        public string CurrencyCode { get; set; }

        public decimal DPPCurrency { get; set; }
        public decimal CurrencyRate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}

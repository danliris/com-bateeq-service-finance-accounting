using Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.LockTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.OthersExpenditureProofDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PurchasingDispositionExpedition;
using Com.Moonlay.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib
{
    public class FinanceDbContext : StandardDbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
        {
        }

        public DbSet<COAModel> ChartsOfAccounts { get; set; }

        public DbSet<CreditorAccountModel> CreditorAccounts { get; set; }

        public DbSet<DailyBankTransactionModel> DailyBankTransactions { get; set; }
        public DbSet<BankTransactionMonthlyBalanceModel> BankTransactionMonthlyBalances { get; set; }

        public DbSet<JournalTransactionModel> JournalTransactions { get; set; }
        public DbSet<JournalTransactionItemModel> JournalTransactionItems { get; set; }
        public DbSet<JournalTransactionNumber> JournalTransactionNumbers { get; set; }

        public DbSet<LockTransactionModel> LockTransactions { get; set; }

        public DbSet<PurchasingDispositionExpeditionModel> PurchasingDispositionExpeditions { get; set; }
        public DbSet<PurchasingDispositionExpeditionItemModel> PurchasingDispositionExpeditionItems { get; set; }

        public DbSet<PaymentDispositionNoteModel> PaymentDispositionNotes { get; set; }
        public DbSet<PaymentDispositionNoteItemModel> PaymentDispositionNoteItems { get; set; }
        public DbSet<PaymentDispositionNoteDetailModel> PaymentDispositionNoteDetails { get; set; }

        public DbSet<OthersExpenditureProofDocumentModel> OthersExpenditureProofDocuments { get; set; }
        public DbSet<OthersExpenditureProofDocumentItemModel> OthersExpenditureProofDocumentItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JournalTransactionItemModel>().Property(x => x.Debit).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<JournalTransactionItemModel>().Property(x => x.Credit).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<COAModel>().Property(x => x.Balance).HasColumnType("decimal(18,2)");
            base.OnModelCreating(modelBuilder);
        }
    }
}

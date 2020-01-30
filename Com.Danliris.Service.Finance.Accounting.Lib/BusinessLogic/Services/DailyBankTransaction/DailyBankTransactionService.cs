using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction
{
    public class DailyBankTransactionService : IDailyBankTransactionService
    {
        private const string _UserAgent = "finance-service";
        protected DbSet<DailyBankTransactionModel> _DbSet;
        protected DbSet<BankTransactionMonthlyBalanceModel> _DbMonthlyBalanceSet;
        protected IIdentityService _IdentityService;
        public FinanceDbContext _DbContext;

        public DailyBankTransactionService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            _DbContext = dbContext;
            _DbSet = dbContext.Set<DailyBankTransactionModel>();
            _DbMonthlyBalanceSet = dbContext.Set<BankTransactionMonthlyBalanceModel>();
            _IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> CreateAsync(DailyBankTransactionModel model)
        {
            do
            {
                model.Code = CodeGenerator.Generate();
            }
            while (_DbSet.Any(d => d.Code.Equals(model.Code)));

            model.Date = model.Date.AddHours(_IdentityService.TimezoneOffset);
            UpdateRemainingBalance(model);

            EntityExtension.FlagForCreate(model, _IdentityService.Username, _UserAgent);

            _DbSet.Add(model);
            return await _DbContext.SaveChangesAsync();
        }

        private void UpdateRemainingBalance(DailyBankTransactionModel model)
        {
            var Month = model.Date.Month;
            var Year = model.Date.Year;
            var BankId = model.AccountBankId;
            var ActualBalanceByMonth = _DbMonthlyBalanceSet.Where(w => w.Month.Equals(Month) && w.Year.Equals(Year) && w.AccountBankId.Equals(BankId)).FirstOrDefault();
            var Nominal = model.Status.Equals("IN") ? model.Nominal : model.Nominal * -1;

            if (ActualBalanceByMonth == null)
            {
                SetNewActualBalanceByMonth(Month, Year, model, Nominal);
            }
            else
            {
                var NextMonthBalance = GetNextMonthBalance(Month, Year);
                var SumInByMonth = GetSumInByMonth(Month, Year, BankId);
                var SumOutByMonth = GetSumOutByMonth(Month, Year, BankId);

                ActualBalanceByMonth.RemainingBalance = ActualBalanceByMonth.InitialBalance + ((double)SumInByMonth + (double)Nominal - (double)SumOutByMonth);

                if (NextMonthBalance != null)
                {
                    NextMonthBalance.InitialBalance = ActualBalanceByMonth.RemainingBalance;
                    EntityExtension.FlagForUpdate(NextMonthBalance, _IdentityService.Username, _UserAgent);
                    _DbMonthlyBalanceSet.Update(NextMonthBalance);
                }

                EntityExtension.FlagForUpdate(ActualBalanceByMonth, _IdentityService.Username, _UserAgent);
                _DbMonthlyBalanceSet.Update(ActualBalanceByMonth);
            }
        }

        private void SetNewActualBalanceByMonth(int month, int year, DailyBankTransactionModel model, decimal nominal)
        {
            var PreviousMonthBalance = GetPreviousMonthBalance(month, year);
            var NextMonthBalance = GetNextMonthBalance(month, year);
            var NewMonthBalance = new BankTransactionMonthlyBalanceModel
            {
                Month = month,
                Year = year,
                InitialBalance = PreviousMonthBalance != null ? PreviousMonthBalance.RemainingBalance : 0,
                RemainingBalance = PreviousMonthBalance != null ? PreviousMonthBalance.RemainingBalance + (double)nominal : (double)nominal,
                AccountBankId = model.AccountBankId
            };

            EntityExtension.FlagForCreate(NewMonthBalance, _IdentityService.Username, _UserAgent);
            _DbMonthlyBalanceSet.Add(NewMonthBalance);

            if (NextMonthBalance != null)
            {
                NextMonthBalance.InitialBalance = NewMonthBalance.RemainingBalance;
                NextMonthBalance.RemainingBalance += (double)nominal;
                EntityExtension.FlagForUpdate(NextMonthBalance, _IdentityService.Username, _UserAgent);
                _DbMonthlyBalanceSet.Update(NextMonthBalance);
            }
        }

        private decimal GetSumOutByMonth(int month, int year, int bankId)
        {
            return _DbSet.Where(w => w.Date.Month.Equals(month) && w.Date.Year.Equals(year) && w.AccountBankId.Equals(bankId) && w.Status.Equals("OUT")).Sum(s => s.Nominal);
        }

        private decimal GetSumInByMonth(int month, int year, int bankId)
        {
            return _DbSet.Where(w => w.Date.Month.Equals(month) && w.Date.Year.Equals(year) && w.AccountBankId.Equals(bankId) && w.Status.Equals("IN")).Sum(s => s.Nominal);
        }

        private BankTransactionMonthlyBalanceModel GetNextMonthBalance(int month, int year)
        {
            if (month == 12)
            {
                return _DbMonthlyBalanceSet.Where(w => w.Month.Equals(1) && w.Year.Equals(year + 1)).FirstOrDefault();
            }
            else
            {
                return _DbMonthlyBalanceSet.Where(w => w.Month.Equals(month + 1) && w.Year.Equals(year)).FirstOrDefault();
            }
        }

        private BankTransactionMonthlyBalanceModel GetPreviousMonthBalance(int month, int year)
        {
            if (month == 1)
            {
                return _DbMonthlyBalanceSet.Where(w => w.Month.Equals(12) && w.Year.Equals(year - 1)).FirstOrDefault();
            }
            else
            {
                return _DbMonthlyBalanceSet.Where(w => w.Month.Equals(month - 1) && w.Year.Equals(year)).FirstOrDefault();
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            //not implemented
            var result = await _DbSet.Where(w => w.Id.Equals(id)).FirstOrDefaultAsync();
            return result.Id;
        }

        public MemoryStream GenerateExcel(int bankId, int month, int year, int clientTimeZoneOffset)
        {
            var Query = GetQuery(bankId, month, year, clientTimeZoneOffset);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Referensi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Referensi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Currency", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Before", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Debit", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kredit", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "After", DataType = typeof(double) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", 0, 0, 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                var BalanceByMonthAndYear = GetBalanceMonthAndYear(bankId, month, year, clientTimeZoneOffset);
                var beforeBalance = BalanceByMonthAndYear.InitialBalance;
                //var previous = new DailyBankTransactionModel();
                foreach (var item in Query)
                {
                    var afterBalance = beforeBalance + (item.Status.Equals("IN") ? (double)item.Nominal : (double)item.Nominal * -1);
                    result.Rows.Add(item.Date.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID")), item.Remark, item.ReferenceNo, item.ReferenceType, item.AccountBankCurrencyCode, beforeBalance, item.Status.ToUpper().Equals("IN") ? item.Nominal : 0, item.Status.ToUpper().Equals("OUT") ? item.Nominal : 0, afterBalance);
                    beforeBalance = afterBalance;
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Mutasi") }, true);
        }

        private BankTransactionMonthlyBalanceModel GetBalanceMonthAndYear(int bankId, int month, int year, int clientTimeZoneOffset)
        {
            return _DbMonthlyBalanceSet.Where(w => w.AccountBankId.Equals(bankId) && w.Month.Equals(month) && w.Year.Equals(year)).FirstOrDefault();
        }

        private IQueryable<DailyBankTransactionModel> GetQuery(int bankId, int month, int year, int clientTimeZoneOffset)
        {
            //DateTimeOffset DateFrom = dateFrom == null ? dateTo == null ? DateTimeOffset.Now.AddDays(-30) : dateTo.Value.AddHours(clientTimeZoneOffset * -1).AddDays(-30) : dateFrom.Value.AddHours(clientTimeZoneOffset * -1);
            //DateTimeOffset DateTo = dateTo == null ? dateFrom == null ? DateTimeOffset.Now : dateFrom.Value.AddHours(clientTimeZoneOffset * -1).AddDays(DateTimeOffset.Now.Subtract(dateFrom.Value.AddHours(clientTimeZoneOffset * -1)).TotalDays) : dateTo.Value.AddHours(clientTimeZoneOffset * -1);

            var Query = (from transaction in _DbContext.DailyBankTransactions
                         where
                         transaction.IsDeleted == false
                         && transaction.AccountBankId == bankId
                         && transaction.Date.Month == month
                         && transaction.Date.Year == year
                         orderby transaction.Date, transaction.CreatedUtc
                         select new DailyBankTransactionModel
                         {
                             Id = transaction.Id,
                             Date = transaction.Date,
                             Remark = $"{transaction.SupplierName ?? transaction.BuyerName}\n{transaction.Remark}",
                             ReferenceNo = transaction.ReferenceNo,
                             ReferenceType = transaction.ReferenceType,
                             AccountBankCurrencyCode = transaction.AccountBankCurrencyCode,
                             BeforeNominal = transaction.BeforeNominal,
                             AfterNominal = transaction.AfterNominal,
                             Nominal = transaction.Nominal,
                             Status = transaction.Status,
                         });

            return Query;
        }

        public ReadResponse<DailyBankTransactionModel> GetReport(int bankId, int month, int year, int clientTimeZoneOffset)
        {
            IQueryable<DailyBankTransactionModel> Query = GetQuery(bankId, month, year, clientTimeZoneOffset);

            //var Test = Query.ToList();
            List<DailyBankTransactionModel> Result = Query.ToList();
            if (Query.ToArray().Count() > 0)
            {
                var BalanceByMonthAndYear = GetBalanceMonthAndYear(bankId, month, year, clientTimeZoneOffset);
                var beforeBalance = BalanceByMonthAndYear.InitialBalance;

                foreach (var item in Result)
                {
                    var afterBalance = beforeBalance + (item.Status.Equals("IN") ? (double)item.Nominal : (double)item.Nominal * -1);
                    item.BeforeNominal = (decimal)beforeBalance;
                    item.AfterNominal = (decimal)afterBalance;
                    beforeBalance = afterBalance;
                }
            }

            Dictionary<string, string> order = new Dictionary<string, string>();

            return new ReadResponse<DailyBankTransactionModel>(Result, Result.Count, order, new List<string>());
        }

        public ReadResponse<DailyBankTransactionModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<DailyBankTransactionModel> Query = _DbSet;

            Query = Query
                .Select(s => new DailyBankTransactionModel
                {
                    Id = s.Id,
                    CreatedUtc = s.CreatedUtc,
                    Code = s.Code,
                    LastModifiedUtc = s.LastModifiedUtc,
                    AccountBankName = s.AccountBankName,
                    AccountBankAccountName = s.AccountBankAccountName,
                    AccountBankAccountNumber = s.AccountBankAccountNumber,
                    AccountBankCode = s.AccountBankCode,
                    AccountBankCurrencyCode = s.AccountBankCurrencyCode,
                    AccountBankCurrencyId = s.AccountBankCurrencyId,
                    AccountBankCurrencySymbol = s.AccountBankCurrencySymbol,
                    AccountBankId = s.AccountBankId,
                    Date = s.Date,
                    ReferenceNo = s.ReferenceNo,
                    ReferenceType = s.ReferenceType,
                    Status = s.Status,
                    SourceType = s.SourceType
                });

            List<string> searchAttributes = new List<string>()
            {
                "Code", "ReferenceNo", "ReferenceType","AccountBankName", "AccountBankCurrencyCode", "Status", "SourceType"
            };

            Query = QueryHelper<DailyBankTransactionModel>.Search(Query, searchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<DailyBankTransactionModel>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<DailyBankTransactionModel>.Order(Query, OrderDictionary);

            Pageable<DailyBankTransactionModel> pageable = new Pageable<DailyBankTransactionModel>(Query, page - 1, size);
            List<DailyBankTransactionModel> Data = pageable.Data.ToList();

            List<DailyBankTransactionModel> list = new List<DailyBankTransactionModel>();
            list.AddRange(
               Data.Select(s => new DailyBankTransactionModel
               {
                   Id = s.Id,
                   CreatedUtc = s.CreatedUtc,
                   Code = s.Code,
                   LastModifiedUtc = s.LastModifiedUtc,
                   AccountBankName = s.AccountBankName,
                   AccountBankAccountName = s.AccountBankAccountName,
                   AccountBankAccountNumber = s.AccountBankAccountNumber,
                   AccountBankCode = s.AccountBankCode,
                   AccountBankCurrencyCode = s.AccountBankCurrencyCode,
                   AccountBankCurrencyId = s.AccountBankCurrencyId,
                   AccountBankCurrencySymbol = s.AccountBankCurrencySymbol,
                   AccountBankId = s.AccountBankId,
                   Date = s.Date,
                   ReferenceNo = s.ReferenceNo,
                   ReferenceType = s.ReferenceType,
                   Status = s.Status,
                   SourceType = s.SourceType
               }).ToList()
            );

            int TotalData = pageable.TotalCount;

            return new ReadResponse<DailyBankTransactionModel>(list, TotalData, OrderDictionary, new List<string>());
        }

        public async Task<DailyBankTransactionModel> ReadByIdAsync(int id)
        {
            return await _DbSet.Where(w => w.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(int id, DailyBankTransactionModel model)
        {
            //not implemented
            var result = await _DbSet.Where(w => w.Id.Equals(id)).FirstOrDefaultAsync();
            return result.Id;
        }

        public async Task<int> DeleteByReferenceNoAsync(string referenceNo)
        {
            var bankTransaction = _DbSet.Where(w => w.ReferenceNo.Equals(referenceNo)).FirstOrDefault();

            if (bankTransaction != null)
            {
                EntityExtension.FlagForDelete(bankTransaction, _IdentityService.Username, _UserAgent);
                _DbSet.Update(bankTransaction);

                var monthlyBalance = _DbMonthlyBalanceSet.Where(w => w.Month.Equals(bankTransaction.Date.Month) && w.Year.Equals(bankTransaction.Date.Year) && w.AccountBankId.Equals(bankTransaction.AccountBankId)).FirstOrDefault();
                var nextMonthBalance = GetNextMonthBalance(bankTransaction.Date.Month, bankTransaction.Date.Year);

                if (monthlyBalance != null)
                {
                    if (bankTransaction.Status.Equals("IN"))
                    {
                        monthlyBalance.RemainingBalance -= (double)bankTransaction.Nominal;
                        if (nextMonthBalance != null)
                        {
                            nextMonthBalance.InitialBalance = monthlyBalance.RemainingBalance;
                            nextMonthBalance.RemainingBalance -= (double)bankTransaction.Nominal;
                            EntityExtension.FlagForUpdate(nextMonthBalance, _IdentityService.Username, _UserAgent);
                            _DbMonthlyBalanceSet.Update(nextMonthBalance);
                        }
                    }
                    else
                    {
                        monthlyBalance.RemainingBalance += (double)bankTransaction.Nominal;
                        if (nextMonthBalance != null)
                        {
                            nextMonthBalance.InitialBalance = monthlyBalance.RemainingBalance;
                            nextMonthBalance.RemainingBalance += (double)bankTransaction.Nominal;
                            EntityExtension.FlagForUpdate(nextMonthBalance, _IdentityService.Username, _UserAgent);
                            _DbMonthlyBalanceSet.Update(nextMonthBalance);
                        }
                    }

                    EntityExtension.FlagForUpdate(monthlyBalance, _IdentityService.Username, _UserAgent);
                    _DbMonthlyBalanceSet.Update(monthlyBalance);
                }
            }

            return await _DbContext.SaveChangesAsync();
        }

        public async Task<int> CreateInOutTransactionAsync(DailyBankTransactionModel model)
        {
            int result = 0;
            var inputModel = model.Clone();
            inputModel.AccountBankAccountName = model.DestinationBankAccountName;
            inputModel.AccountBankAccountNumber = model.DestinationBankAccountNumber;
            inputModel.AccountBankCode = model.DestinationBankCode;
            inputModel.AccountBankCurrencyCode = model.DestinationBankCurrencyCode;
            inputModel.AccountBankCurrencyId = model.DestinationBankCurrencyId;
            inputModel.AccountBankCurrencySymbol = model.DestinationBankCurrencySymbol;
            inputModel.AccountBankId = model.DestinationBankId;
            inputModel.AccountBankName = model.DestinationBankName;
            inputModel.Status = "IN";
            inputModel.DestinationBankAccountName = "";
            inputModel.DestinationBankAccountNumber = "";
            inputModel.DestinationBankCode = "";
            inputModel.DestinationBankCurrencyCode = "";
            inputModel.DestinationBankCurrencyId = 0;
            inputModel.DestinationBankCurrencySymbol = "";
            inputModel.DestinationBankId = 0;
            inputModel.DestinationBankName = "";
            inputModel.Nominal = model.TransactionNominal;

            model.Remark = FormatOutRemark(model);
            inputModel.Remark = FormatInRemark(inputModel, model);

            using (var transaction = _DbContext.Database.BeginTransaction())
            {
                try
                {
                    result += await CreateAsync(model);
                    result += await CreateAsync(inputModel);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }

            }
            return result;
        }

        private string FormatInRemark(DailyBankTransactionModel inputModel, DailyBankTransactionModel model)
        {
            return !string.IsNullOrWhiteSpace(inputModel.Remark) ? $"{inputModel.Remark}\n\nPendanaan dari {model.AccountBankAccountName} - {model.AccountBankName} - {model.AccountBankAccountNumber} - {model.AccountBankCurrencyCode}\nSenilai {string.Format("{0:0,0.0}", model.Nominal)} {model.AccountBankCurrencyCode}" : $"Pendanaan dari {model.AccountBankAccountName} - {model.AccountBankName} - {model.AccountBankAccountNumber} - {model.AccountBankCurrencyCode}\nSenilai {string.Format("{0:0,0.0}", model.Nominal)} {model.AccountBankCurrencyCode}";
        }

        private string FormatOutRemark(DailyBankTransactionModel model)
        {
            return !string.IsNullOrWhiteSpace(model.Remark) ? $"{model.Remark}\n\nPendanaan untuk {model.DestinationBankAccountName} - {model.DestinationBankName} - {model.DestinationBankAccountNumber} - {model.DestinationBankCurrencyCode}\nSenilai {string.Format("{0:0,0.0}", model.TransactionNominal)} {model.DestinationBankCurrencyCode}" : $"Pendanaan untuk {model.DestinationBankAccountName} - {model.DestinationBankName} - {model.DestinationBankAccountNumber} - {model.DestinationBankCurrencyCode}\nSenilai {string.Format("{0:0,0.0}", model.TransactionNominal)} {model.DestinationBankCurrencyCode}";
        }

        public List<DailyBalanceReportViewModel> GetDailyBalanceReport(int bankId, DateTime startDate, DateTime endDate)
        {
            //var result = _DbSet.Where(w => w.AccountBankId.Equals(bankId))
            var query = _DbSet.Where(w => w.Date >= startDate && w.Date <= endDate);

            if (bankId > 0)
            {
                query = query.Where(w => w.AccountBankId.Equals(bankId));
            }

            var result = query.GroupBy(g => g.AccountBankId).Select(s => new DailyBalanceReportViewModel()
            {
                AccountNumber = s.FirstOrDefault().AccountBankAccountNumber,
                Balance = (decimal)s.Sum(sum => sum.Status.Equals("IN") ? sum.Nominal : sum.Nominal * -1),
                BankName = s.FirstOrDefault().AccountBankName,
                Credit = (decimal)s.Sum(sum => sum.Status.Equals("OUT") ? sum.Nominal : 0),
                Debit = (decimal)s.Sum(sum => sum.Status.Equals("IN") ? sum.Nominal : 0),
                CurrencyCode = s.FirstOrDefault().AccountBankCurrencyCode
            });

            
            return result.ToList();
            //throw new NotImplementedException();
        }

        public MemoryStream GenerateExcelDailyBalance(int bankId, DateTime startDate, DateTime endDate, int clientTimeZoneOffset)
        {
            var queryResult = GetDailyBalanceReport(bankId, startDate, endDate);
            var currencyQueryResult = GetDailyBalanceCurrencyReport(bankId, startDate, endDate);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "Nama Bank", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Rekening", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Debit", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Credit", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo", DataType = typeof(double) });

            if (queryResult.ToArray().Count() == 0)
                result.Rows.Add("", "", "", 0, 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                foreach (var item in queryResult)
                {
                    result.Rows.Add(item.BankName, item.AccountNumber, item.CurrencyCode, item.Debit, item.Credit, item.Balance);
                }
            }

            DataTable currency = new DataTable();
            currency.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            currency.Columns.Add(new DataColumn() { ColumnName = "Debit", DataType = typeof(double) });
            currency.Columns.Add(new DataColumn() { ColumnName = "Credit", DataType = typeof(double) });
            currency.Columns.Add(new DataColumn() { ColumnName = "Saldo", DataType = typeof(double) });

            if (currencyQueryResult.ToArray().Count() == 0)
                currency.Rows.Add("", 0, 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                foreach (var item in currencyQueryResult)
                {
                    currency.Rows.Add(item.CurrencyCode, item.Debit, item.Credit, item.Balance);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Saldo Harian"), new KeyValuePair<DataTable, string>(currency, "Saldo Harian Mata Uang") }, true);
        }

        public List<DailyBalanceCurrencyReportViewModel> GetDailyBalanceCurrencyReport(int bankId, DateTime startDate, DateTime endDate)
        {
            var query = _DbSet.Where(w => w.Date >= startDate && w.Date <= endDate);

            if (bankId > 0)
            {
                query = query.Where(w => w.AccountBankId.Equals(bankId));
            }

            var currencyResult = query.GroupBy(g => g.AccountBankCurrencyId).Select(s => new DailyBalanceCurrencyReportViewModel()
            {
                Balance = (decimal)s.Sum(sum => sum.Status.Equals("IN") ? sum.Nominal : sum.Nominal * -1),
                Credit = (decimal)s.Sum(sum => sum.Status.Equals("OUT") ? sum.Nominal : 0),
                Debit = (decimal)s.Sum(sum => sum.Status.Equals("IN") ? sum.Nominal : 0),
                CurrencyCode = s.FirstOrDefault().AccountBankCurrencyCode
            });


            return currencyResult.ToList();
        }
    }
}

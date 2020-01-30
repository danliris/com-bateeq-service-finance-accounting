using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditBalance;
using Com.Danliris.Service.Finance.Accounting.Lib.Helpers;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditBalance;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CreditBalance
{
    public class CreditBalanceService : ICreditBalanceService
    {
        private const string UserAgent = "finance-service";
        protected DbSet<CreditorAccountModel> DbSet;
        protected IIdentityService IdentityService;
        public FinanceDbContext DbContext;

        public CreditBalanceService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<CreditorAccountModel>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public List<CreditBalanceViewModel> GetReport(bool isImport, string suplierName, int month, int year, int offSet)
        {
            IQueryable<CreditorAccountModel> query = DbContext.CreditorAccounts.Where(x => x.SupplierIsImport == isImport).AsQueryable();
            List<CreditBalanceViewModel> result = new List<CreditBalanceViewModel>();
            int previousMonth = month - 1;
            int previousYear = year;

            if (previousMonth == 0)
            {
                previousMonth = 12;
                previousYear = year - 1;
            }

            query = query.Where(x => x.UnitReceiptNoteDate.HasValue && x.UnitReceiptNoteDate.Value.Month == month && x.UnitReceiptNoteDate.Value.Year == year);
            if (!string.IsNullOrEmpty(suplierName))
                query = query.Where(x => x.SupplierName == suplierName);



            foreach (var item in query.GroupBy(x => x.SupplierCode).ToList())
            {
                var productsUnion = string.Join("\n", item.Select(x => x.Products).ToList());
                var uniqueProducts = string.Join("\n", productsUnion.Split("\n").Distinct());

                var creditBalance = new CreditBalanceViewModel()
                {
                    StartBalance = DbSet.AsQueryable().Where(x => x.SupplierCode == item.Key
                                    && x.UnitReceiptNoteDate.HasValue && x.UnitReceiptNoteDate.Value.Month == previousMonth
                                    && x.UnitReceiptNoteDate.Value.Year == previousYear).ToList().Sum(x => x.FinalBalance),
                    Products = uniqueProducts,
                    Purchase = item.Sum(x => x.UnitReceiptMutation),
                    Payment = item.Sum(x => x.BankExpenditureNoteMutation),
                    FinalBalance = item.Sum(x => x.FinalBalance),
                    SupplierName = item.FirstOrDefault() == null ? "" : item.FirstOrDefault().SupplierName ?? "",
                    Currency = item.FirstOrDefault() == null ? "" : item.FirstOrDefault().CurrencyCode ?? "",
                    CurrencyRate = item.FirstOrDefault() == null ? 1 : item.FirstOrDefault().CurrencyRate
                };
                creditBalance.FinalBalance = creditBalance.StartBalance + creditBalance.Purchase - creditBalance.Payment;
                result.Add(creditBalance);
            }

            return result.OrderBy(x => x.Currency).ThenBy(x => x.Products).ThenBy(x => x.SupplierName).ToList();
        }

        public MemoryStream GenerateExcel(bool isImport, string suplierName, int month, int year, int offSet)
        {
            var data = GetReport(isImport, suplierName, month, year, offSet);

            DataTable dt = new DataTable();

            // v1
            //dt.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            //dt.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(string) });
            //dt.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(string) });
            //dt.Columns.Add(new DataColumn() { ColumnName = "Pembelian", DataType = typeof(string) });
            //dt.Columns.Add(new DataColumn() { ColumnName = "Pembayaran", DataType = typeof(string) });
            //dt.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(string) });

            //v2 
            dt.Columns.Add(new DataColumn() { ColumnName = "Supplier", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Pembelian", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Pembayaran", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir", DataType = typeof(string) });

            if (isImport)
            {
                dt.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal (IDR)", DataType = typeof(string) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Pembelian (IDR)", DataType = typeof(string) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Pembayaran (IDR)", DataType = typeof(string) });
                dt.Columns.Add(new DataColumn() { ColumnName = "Saldo Akhir (IDR)", DataType = typeof(string) });
            }

            if (data.Count == 0)
            {
                if (isImport)
                {
                    dt.Rows.Add("", "", "", "", "", "", "", "", "", "");
                }
                else
                {
                    dt.Rows.Add("", "", "", "", "", "");
                }
            }
            else
            {
                foreach (var item in data)
                {
                    // v1
                    //dt.Rows.Add(item.Currency, item.SupplierName, item.StartBalance.ToString("#,##0"), item.Purchase.ToString("#,##0"),
                    //    item.Payment.ToString("#,##0"), item.FinalBalance.ToString("#,##0"));

                    // v2

                    if (isImport)
                    {
                        dt.Rows.Add(item.SupplierName, item.Currency, item.StartBalance.ToString("#,##0"), item.Purchase.ToString("#,##0"),
                                item.Payment.ToString("#,##0"), item.FinalBalance.ToString("#,##0"), (item.StartBalance * item.CurrencyRate).ToString("#,##0"),
                                (item.Purchase * item.CurrencyRate).ToString("#,##0"), (item.Payment * item.CurrencyRate).ToString("#,##0"),
                                (item.FinalBalance * item.CurrencyRate).ToString("#,##0"));
                    }
                    else
                    {

                        dt.Rows.Add(item.SupplierName, item.Currency, item.StartBalance.ToString("#,##0"), item.Purchase.ToString("#,##0"),
                                item.Payment.ToString("#,##0"), item.FinalBalance.ToString("#,##0"));
                    }
                }
            }

            return CreateExcel(isImport, month, year, new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Saldo Hutang Lokal") }, true);
        }

        public ReadResponse<CreditBalanceViewModel> GetReport(bool isImport, int page, int size, string suplierName, int month, int year, int offSet)
        {
            var queries = GetReport(isImport, suplierName, month, year, offSet);

            Pageable<CreditBalanceViewModel> pageable = new Pageable<CreditBalanceViewModel>(queries, page - 1, size);
            List<CreditBalanceViewModel> data = pageable.Data.ToList();

            return new ReadResponse<CreditBalanceViewModel>(queries, pageable.TotalCount, new Dictionary<string, string>(), new List<string>());
        }

        private MemoryStream CreateExcel(bool isImport, int month, int year, List<KeyValuePair<DataTable, string>> dtSourceList, bool styling = false)
        {
            ExcelPackage package = new ExcelPackage();
            foreach (KeyValuePair<DataTable, string> item in dtSourceList)
            {
                var sheet = package.Workbook.Worksheets.Add(item.Value);

                var lastDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                sheet.Cells["A1:B3"].Style.Font.Size = 14;
                sheet.Cells["A1:B3"].Style.Font.Bold = true;
                sheet.Cells["A1:B1"].Merge = true;
                sheet.Cells["A2:B2"].Merge = true;
                sheet.Cells["A3:B3"].Merge = true;
                sheet.Cells["A1"].Value = "PT DANLIRIS";

                if (isImport)
                {
                    sheet.Cells["A2"].Value = "SALDO HUTANG IMPOR";
                }
                else
                {
                    sheet.Cells["A2"].Value = "SALDO HUTANG LOKAL";
                }
                sheet.Cells["A3"].Value = "PER " + lastDate.ToString("dd MMMM yyyy").ToUpper();
                sheet.Cells["A4"].LoadFromDataTable(item.Key, true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            }
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }

    }
}

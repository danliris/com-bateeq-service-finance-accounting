using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Com.Moonlay.Models;
using System.Threading.Tasks;
using System.Linq;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Newtonsoft.Json;
using Com.Moonlay.NetCore.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.PaymentDispositionNote
{
    public class PaymentDispositionNoteService : IPaymentDispositionNoteService
    {
        private const string UserAgent = "finance-service";
        protected DbSet<PaymentDispositionNoteModel> DbSet;
        public IIdentityService IdentityService;
        private readonly IAutoDailyBankTransactionService _autoDailyBankTransactionService;
        public readonly IServiceProvider ServiceProvider;
        public FinanceDbContext DbContext;

        public PaymentDispositionNoteService(IServiceProvider serviceProvider, FinanceDbContext dbContext)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
            DbSet = dbContext.Set<PaymentDispositionNoteModel>();
            IdentityService = serviceProvider.GetService<IIdentityService>();
            _autoDailyBankTransactionService = serviceProvider.GetService<IAutoDailyBankTransactionService>();
        }

        public void CreateModel(PaymentDispositionNoteModel model)
        {
            EntityExtension.FlagForCreate(model, IdentityService.Username, UserAgent);
            foreach (var item in model.Items)
            {
                PurchasingDispositionExpeditionModel expedition = DbContext.PurchasingDispositionExpeditions.FirstOrDefault(ex => ex.Id.Equals(item.PurchasingDispositionExpeditionId));
                EntityExtension.FlagForCreate(item, IdentityService.Username, UserAgent);
                expedition.IsPaid = true;
                expedition.BankExpenditureNoteNo = model.PaymentDispositionNo;
                expedition.BankExpenditureNoteDate = model.PaymentDate;
                foreach (var detail in item.Details)
                {
                    EntityExtension.FlagForCreate(detail, IdentityService.Username, UserAgent);
                }
            }
            DbSet.Add(model);
        }

        public async Task<int> CreateAsync(PaymentDispositionNoteModel model)
        {
            model.PaymentDispositionNo = await GenerateNo(model, 7);
            CreateModel(model);
            await _autoDailyBankTransactionService.AutoCreateFromPaymentDisposition(model);
            return await DbContext.SaveChangesAsync();
        }

        async Task<string> GenerateNo(PaymentDispositionNoteModel model, int clientTimeZoneOffset)
        {
            DateTimeOffset Now = model.PaymentDate;
            string Year = Now.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("yy");
            string Month = Now.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("MM");
            string Day = Now.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd");
            //PD + 2 digit tahun + 2 digit bulan + 2 digit tgl + 3 digit urut
            string no = $"PD-{Year}-{Month}-{Day}-";
            int Padding = 3;

            var lastNo = await this.DbSet.Where(w => w.PaymentDispositionNo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.PaymentDispositionNo).FirstOrDefaultAsync();
            no = $"{no}";

            if (lastNo == null)
            {
                return no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastNo.PaymentDispositionNo.Replace(no, "")) + 1;
                return no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }
        }

        public async Task DeleteModel(int id)
        {
            PaymentDispositionNoteModel model = await ReadByIdAsync(id);
            foreach (var item in model.Items)
            {
                PurchasingDispositionExpeditionModel expedition = DbContext.PurchasingDispositionExpeditions.FirstOrDefault(ex => ex.Id.Equals(item.PurchasingDispositionExpeditionId));
                EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);
                expedition.IsPaid = false;
                expedition.BankExpenditureNoteNo = null;
                expedition.BankExpenditureNoteDate = DateTimeOffset.MinValue;
                foreach (var detail in item.Details)
                {
                    EntityExtension.FlagForDelete(detail, IdentityService.Username, UserAgent, true);
                }
            }
            EntityExtension.FlagForDelete(model, IdentityService.Username, UserAgent, true);
            DbSet.Update(model);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var existingModel = DbSet
                        .Include(d => d.Items)
                        .ThenInclude(d => d.Details)
                        .Single(dispo => dispo.Id == id && !dispo.IsDeleted);

            await _autoDailyBankTransactionService.AutoCreateFromPaymentDisposition(existingModel);
            await DeleteModel(id);
            return await DbContext.SaveChangesAsync();
        }

        public Task<PaymentDispositionNoteModel> ReadByIdAsync(int id)
        {
            return DbSet.Include(m => m.Items).ThenInclude(m => m.Details).FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
        }

        public async Task<int> UpdateAsync(int id, PaymentDispositionNoteModel model)
        {
            var existingModel = DbSet
                        .Include(d => d.Items)
                        .ThenInclude(d => d.Details)
                        .Single(dispo => dispo.Id == id && !dispo.IsDeleted);
            UpdateModel(id, model);
            await _autoDailyBankTransactionService.AutoRevertFromPaymentDisposition(existingModel);
            await _autoDailyBankTransactionService.AutoCreateFromPaymentDisposition(model);
            return await DbContext.SaveChangesAsync();
        }

        public void UpdateModel(int id, PaymentDispositionNoteModel model)
        {
            PaymentDispositionNoteModel exist = DbSet
                        .Include(d => d.Items)
                        .ThenInclude(d => d.Details)
                        .Single(dispo => dispo.Id == id && !dispo.IsDeleted);


            exist.BGCheckNumber = model.BGCheckNumber;
            exist.PaymentDate = model.PaymentDate;

            foreach (var item in exist.Items)
            {
                PaymentDispositionNoteItemModel itemModel = model.Items.FirstOrDefault(prop => prop.Id.Equals(item.Id));

                if (itemModel == null)
                {
                    PurchasingDispositionExpeditionModel expedition = DbContext.PurchasingDispositionExpeditions.FirstOrDefault(ex => ex.Id.Equals(item.PurchasingDispositionExpeditionId));
                    expedition.IsPaid = false;
                    expedition.BankExpenditureNoteNo = null;
                    expedition.BankExpenditureNoteDate = DateTimeOffset.MinValue;

                    EntityExtension.FlagForDelete(item, IdentityService.Username, UserAgent, true);

                    foreach (var detail in item.Details)
                    {
                        EntityExtension.FlagForDelete(detail, IdentityService.Username, UserAgent, true);
                        //DbContext.PaymentDispositionNoteDetails.Update(detail);
                    }

                    //DbContext.PaymentDispositionNoteItems.Update(item);
                }
                else
                {
                    EntityExtension.FlagForUpdate(item, IdentityService.Username, UserAgent);

                    foreach (var detail in DbContext.PaymentDispositionNoteDetails.AsNoTracking().Where(p => p.PaymentDispositionNoteItemId == item.Id))
                    {
                        EntityExtension.FlagForUpdate(detail, IdentityService.Username, UserAgent);

                    }
                }
            }

            EntityExtension.FlagForUpdate(exist, IdentityService.Username, UserAgent);
            //DbSet.Update(exist);
        }

        public ReadResponse<PaymentDispositionNoteModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<PaymentDispositionNoteModel> Query = this.DbSet.Include(m => m.Items).ThenInclude(m => m.Details);
            List<string> searchAttributes = new List<string>()
            {
                "PaymentDispositionNo", "Items.DispositionNo",  "SupplierName", "BankCurrencyCode","BankName"
            };

            Query = QueryHelper<PaymentDispositionNoteModel>.Search(Query, searchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<PaymentDispositionNoteModel>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<PaymentDispositionNoteModel>.Order(Query, OrderDictionary);

            Pageable<PaymentDispositionNoteModel> pageable = new Pageable<PaymentDispositionNoteModel>(Query, page - 1, size);
            List<PaymentDispositionNoteModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return new ReadResponse<PaymentDispositionNoteModel>(Data, TotalData, OrderDictionary, new List<string>());
        }

        public ReadResponse<PaymentDispositionNoteDetailModel> ReadDetailsByEPOId(string epoId)
        {
            List<PaymentDispositionNoteDetailModel> paymentDispositionNoteDetails = DbContext.PaymentDispositionNoteDetails.Where(a => a.EPOId == epoId).ToList();

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>("{ }");

            int TotalData = paymentDispositionNoteDetails.Count;

            return new ReadResponse<PaymentDispositionNoteDetailModel>(paymentDispositionNoteDetails, TotalData, OrderDictionary, new List<string>());
        }
    }
}

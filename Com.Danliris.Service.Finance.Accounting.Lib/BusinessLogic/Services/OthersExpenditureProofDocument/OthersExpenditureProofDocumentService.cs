using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.OthersExpenditureProofDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.OthersExpenditureProofDocumentViewModels;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Services.OthersExpenditureProofDocument
{
    public class OthersExpenditureProofDocumentService : IOthersExpenditureProofDocumentService
    {
        private const string _userAgent = "finance-accounting-service";
        private const int _taskDone = 1;

        private readonly FinanceDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly IIdentityService _identityService;
        private readonly IAutoJournalService _autoJournalService;
        private readonly IAutoDailyBankTransactionService _autoDailyBankTransactionService;
        private readonly DbSet<OthersExpenditureProofDocumentModel> _dbSet;
        private readonly DbSet<OthersExpenditureProofDocumentItemModel> _itemDbSet;

        public OthersExpenditureProofDocumentService(FinanceDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<OthersExpenditureProofDocumentModel>();
            _itemDbSet = dbContext.Set<OthersExpenditureProofDocumentItemModel>();

            _serviceProvider = serviceProvider;
            _identityService = serviceProvider.GetService<IIdentityService>();
            _autoJournalService = serviceProvider.GetService<IAutoJournalService>();
            _autoDailyBankTransactionService = serviceProvider.GetService<IAutoDailyBankTransactionService>();
        }

        public async Task<int> CreateAsync(OthersExpenditureProofDocumentCreateUpdateViewModel viewModel)
        {
            var model = viewModel.MapToModel();
            model.DocumentNo = DocumentNoGenerator(viewModel);
            EntityExtension.FlagForCreate(model, _identityService.Username, _userAgent);
            _dbSet.Add(model);
            await _dbContext.SaveChangesAsync();

            var itemModels = viewModel.MapItemToModel().Select(item =>
            {
                EntityExtension.FlagForCreate(item, _identityService.Username, _userAgent);
                item.OthersExpenditureProofDocumentId = model.Id;
                return item;
            }).ToList();
            _itemDbSet.UpdateRange(itemModels);
            await _dbContext.SaveChangesAsync();

            await _autoJournalService.AutoJournalFromOthersExpenditureProof(viewModel, model.DocumentNo);
            await _autoDailyBankTransactionService.AutoCreateFromOthersExpenditureProofDocument(model, itemModels);

            return _taskDone;
        }

        private string DocumentNoGenerator(OthersExpenditureProofDocumentCreateUpdateViewModel viewModel)
        {
            var latestDocumentNo = _dbSet.IgnoreQueryFilters().Where(document => document.DocumentNo.Contains(viewModel.AccountBankCode)).OrderByDescending(document => document.Id).Select(document => new { document.DocumentNo, document.CreatedUtc }).FirstOrDefault();

            var now = DateTimeOffset.Now;
            if (latestDocumentNo == null)
            {
                return $"{now.ToString("yy")}{now.ToString("MM")}{viewModel.AccountBankCode}K0001";
            }
            else
            {
                if (latestDocumentNo.CreatedUtc.Month != now.Month)
                    return $"{now.ToString("yy")}{now.ToString("MM")}{viewModel.AccountBankCode}K0001";
                else
                {
                    var numberString = latestDocumentNo.DocumentNo.Split("K").ToList()[1];
                    var number = int.Parse(numberString) + 1;
                    return $"{now.ToString("yy")}{now.ToString("MM")}{viewModel.AccountBankCode}K{number.ToString().PadLeft(4, '0')}";
                }
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            var model = _dbSet.FirstOrDefault(document => document.Id == id);
            EntityExtension.FlagForDelete(model, _identityService.Username, _userAgent);
            _dbSet.Update(model);

            var itemModels = _itemDbSet.Where(item => item.OthersExpenditureProofDocumentId == id).ToList();
            itemModels = itemModels.Select(item =>
            {
                EntityExtension.FlagForDelete(item, _identityService.Username, _userAgent);
                return item;
            }).ToList();
            _itemDbSet.UpdateRange(itemModels);

            await _dbContext.SaveChangesAsync();
            await _autoJournalService.AutoJournalReverseFromOthersExpenditureProof(model.DocumentNo);
            await _autoDailyBankTransactionService.AutoRevertFromOthersExpenditureProofDocument(model, itemModels);

            return _taskDone;
        }

        public async Task<List<int>> GetAccountBankIds(string keyword)
        {
            var http = _serviceProvider.GetService<IHttpClientService>();

            var response = await http.GetAsync(APIEndpoint.Core + $"master/account-banks?keyword={keyword}&size={int.MaxValue}");

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonSerializationSetting = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore };

            var result = JsonConvert.DeserializeObject<APIDefaultResponse<List<AccountBank>>>(responseString, jsonSerializationSetting);

            return result.data.Select(accountBank => accountBank.Id).ToList();
        }

        public async Task<OthersExpenditureProofPagedListViewModel> GetPagedListAsync(int page, int size, string order, string keyword, string filter)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var accountBankIds = await GetAccountBankIds(keyword);
                query = query.Where(document => document.Type.Contains(keyword) || document.DocumentNo.Contains(keyword) || accountBankIds.Contains(document.AccountBankId));
            }

            var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            query = QueryHelper<OthersExpenditureProofDocumentModel>.Filter(query, filterDictionary);

            var orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            query = QueryHelper<OthersExpenditureProofDocumentModel>.Order(query, orderDictionary);


            var total = await query.CountAsync();
            var data = query.Skip((page - 1) * size).Take(size).Select(document => new OthersExpenditureProofListViewModel()
            {
                AccountBankId = document.AccountBankId,
                DocumentNo = document.DocumentNo,
                Type = document.Type,
                Id = document.Id,
                Date = document.Date
            }).ToList();

            data = data.Select(element =>
            {
                element.Total = _itemDbSet.Where(item => item.OthersExpenditureProofDocumentId == element.Id).Sum(item => item.Debit);
                return element;
            }).ToList();

            return new OthersExpenditureProofPagedListViewModel()
            {
                Data = data,
                Page = page,
                Size = size,
                Total = total,
                Count = data.Count
            };
        }

        public async Task<int> UpdateAsync(int id, OthersExpenditureProofDocumentCreateUpdateViewModel viewModel)
        {
            var itemIds = viewModel.Items.Select(item => item.Id.GetValueOrDefault()).ToList();

            var itemModels = await _itemDbSet.Where(item => itemIds.Contains(item.Id)).ToListAsync();
            var model = await _dbSet.AsNoTracking().FirstOrDefaultAsync(document => document.Id == id);
            await _autoDailyBankTransactionService.AutoRevertFromOthersExpenditureProofDocument(model, itemModels);

            var itemModelsToUpdate = viewModel.MapItemToModel();

            foreach (var itemModelToUpdate in itemModelsToUpdate)
            {
                if (itemModelToUpdate.Id != 0)
                {
                    var existedItem = itemModels.FirstOrDefault(item => item.Id == itemModelToUpdate.Id);
                    existedItem.UpdateCOAId(itemModelToUpdate.COAId);
                    existedItem.UpdateDebit(itemModelToUpdate.Debit);
                    existedItem.UpdateRemark(itemModelToUpdate.Remark);

                    if (existedItem.IsUpdated)
                    {
                        EntityExtension.FlagForUpdate(existedItem, _identityService.Username, _userAgent);
                        _itemDbSet.Update(existedItem);
                    }
                }
                else
                {
                    EntityExtension.FlagForCreate(itemModelToUpdate, _identityService.Username, _userAgent);
                    _itemDbSet.Add(itemModelToUpdate);
                }
            }

            var itemModelsToDelete = await _itemDbSet.Where(item => !itemIds.Contains(item.Id)).ToListAsync();
            itemModelsToDelete = itemModelsToDelete.Select(item =>
            {
                EntityExtension.FlagForDelete(item, _identityService.Username, _userAgent);
                return item;
            }).ToList();
            _itemDbSet.UpdateRange(itemModelsToDelete);

            await _dbContext.SaveChangesAsync();
            await _autoJournalService.AutoJournalReverseFromOthersExpenditureProof(model.DocumentNo);
            await _autoJournalService.AutoJournalFromOthersExpenditureProof(viewModel, model.DocumentNo);
            await _autoDailyBankTransactionService.AutoCreateFromOthersExpenditureProofDocument(model, itemModelsToUpdate);

            await _autoDailyBankTransactionService.AutoRevertFromOthersExpenditureProofDocument(model, itemModels);
            await _autoDailyBankTransactionService.AutoCreateFromOthersExpenditureProofDocument(model, itemModelsToUpdate);

            return _taskDone;
        }

        public async Task<OthersExpenditureProofDocumentViewModel> GetSingleByIdAsync(int id)
        {
            var model = await _dbSet.AsNoTracking().FirstOrDefaultAsync(document => document.Id == id);
            var items = _itemDbSet.AsNoTracking().Where(item => item.OthersExpenditureProofDocumentId == id).ToList();

            return new OthersExpenditureProofDocumentViewModel(model, items);
        }
    }
}
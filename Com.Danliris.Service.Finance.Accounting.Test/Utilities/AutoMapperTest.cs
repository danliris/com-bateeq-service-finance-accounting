using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.LockTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.LockTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.LockTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PaymentDispositionNoteViewModel;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionExpedition;
using System.Collections.Generic;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Utilities
{
    public class AutoMapperTest
    {
        public AutoMapperTest()
        {
        }

        [Fact]
        public void Should_Success_Map_DailyBankTransaction()
        {
            var mapper = new MapperConfiguration(configuration => configuration.AddProfile<DailyBankTransactionProfile>()).CreateMapper();
            var model = new DailyBankTransactionModel();
            var vm = mapper.Map<DailyBankTransactionViewModel>(model);
            Assert.True(true);
        }

        [Fact]
        public void Should_Success_Map_JournalTransaction()
        {
            var mapper = new MapperConfiguration(configuration => configuration.AddProfile<JournalTransactionProfile>()).CreateMapper();
            var model = new JournalTransactionModel()
            {
                Items = new List<JournalTransactionItemModel>()
                {
                    new JournalTransactionItemModel()
                }
            };
            var vm = mapper.Map<JournalTransactionViewModel>(model);
            Assert.True(true);
        }

        [Fact]
        public void Should_Success_Map_LockTransaction()
        {
            var mapper = new MapperConfiguration(configuration => configuration.AddProfile<LockTransactionProfile>()).CreateMapper();
            var model = new LockTransactionModel();
            var vm = mapper.Map<LockTransactionViewModel>(model);
            Assert.True(true);
        }

        [Fact]
        public void Should_Success_Map_COA()
        {
            var mapper = new MapperConfiguration(configuration => configuration.AddProfile<COAProfile>()).CreateMapper();
            var model = new COAModel();
            var vm = mapper.Map<COAViewModel>(model);
            Assert.True(true);
        }

        [Fact]
        public void Should_Success_Map_Payment_Disposition()
        {
            var mapper = new MapperConfiguration(configuration => configuration.AddProfile<PaymentDispositionNoteProfile>()).CreateMapper();
            var model = new PaymentDispositionNoteModel();
            var vm = mapper.Map<PaymentDispositionNoteViewModel>(model);
            Assert.True(true);
        }

        [Fact]
        public void Should_Success_Map_Purchasing_Disposition()
        {
            var mapper = new MapperConfiguration(configuration => configuration.AddProfile<PurchasingDispositionExpeditionProfile>()).CreateMapper();
            var model = new PurchasingDispositionExpeditionModel();
            var vm = mapper.Map<PurchasingDispositionExpeditionViewModel>(model);
            Assert.True(true);
        }

    }
}

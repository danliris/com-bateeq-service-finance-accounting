using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.Test.Services.PurchasingDispositionExpedition;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.PurchasingDispositionExpedition
{
    public class PurchasingDispositionDataUtil
    {
        public PurchasingDispositionExpeditionViewModel GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new PurchasingDispositionExpeditionViewModel
            {
                dispositionNo="No",
                position=1
            };
            return data;
        }

        public Dictionary<string, object> GetResultFormatterOk()
        {
            var data = GetNewData();

            Dictionary<string, object> result =
                new resultFormatterForTest("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public string GetResultFormatterOkString()
        {
            var result = GetResultFormatterOk();

            return JsonConvert.SerializeObject(result);
        }

        public Dictionary<string, object> GetMultipleResultFormatterOk()
        {
            var data = new List<PurchasingDispositionExpeditionViewModel> { GetNewData() };

            Dictionary<string, object> result =
                new resultFormatterForTest("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public string GetMultipleResultFormatterOkString()
        {
            var result = GetMultipleResultFormatterOk();

            return JsonConvert.SerializeObject(result);
        }
    }
}

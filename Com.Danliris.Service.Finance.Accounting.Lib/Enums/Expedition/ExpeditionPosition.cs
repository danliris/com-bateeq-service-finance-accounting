using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition
{
    public enum ExpeditionPosition
    {
        [Description("Bag. Pembelian")]
        PURCHASING_DIVISION = 1,
        [Description("Dikirim ke Bag. Verifikasi")]
        SEND_TO_VERIFICATION_DIVISION = 2,
        [Description("Bag. Verifikasi")]
        VERIFICATION_DIVISION = 3,
        [Description("Dikirim ke Bag. Kasir")]
        SEND_TO_CASHIER_DIVISION = 4,
        [Description("Dikirim ke Bag. Keuangan")]
        SEND_TO_ACCOUNTING_DIVISION = 5,
        [Description("Dikirim ke Bag. Pembelian")]
        SEND_TO_PURCHASING_DIVISION = 6,
        [Description("Bag. Kasir")]
        CASHIER_DIVISION = 7,
        [Description("Bag. Keuangan")]
        FINANCE_DIVISION = 8
    }

    // Display Friendly Name for enum
    // source : https://www.codingame.com/playgrounds/2487/c---how-to-display-friendly-names-for-enumerations
    public static class ExpeditionPositionExtensions
    {
        public static string ToDescriptionString(this ExpeditionPosition me)
        {
            Type enumType = me.GetType();
            MemberInfo[] memberInfo = enumType.GetMember(me.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var _attr = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (_attr != null && _attr.Count() > 0)
                {
                    return ((DescriptionAttribute)_attr.ElementAt(0)).Description;
                }
            }
            return me.ToString();
        }
    }
}

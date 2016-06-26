using System;
using Caribs.Common.Helpers;
using Caribs.Common.Services;
using Caribs.Services.Data;
using TimeZone = Caribs.Common.Helpers.TimeZone;

namespace Caribs.Services
{
    public class SmsService
    {
        private const string Sign = "Promobiz";

        private static SmsService _instance;
        public static SmsService Instance
        {
            get { return _instance ?? (_instance = new SmsService()); }
        }

        public void SendSms(string toPhone, string message)
        {
            var mySqlService = new MySqlService(SettingsService.TurboSmsDbHost, SettingsService.TurboSmsDbName,
                SettingsService.TurboSmsDbUserName, SettingsService.TurboSmsDbUserPassword);
            var smsInsert =
                string.Format(
                    "Insert Into Faraon_ua (`number`, `sign`, `message`, `send_time`) Values ('{0}', '{1}', '{2}', '{3}');",
                    toPhone, Sign, message, DateTime.Now.AddMinutes(15).ToLocalizedDateTime(TimeZone.Ukraine).ToString("YYYY-MM-DD HH:mm"));
            mySqlService.ExecuteSql(smsInsert);
        }
    }
}

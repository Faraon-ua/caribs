using System.Configuration;

namespace Caribs.Common.Services
{
    public static class SettingsService
    {
        public static string CaribsLoginUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["CaribsLoginUrl"];
            }
        }

        public static string CaribsSocialBonusUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["CaribsSocialBonusUrl"];
            }
        }
        public static string CaribsLoginBodyTemplate
        {
            get
            {
                return "LoginForm%5Blogin%5D={0}&LoginForm%5Bpassword%5D={1}&login-button=";
            }
        }
        public static string CaribsTransactionHistoryUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["CaribsTransactionHistoryUrl"];
            }
        }
        public static string CaribsTransactionAdvertisingBonusText
        {
            get
            {
                return ConfigurationManager.AppSettings["CaribsTransactionAdvertisingBonusText"];
            }
        }
        public static string CaribsTransactionDateFormatText
        {
            get
            {
                return ConfigurationManager.AppSettings["CaribsTransactionDateFormatText"];
            }
        }
        public static string CaribsSecretYandexKey
        {
            get
            {
                return ConfigurationManager.AppSettings["CaribsSecretYandexKey"];
            }
        }

        public static string TurboSmsDbHost
        {
            get
            {
                return ConfigurationManager.AppSettings["TurboSmsDbHost"];
            }
        }
        public static string TurboSmsDbName
        {
            get
            {
                return ConfigurationManager.AppSettings["TurboSmsDbName"];
            }
        }
        public static string TurboSmsDbUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["TurboSmsDbUserName"];
            }
        }
        public static string TurboSmsDbUserPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["TurboSmsDbUserPassword"];
            }
        }
        public static string NotificationEmails
        {
            get
            {
                return ConfigurationManager.AppSettings["NotificationEmails"];
            }
        }
    }
}

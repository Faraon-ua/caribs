using System;
using System.Net.Mail;
using System.Text;

namespace Caribs.Common.Helpers
{
    public class EmailHelper
    {
        private const string From = "promobiz-soft@yandex.ru";
        public const string ToAdmin = "faraon.ua@gmail.com";
        private const string DisplayName = "Promobiz Soft";

        private static EmailHelper _instance;
        public static EmailHelper Instance
        {
            get { return _instance ?? (_instance = new EmailHelper()); }
        }

        private bool SendEmail(string fromad, string toad, string body, string subjectcontent)
        {
            MailMessage usermail = Mailbodplain(fromad, toad, body, DisplayName, subjectcontent);

            bool result;
            SmtpClient client = new SmtpClient();
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.Send(usermail);
            return true;
        }

        private MailMessage Mailbodplain(string fromad, string toad, string body, string displayName, string subjectcontent)
        {
            var mail = new MailMessage();
            string from = fromad;
            string to = toad;
            mail.To.Add(to);
            mail.From = new MailAddress(from, displayName, System.Text.Encoding.UTF8);
            mail.Subject = subjectcontent;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = body;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            return mail;
        }

        public void SendSchedulerFailed(string exceptionMessage)
        {
            SendEmail(From, ToAdmin, exceptionMessage, "Scheduler failed");
        }

        public void SendLoginFailed(string email, string accontName)
        {
            var body = new StringBuilder();
            body.Append(string.Format("Не удалось залогинится на сайт caribbeanbridge.com под логином {0} с реквизитами, которые вы указали, пожалуйста проверте правильность ввода логина и пароля.", accontName));
            SendEmail(From, email, body.ToString(), string.Format("[{0}] Login failed to caribbeanbridge.com", accontName));
        }

        public void SendAutoClickFailed(string email, string accountName, string message = null)
        {
            var body = new StringBuilder();
            body.Append(string.Format("Автокликер не сработал для аккаунта {0}, {1}", accountName, message));
            SendEmail(From, email, body.ToString(), "Автокликер не сработал!!!");
        }
        public void SendNewAwards(string newAwardsHtml, string email, string accountName)
        {
            var body = new StringBuilder();
            body.Append("<table border='1'>");
            body.Append(newAwardsHtml);
            body.Append("</table>");
            SendEmail(From, email, body.ToString(), string.Format("Новые операции по счету на вашем аккаунте {0} в Caribbean Bridge", accountName));
        }

        public void SendAutoClickSubscribed(string email, string accountName)
        {
            var body = new StringBuilder();
            body.Append(string.Format("Ваш аккаунт {0} был успешно зарегистрирован на сервисе Автокликер для CaribbeanBridge. Спасибо за использование нашего сервиса.", accountName));
            SendEmail(From, email, body.ToString(), "Регистрация аккаунта для автокликера CaribbeanBridge");
        } 
        
        public void SendNewPayment(string notification_type, string operation_id, string label, string datetime,
                decimal amount, decimal withdraw_amount, string sender, string sha1_hash, string currency, bool codepro)
        {
            string paramString = String.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
              notification_type, operation_id, amount, currency, datetime, sender,
              codepro.ToString().ToLower(), label);
            SendEmail(From, ToAdmin, paramString, "New Payment");
        }
    }
}
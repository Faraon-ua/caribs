using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Caribs.Common.Helpers;
using Caribs.Common.Services;
using Caribs.Domain.DbContext;
using Caribs.Domain.Models;
using Caribs.Models;
using Caribs.Services.Clients;

namespace Caribs.Controllers
{
    public class SoftController : Controller
    {
        private readonly int[] _subscriptionForMonth = { 1, 2, 3, 4, 5, 6 };

        //
        // GET: /Soft/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Registration()
        {
            ViewBag.SubscriptionFor = _subscriptionForMonth.Select(entry => new SelectListItem() { Text = entry.ToString() + " мес", Value = entry.ToString() }).ToList();
            var currencyHelper = new CurrencyHelper();
            var viewModel = new SoftRegistrationViewModel()
            {
                CurrencyRate = currencyHelper.CurrencyRate
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> ValidateCaribsAccount(string userName, string password, string email, string phone)
        {
            phone = phone.Replace("(", "").Replace(")", "").Replace("-", "");
            Guid userId;
            var client = new CaribsClient();
            var loginResult = await client.Login(userName, password).ConfigureAwait(false);
            if (!loginResult)
                return Json(new { loginResult = false });
            using (var db = new ApplicationDbContext())
            {
                var existingAccount = db.MlmAccounts.FirstOrDefault(entry => entry.Login == userName);
                if (existingAccount != null)
                {
                    userId = existingAccount.Id;
                    existingAccount.Password = password;
                    existingAccount.Email = email;
                    existingAccount.Phone = phone;
                    if (existingAccount.ActiveUntill < DateTime.Now) existingAccount.ActiveUntill = DateTime.Now.AddDays(1);
                    db.Entry(existingAccount).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var newAccount = new MlmAccount
                    {
                        Id = userId = Guid.NewGuid(),
                        Login = userName,
                        Password = password,
                        Email = email,
                        Phone = phone,
                        MlmAccountType = MlmAccountType.CaribbeanBridge,
                        ActiveUntill = DateTime.Now.AddDays(1)
                    };
                    db.MlmAccounts.Add(newAccount);
                    db.SaveChanges();
                }
            }
            return Json(new { loginResult = true, userId });
        }

        public ActionResult PaymentSuccess()
        {
            return View();
        }

        [HttpPost]
        public void PaymentSuccessCallback(string notification_type, string operation_id, string datetime, string sha1_hash,
                 string sender, bool codepro, string currency, decimal amount, decimal withdraw_amount, string label)
        {
            EmailHelper.Instance.SendNewPayment(notification_type, operation_id, label, datetime, amount,
                withdraw_amount, sender, sha1_hash, currency, codepro);
            ////////////////////////////////////////////////////////////////////////
            string key = SettingsService.CaribsSecretYandexKey; // секретный код
            // проверяем хэш
            string paramString = String.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}&{7}&{8}",
                notification_type, operation_id, amount, currency, datetime, sender,
                codepro.ToString().ToLower(), key, label);

            string paramStringHash1 = GetHash(paramString);
            // создаем класс для сравнения строк
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            // если хэши идентичны, добавляем данные о заказе в бд
            if (0 == comparer.Compare(paramStringHash1, sha1_hash))
            {
                ////////////////////////////////////////////////////////////////////////
                var accountId = Guid.Parse(label);
                using (var db = new ApplicationDbContext())
                {
                    //extend activeUntill value
                    var currencyHelper = new CurrencyHelper();
                    var months = (int)Math.Round(withdraw_amount / (decimal)currencyHelper.CurrencyRate.RUB / 2); //2 usd
                    var mlmAccount = db.MlmAccounts.FirstOrDefault(entry => entry.Id == accountId);
                    if (mlmAccount == null)
                        EmailHelper.Instance.SendAutoClickFailed("faraon.ua@gmail.com", accountId.ToString(),
                            "No such account");
                    else
                    {
                        var activeUntill = mlmAccount.ActiveUntill > DateTime.Now
                            ? mlmAccount.ActiveUntill.AddMonths(months)
                            : DateTime.Now.AddMonths(months);
                        mlmAccount.ActiveUntill = activeUntill;
                        db.Entry(mlmAccount).State = EntityState.Modified;
                    }

                    //add to DB

                    var order = new Order
                    {
                        Id = Guid.NewGuid(),
                        OperationId = operation_id,
                        Date = DateTime.Now,
                        Amount = amount,
                        WithdrawAmount = withdraw_amount,
                        Sender = sender,
                        AccountId = accountId
                    };
                    db.Orders.Add(order);

                    db.SaveChanges();
                }
            }
        }

        public string GetHash(string val)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] data = sha.ComputeHash(Encoding.UTF8.GetBytes(val));
            var sBuilder = new StringBuilder();
            foreach (byte t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Caribs.Common.Helpers;
using Caribs.Domain.DbContext;
using Caribs.Services.Clients;
using NLog;

namespace Caribs.Services
{
    public class SchedulerService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task StartAutoclicker()
        {
            try
            {
                var db = new ApplicationDbContext();
                var accounts = db.MlmAccounts.ToList();
                var activeAccounts = accounts.Where(entry => entry.ActiveUntill > DateTime.Now);
                var unactiveAccounts = accounts.Where(entry => entry.ActiveUntill < DateTime.Now);
                var expiringAccounts = accounts.Where(entry => entry.ActiveUntill > DateTime.Now && entry.ActiveUntill < DateTime.Now.AddDays(3));
                //process active
                foreach (var account in activeAccounts)
                {
                    var client = new CaribsClient();
                    if (!await client.Login(account.Login, account.Password).ConfigureAwait(false))
                    {
                        if (account.Email != null)
                            EmailHelper.Instance.SendLoginFailed(account.Email, account.Login);
                        continue;
                    }
                    client.ProcessAccount(account);
                }

                //notify expiring
                foreach (var expAcc in expiringAccounts)
                {
                    EmailHelper.Instance.SendAutoClickExpiring(expAcc.Email, expAcc.Login, expAcc.ActiveUntill);
                }

                //remove unactive
                db.MlmAccounts.RemoveRange(unactiveAccounts);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
                EmailHelper.Instance.SendSchedulerFailed(ex.ToString());
            }
        }
    }
}

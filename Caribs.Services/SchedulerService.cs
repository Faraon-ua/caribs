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

                foreach (var account in accounts)
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
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
                EmailHelper.Instance.SendSchedulerFailed(ex.ToString());
            }
        }
    }
}

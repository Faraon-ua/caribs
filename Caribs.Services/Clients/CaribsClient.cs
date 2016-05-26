using System;
using System.Collections;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using Caribs.Common.Helpers;
using Caribs.Common.Services;
using Caribs.Domain.DbContext;
using Caribs.Domain.Models;
using HtmlAgilityPack;
using TimeZone = Caribs.Common.Helpers.TimeZone;

namespace Caribs.Services.Clients
{
    public class CaribsClient
    {
        #region Helper Methods

        private CookieContainer cookieContainer = new CookieContainer();

        private CookieCollection GetAllCookies(CookieContainer cookieJar)
        {
            CookieCollection cookieCollection = new CookieCollection();
            Hashtable table = (Hashtable)cookieJar.GetType().InvokeMember("m_domainTable",
                                                                            BindingFlags.NonPublic |
                                                                            BindingFlags.GetField |
                                                                            BindingFlags.Instance,
                                                                            null,
                                                                            cookieJar,
                                                                            new object[] { });

            foreach (var tableKey in table.Keys)
            {
                String str_tableKey = (string)tableKey;

                if (str_tableKey[0] == '.')
                {
                    str_tableKey = str_tableKey.Substring(1);
                }

                SortedList list = (SortedList)table[tableKey].GetType().InvokeMember("m_list",
                                                                            BindingFlags.NonPublic |
                                                                            BindingFlags.GetField |
                                                                            BindingFlags.Instance,
                                                                            null,
                                                                            table[tableKey],
                                                                            new object[] { });

                foreach (var listKey in list.Keys)
                {
                    String url = "https://" + str_tableKey + (string)listKey;
                    cookieCollection.Add(cookieJar.GetCookies(new Uri(url)));
                }
            }

            return cookieCollection;
        }
        private async Task<HttpResponseMessage> GetServiceResponse(string url, HttpVerbs httpMethod, CookieContainer authorizedCookieContainer = null, string content = null)
        {
            using (
                var handler = new HttpClientHandler
                {
                    CookieContainer = authorizedCookieContainer ?? cookieContainer,
                    UseCookies = true,
                    UseDefaultCredentials = true
                })
            {
                using (var client = new HttpClient(handler as HttpMessageHandler))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-En"));
                    HttpResponseMessage response = null;
                    switch (httpMethod)
                    {
                        case HttpVerbs.Get:
                            response = await client.GetAsync(url).ConfigureAwait(false);
                            break;
                        case HttpVerbs.Post:
                            var callContent = new StringContent(content);
                            callContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                            response = await client.PostAsync(url, callContent).ConfigureAwait(false);
                            break;
                        case HttpVerbs.Delete:
                            response = await client.DeleteAsync(url).ConfigureAwait(false);
                            break;
                    }
                    return response;
                }
            }
        }

        #endregion

        public async Task<bool> Login(string userName, string password)
        {
            var result = await GetServiceResponse(SettingsService.CaribsLoginUrl, HttpVerbs.Post, null,
               string.Format(SettingsService.CaribsLoginBodyTemplate, userName, password)).ConfigureAwait(false);
            var resultPage = await result.Content.ReadAsStringAsync();
            return resultPage.Contains(userName + " - id");
        }

        public async Task ProcessAccount(MlmAccount account)
        {
            var result = await GetServiceResponse(SettingsService.CaribsTransactionHistoryUrl, HttpVerbs.Get).ConfigureAwait(false);
            var resultPage = await result.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(resultPage);
            var caribeanNow = DateTime.Now.ToLocalizedDateTime(TimeZone.Caribs);
            var dateToSearch = string.Format(SettingsService.CaribsTransactionDateFormatText, caribeanNow.Year,
                caribeanNow.Month.ToString("00"), caribeanNow.Day.ToString("00"));
            var spansWithAwardDates = doc.DocumentNode.SelectNodes("//table[@class='table table-history']/tbody/tr/td[last()]/span").ToList();
            //search for click award
            var todayTrs =
                spansWithAwardDates.Where(entry => entry.InnerText.Contains(dateToSearch))
                    .Select(entry => entry.ParentNode.ParentNode)
                    .ToList();
            var todayTrClickBonus =
                todayTrs.FirstOrDefault(entry => entry.SelectSingleNode("./td[6]")
                    .InnerText.Contains(SettingsService.CaribsTransactionAdvertisingBonusText));
            //if not found and it's next day click
            if (todayTrClickBonus == null)
            {
                if (await GetSocialBonus())
                {
                    //reload transaction page
                    result = await GetServiceResponse(SettingsService.CaribsTransactionHistoryUrl, HttpVerbs.Get).ConfigureAwait(false);
                    resultPage = await result.Content.ReadAsStringAsync();
                    doc.LoadHtml(resultPage);
                    spansWithAwardDates = doc.DocumentNode.SelectNodes("//table[@class='table table-history']/tbody/tr/td[last()]/span").ToList();
                }
                else
                {
                    EmailHelper.Instance.SendAutoClickFailed(account.Email, account.Login);
                }
            }
            //select new transactions and send to email
            if (spansWithAwardDates.Any())
            {
                var spanValues = spansWithAwardDates.Select(entry => entry.InnerText).ToList();
                var lastAwardedIndex = spanValues.FindIndex(entry => entry == account.LastAwardedOn);
                //take all awards
                var newAwards = spansWithAwardDates;
                //if new from last awarded cut to new only
                if (lastAwardedIndex > 0)
                {
                    newAwards = newAwards.Take(lastAwardedIndex).ToList();
                }
                //if db last awarded != today
                if (lastAwardedIndex != 0)
                {
                    //update lastAwarded to latest
                    using (var db = new ApplicationDbContext())
                    {
                        account.LastAwardedOn = spansWithAwardDates.First().InnerText;
                        db.MlmAccounts.Attach(account);
                        db.Entry(account).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }

                    var resultHtml =
                        string.Concat(
                            newAwards.Select(entry => entry.ParentNode.ParentNode).Select(entry => entry.OuterHtml));
                    EmailHelper.Instance.SendNewAwards(resultHtml, account.Email, account.Login);
                }
            }
        }

        public async Task<bool> GetSocialBonus()
        {
            var result = await GetServiceResponse(SettingsService.CaribsSocialBonusUrl, HttpVerbs.Get).ConfigureAwait(false);
            var resultContent = await result.Content.ReadAsStringAsync();
            return resultContent == "";
        }
    }
}

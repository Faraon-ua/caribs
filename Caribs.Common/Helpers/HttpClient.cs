using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Caribs.Common.Helpers
{
    public class MlmHttpClient
    {
        #region Helper Methods

        private CookieContainer cookieContainer = new CookieContainer();

        public CookieCollection GetAllCookies(CookieContainer cookieJar)
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
        private async Task<HttpResponseMessage> GetServiceResponse(string url, HttpVerbs httpMethod, CookieContainer authorizedCookieContainer = null, HttpContent content = null)
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
                    HttpResponseMessage response = null;
                    switch (httpMethod)
                    {
                        case HttpVerbs.Get:
                            response = await client.GetAsync(url).ConfigureAwait(false);
                            break;
                        case HttpVerbs.Post:
                            var callContent = new StringContent(await content.ReadAsStringAsync());
                            callContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            response = await client.PostAsync(url, callContent).ConfigureAwait(false);
                            break;
                        case HttpVerbs.Delete:
                            response = await client.DeleteAsync(url).ConfigureAwait(false);
                            break;
                    }
                    if (response.StatusCode != HttpStatusCode.Unauthorized)
                        return response;
                    //if Unauthorized - authanticate device and set cookies
                    var deviceAuthCookie = new CookieContainer();// await AuthenticateDevice(httpMethod, superKeyRequired);
                    if (deviceAuthCookie == null)
                        throw new Exception("Can not authenticate with credentials set in web.config");
                    return await GetServiceResponse(url, httpMethod, deviceAuthCookie, content);
                }
            }
        }

        #endregion

        public async Task Login(string loginUrl, string userName, string password)
        {
            var result = await GetServiceResponse(loginUrl, HttpVerbs.Post, null,
                new StringContent(string.Format("LoginForm%5Blogin%5D={0}&LoginForm%5Bpassword%5D={1}&login-button=", userName, password)));
        }
//        private static async Task<T> GetObjectFromService<T>(string url, bool superKeyRequired = false)
//        {
//            var response = await GetServiceResponse(url, HttpVerbs.Get, superKeyRequired: superKeyRequired).ConfigureAwait(false);
//            var json = await response.Content.ReadAsStringAsync();
//            T obj = default(T);
//            using (JsonReader reader = new JsonTextReader(new StringReader(json)))
//            {
//                try
//                {
//                    obj = JsonSerializer.Create().Deserialize<T>(reader);
//                }
//                catch (Exception ex)
//                {
//                    Debug.WriteLine(ex.Message);
//                }
//            }
//            return obj;
//        }

        /*   public static async Task<CookieContainer> AuthenticateDevice(HttpVerbs httpMethod, bool superKeyRequired = false)
           {
               var authpath = httpMethod == HttpVerbs.Post
                   ? Configuration.NetTv4Api.IngestAuthPath
                   : superKeyRequired
                       ? Configuration.NetTv4Api.IngestAuthPath
                       : Configuration.NetTv4Api.DeviceAuthPath;
               cookieContainer = new CookieContainer();
               var deviceAuthResponce = await GetServiceResponse(authpath, HttpVerbs.Get);
               //if devise authorized - 
               if (deviceAuthResponce.StatusCode == HttpStatusCode.OK)
               {
                   var cookies = GetAllCookies(cookieContainer).Cast<Cookie>().ToList();
                   var authCookieContainer = new CookieContainer();
                   var deviceAuthCookie = cookies.First(entry => entry.Name == "DeviceAuthToken");

                   deviceAuthCookie.Path = String.Empty;

                   authCookieContainer.Add(new Uri(NetTv4ServiceApi.Instance.Domain), deviceAuthCookie);
                   return authCookieContainer;
               }
               return null;
           }*/
    }

}

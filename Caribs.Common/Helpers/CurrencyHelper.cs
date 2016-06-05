using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Xml.Linq;

namespace Caribs.Common.Helpers
{
    public class CurrencyRate
    {
        public double UAH { get; set; }
        public double USD { get; set; }
        public double RUB { get; set; }
    }

    public class CurrencyHelper
    {
        private const string ApiRoute = "https://api.privatbank.ua/p24api/pubinfo?exchange&coursid=5";
        private const string CurrencyRateKey = "CurrencyRate";

        public CurrencyRate CurrencyRate
        {
            get
            {
                var cache = HttpContext.Current.Cache;
                if (cache[CurrencyRateKey] == null)
                {
                    var xmlDoc = XDocument.Load(ApiRoute, LoadOptions.None);
                    var exchangerates = xmlDoc.Descendants("exchangerate");
                    var currencyRate = new CurrencyRate();
                    currencyRate.UAH =
                        double.Parse(
                            exchangerates.FirstOrDefault(entry => entry.Attribute("ccy").Value == "USD")
                                .Attribute("sale")
                                .Value, CultureInfo.InvariantCulture);
                    currencyRate.USD = 1;
                    currencyRate.RUB = currencyRate.UAH/
                                       double.Parse(
                                           exchangerates.FirstOrDefault(entry => entry.Attribute("ccy").Value == "RUR")
                                               .Attribute("buy")
                                               .Value, CultureInfo.InvariantCulture);
                    cache.Insert(CurrencyRateKey, currencyRate, null, Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes(600));
                }
                return cache[CurrencyRateKey] as CurrencyRate;
            }
        }
    }
}
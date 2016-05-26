using System.Web.Mvc;
using Caribs.Filters;

namespace Caribs
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new GlobalErrorHandler());
        }
    }
}

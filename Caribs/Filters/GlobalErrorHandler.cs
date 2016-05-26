using System.Web.Mvc;
using NLog;

namespace Caribs.Filters
{
    public class GlobalErrorHandler : HandleErrorAttribute
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public override void OnException(ExceptionContext filterContext)
        {
            var ex = filterContext.Exception;
            filterContext.ExceptionHandled = true;
            _logger.Fatal(ex);
        }
    }
}

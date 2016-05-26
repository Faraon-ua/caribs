using System.Threading.Tasks;
using System.Web.Mvc;
using Caribs.Services;

namespace Caribs.Controllers
{
    public class ScheduleController : Controller
    {
        //
        // GET: /Schedule/
        public async Task<ActionResult> Index()
        {
            var schedulerService = new SchedulerService();
            schedulerService.StartAutoclicker();
            return Content("Scheduler started");
        }
    }
}
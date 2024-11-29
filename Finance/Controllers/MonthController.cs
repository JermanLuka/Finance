using Microsoft.AspNetCore.Mvc;

namespace Finance.Controllers
{
    public class MonthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

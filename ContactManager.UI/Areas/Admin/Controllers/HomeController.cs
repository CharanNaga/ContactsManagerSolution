using Microsoft.AspNetCore.Mvc;

namespace ContactManager.UI.Areas.AdminArea.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

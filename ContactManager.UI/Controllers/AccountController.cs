using ContactManager.Core.DTO;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.UI.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterDTO registerDTO)
        {
            //Store User Registration Details into Identity DB
            return RedirectToAction(nameof(PersonsController.Index),"Persons");
        }
    }
}

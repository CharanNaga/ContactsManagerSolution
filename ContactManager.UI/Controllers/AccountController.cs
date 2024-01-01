using ContactManager.Core.Domain.IdentityEntities;
using ContactManager.Core.DTO;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.UI.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            //Store User Registration Details into Identity DB
            if(!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x=>x.ErrorMessage).ToList();
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                PersonName = registerDTO.PersonName,
                PhoneNumber = registerDTO.Phone
            };
            IdentityResult result = await _userManager.CreateAsync(user,registerDTO.Password);
            if(result.Succeeded)
            {
                //sign-in
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }
            else
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("Register",error.Description);
                }
                return View(registerDTO);
            }
        }
    }
}

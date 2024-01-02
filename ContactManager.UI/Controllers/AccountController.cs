using ContactManager.Core.Domain.IdentityEntities;
using ContactManager.Core.DTO;
using ContactManager.Core.Enums;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.UI.Controllers
{
    //[Route("[controller]/[action]")] //Uncomment this if we don't choose for conventional routing
    [AllowAnonymous] //all the action methods if this controller are able to open even without login
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return View(registerDTO);
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                PersonName = registerDTO.PersonName,
                PhoneNumber = registerDTO.Phone
            };
            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                if(registerDTO.UserType == UserTypeOptions.Admin)
                {
                    //Create 'Admin' role
                    string adminUserType = UserTypeOptions.Admin.ToString();
                    var adminUser = await _roleManager.FindByNameAsync(adminUserType);
                    if(adminUser != null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole()
                        {
                            Name = adminUserType
                        };
                        await _roleManager.CreateAsync(applicationRole); //inserts new row in AspNetRoles table
                    }
                    //Add newly created user into 'Admin' role
                    await _userManager.AddToRoleAsync(user,adminUserType); //inserts new row in AspNetUserRoles table
                }
                else
                {
                    string userType = UserTypeOptions.User.ToString();
                    //Add newly created user into 'User' role
                    await _userManager.AddToRoleAsync(user, userType); //inserts new row in AspNetUserRoles table
                }
                //sign-in
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
                return View(registerDTO);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO,string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return View(loginDTO);
            }
            var result = await _signInManager.PasswordSignInAsync(
                loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false
                ); //This method communicates with Db and check if there is atleast one row matching with the entered credentials are present in Db.
            //If present, then it adds an identity cookie to the request indicating it is authenticated

            if (result.Succeeded)
            {
                if(!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }
            ModelState.AddModelError("Login", "Invalid Email or Password");
            return View(loginDTO);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); //removing identity cookie by calling SignOutAsync()
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);
            if(result != null)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }
    }
}

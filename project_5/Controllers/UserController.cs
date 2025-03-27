using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project_5.Models;
using System.Threading.Tasks;

namespace project_5.Controllers
{
    public class UserController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Show login page
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // Process login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        // Process logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "User");
        }

        // Show registration page
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // Process registration
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign the 'User' role by default
                var userRoleExist = await _roleManager.RoleExistsAsync("User");
                if (!userRoleExist)
                {
                    var userRole = new IdentityRole("User");
                    await _roleManager.CreateAsync(userRole);
                }

                if (user.Email == "jacques@example.com")
                {
                    var adminRoleExist = await _roleManager.RoleExistsAsync("Admin");
                    if (!adminRoleExist)
                    {
                        var adminRole = new IdentityRole("Admin");
                        await _roleManager.CreateAsync(adminRole);
                    }
                    await _userManager.AddToRoleAsync(user, "Admin");
                }

                await _userManager.AddToRoleAsync(user, "User");

                // Sign in the user after successful registration
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}

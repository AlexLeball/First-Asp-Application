using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project_5.Models;
using System.Security.Claims;
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
                return RedirectToAction("Cars", "CarList");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        // Process logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Cars", "CarList");
        }

        // Show registration page
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.Username,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Add claims and roles after successful registration
                var displayName = model.Username;
                await _userManager.AddClaimAsync(user, new Claim("DisplayName", displayName));

                // Assign default roles (e.g., "User")
                var userRoleExist = await _roleManager.RoleExistsAsync("User");
                if (!userRoleExist)
                {
                    var userRole = new IdentityRole("User");
                    await _roleManager.CreateAsync(userRole);
                }

                await _userManager.AddToRoleAsync(user, "User");

                // If it's a specific user (e.g., admin), assign the admin role
                if (user.Email == "admin@example.com")
                {
                    var adminRoleExist = await _roleManager.RoleExistsAsync("Admin");
                    if (!adminRoleExist)
                    {
                        var adminRole = new IdentityRole("Admin");
                        await _roleManager.CreateAsync(adminRole);
                    }
                    await _userManager.AddToRoleAsync(user, "Admin");
                }

                // Sign in the user after successful registration
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Cars", "CarList");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Handle unauthenticated user
            }

            var model = new ProfileViewModel
            {
                Username = user.UserName,
                Email = user.Email,
            };

            return View(model);
        }

        // Process profile update
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Validate uniqueness before updating username or email
            if (user.UserName != model.Username)
            {
                var existingUser = await _userManager.FindByNameAsync(model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "This username is already taken.");
                    return View(model);
                }
                user.UserName = model.Username;
            }

            if (user.Email != model.Email)
            {
                var existingEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "This email is already in use.");
                    return View(model);
                }
                user.Email = model.Email;

                // Optionally, require email confirmation again
                user.EmailConfirmed = false;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

    }
}

using Microsoft.AspNetCore.Identity; 
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
             UserManager<AppUser> userManager,
             SignInManager<AppUser> signInManager,
             RoleManager<IdentityRole> roleManager) // <--- Add this parameter
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager; // <--- Assign it
        }

        // GET: Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // Create the user object
                AppUser user = new()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName 
                    
                };

                // Save to database (password is hashed automatically)
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // 1. Check if the role exists
                    if (!await _roleManager.RoleExistsAsync("AuthenticatedUser"))
                    {
                        // 2. Create it if it doesn't
                        await _roleManager.CreateAsync(new IdentityRole("AuthenticatedUser"));
                    }

                    // 3. Now it is safe to assign
                    await _userManager.AddToRoleAsync(user, "AuthenticatedUser");
                    // Assign the default role "User"
                    // Sign them in immediately
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // If errors (e.g., password too weak), show them
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }

        // GET: Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
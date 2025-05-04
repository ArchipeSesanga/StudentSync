using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentSync.Data;
using StudentSync.Models;

namespace StudentSync.Controllers;

public class AdminController : Controller
{
    
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    
    public AdminController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign admin role
                await _userManager.AddToRoleAsync(user, "Admin");

                // Sign in the admin
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Dashboard");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View(model);
    }//End register()
    
    // GET: Admin/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: Admin/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // ✅ Check if the user is in the Admin role
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        if (!isAdmin)
        {
            ModelState.AddModelError("", "Access denied. Only admins can log in here.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
            return RedirectToAction("Dashboard", "Admin");
        }

        ModelState.AddModelError("", "Invalid login attempt.");
        return View(model);
    }


    public IActionResult Dashboard()
    {
        return View();
    }
}

internal interface IAdmin
{
    Admin Details(string id);
    Admin Create(Admin consumer);
    Admin Edit(Admin consumer);
    bool Delete(Admin consumer);
    bool IsExist(string id);
   
}
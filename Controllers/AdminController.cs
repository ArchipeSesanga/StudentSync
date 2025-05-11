using Microsoft.AspNetCore.Authorization;
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
    // [Authorize(Roles = "Admin")]
    public IActionResult Login()
    {
        
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Dashboard", "Admin");
                
        }
            
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


    [Authorize(Roles = "Admin")]
    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]

    public IActionResult ForgotPassword()

    {

        return View();

    }



    [HttpPost]

    [ValidateAntiForgeryToken]

    public async Task<IActionResult> ForgotPassword(string email)

    {

        if (string.IsNullOrEmpty(email))

        {

            ModelState.AddModelError("", "Email is required.");

            return View();

        }



        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || !(await _userManager.IsInRoleAsync(user, "Admin")))

        {

            // Do not reveal that the user does not exist or is not an Admin 

            return RedirectToAction("ForgotPasswordConfirmation");

        }



        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var callbackUrl = Url.Action("ResetPassword", "Admin", new { token, email = user.Email }, Request.Scheme);



        // For now, just show it on screen (in real app, email it) 

        ViewBag.Link = callbackUrl;



        return View("DisplayResetLink"); // Create this view to display the reset link 

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
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using StudentSync.Data;
using StudentSync.Interfaces;
using StudentSync.Models;

namespace StudentSync.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly StudentDBContext _context;
    private readonly IDBInitializer _seedDatabase;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ILogger<HomeController> logger, StudentDBContext context, IDBInitializer seedDatabase, UserManager<IdentityUser> userManager) 
    {
        _logger = logger;
        _context = context;
        _seedDatabase = seedDatabase;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        ViewData ["UserID"] = _userManager.GetUserId(this.User);
        ViewData["UserName"] = _userManager.GetUserName(this.User);

        if (this.User.IsInRole("Admin"))
        {
            ViewData["UserRole"] = "Admin";
        }

        if (this.User.IsInRole("Student"))
        {
            ViewData["UserRole"] = "Student";
        }

        if (this.User.IsInRole("Consumer"))
        {
            ViewData["UserRole"] = "Consumer";
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
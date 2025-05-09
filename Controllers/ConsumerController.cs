using Microsoft.AspNetCore.Identity;
using StudentSync.Interfaces;
using StudentSync.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using StudentSync.Repositories;
using Microsoft.AspNetCore.Hosting;

namespace StudentSync.Controllers
{
    public class ConsumerController : Controller
    {
        private readonly IConsumer _consumerRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ConsumerController(IConsumer consumerRepo, UserManager<IdentityUser> userManager,
             SignInManager<IdentityUser> signInManager, IHttpContextAccessor httpContextAccessor,
             IWebHostEnvironment webHostEnvironment)
        {

            _consumerRepo = consumerRepo;
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;


        }


        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            pageNumber ??= 1;
            int pageSize = 3;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["StudentNumberSortParm"] = string.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var consumers = _consumerRepo.GetConsumer(searchString, sortOrder);

            if (consumers == null || !consumers.Any())
            {
                ViewBag.Message = "No consumer records found.";
                return View();
            }

            var paginatedList = PaginatedList<Consumer>.Create(consumers.AsNoTracking(), pageNumber.Value, pageSize);
            return View(paginatedList);
        }//End Method
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Consumer");
                
            }
            
            return View();
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Attempt login
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Redirect to Consumer's Dashboard
                return RedirectToAction("Dashboard", "Consumer");
            }

            // If login fails
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }


        [HttpGet]
        public IActionResult Register()
        { //Handles registration view
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            //Handles registration action from the user
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email is already in use.");
                    return View(model);
                }

                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
        
                if (result.Succeeded)
                {
                    // Automatically assign "Consumer" role
                    await _userManager.AddToRoleAsync(user, "Consumer");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Dashboard", "Consumer");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                

                return View(model);
            }


            return View(model);
        } //End REgister()
      
        public IActionResult Details(string id)
        {
            var consumer = _consumerRepo.Details(id);
            if (consumer == null)
                return NotFound("Consumer detail not found");
            return View(consumer);
        }

        [HttpGet]
        [Authorize(Roles ="Consumer")]
        public IActionResult Create()
        {
            Consumer consumer = new Consumer();
            string fileName = "Default.PNG";
            consumer.Photo = fileName;
            return View(consumer);
        }


        [HttpPost]
        [Authorize(Roles = "Consumer")]
        public IActionResult Create(Consumer consumer)
        {
            var files = HttpContext.Request.Form.Files;
            string webRootPath = _webHostEnvironment.WebRootPath;
            string upload = webRootPath + WebConstants.ImagePath;
            string fileName = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(files[0].FileName);
            using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension),
            FileMode.Create))
            {
                files[0].CopyTo(fileStream);
            }
            consumer.Photo = fileName + extension;
            try
            {
                if (ModelState.IsValid)
                {
                    _consumerRepo.Create(consumer);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Consumer record not saved.");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Comsumer")]
        public IActionResult Edit(string id)
        {
            var consumer = _consumerRepo.Details(id);
            if (consumer == null)
                return NotFound("Consumer detail not found");

            return View(consumer);
        }

       [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Consumer")]
        public IActionResult Edit(Consumer consumer)
        {
            if (ModelState.IsValid)
            {
                _consumerRepo.Edit(consumer);
                return RedirectToAction(nameof(Index));
            }

            return View(consumer);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            ViewResult viewDetail = View();
            try
            {
                viewDetail = View(_consumerRepo.Details(id));
            }
            catch (Exception ex)
            {
                throw new Exception("Consumer detail not found");
            }
            return viewDetail;
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(string id)
        {
            try
            {
                _consumerRepo.Delete(id);
            }
            catch (Exception ex)
            {
                // You can log the actual error message here for debugging
                throw new Exception("Consumer could not be deleted");
            }

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult OurStore()
        {
            throw new NotImplementedException();
        }
    }

   
}

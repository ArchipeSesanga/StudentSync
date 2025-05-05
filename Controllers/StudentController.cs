using Microsoft.AspNetCore.Identity;
using StudentSync.Interfaces;
using StudentSync.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace StudentSync.Controllers
{
    [TypeFilter(typeof(CustomExceptionFilter))]
    public class StudentController : Controller
    {
        private readonly IStudent _studentRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public StudentController(IStudent studentRepo, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            
                _studentRepo = studentRepo;
                _userManager = userManager;
                _signInManager = signInManager;
           

        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            pageNumber = pageNumber ?? 1;
            int pageSize = 3;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["StudentNumberSortParm"] = String.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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

            ViewResult viewResult;

            try
            {
                viewResult = View(PaginatedList<Student>.Create(_studentRepo.GetStudents(searchString, sortOrder).AsNoTracking(), pageNumber ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                throw new Exception("No student records detected");
            }

            return viewResult;
        }
        


        public IActionResult Details(string id)
        {
            ViewResult viewDetail = View();
            try
            {
                viewDetail = View(_studentRepo.Details(id));
            }
            catch (Exception ex)
            {
                throw new Exception("Student detail not found");
            }


            return viewDetail;
        }
        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("StudentNumber, FirstName, Surname, EnrollmentDate")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _studentRepo.Create(student);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Student could not be created");
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            ViewResult viewDetail = View();
            try
            {
                viewDetail = View(_studentRepo.Details(id));
            }
            catch (Exception ex)
            {
                throw new Exception("Student detail not found");
            }
            return viewDetail;
        }
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _studentRepo.Edit(student);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Student detail could not be edited");
            }

            return RedirectToAction(nameof(Index));
        }//End Method
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            ViewResult viewDetail = View();
            try
            {
                var student = _studentRepo.Details(id);
                if (student == null)
                {
                    return NotFound();
                }

                viewDetail = View(student);
            }
            catch (Exception)
            {
                throw new Exception("Student detail not found");
            }
            return viewDetail;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            try
            {
                _studentRepo.Delete(id);
                TempData["SuccessMessage"] = "Student successfully deleted.";
            }
            catch (Exception)
            {
                throw new Exception("Student could not be deleted.");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register(Student student)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // ✅ Check if user is in the Student role
            if (!await _userManager.IsInRoleAsync(user, "Student"))
            {
                ModelState.AddModelError(string.Empty, "Access denied. Only students can log in here.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Dashboard", "Student");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }



        public IActionResult Dashboard()
        {
           // throw new NotImplementedException();
           return View();
        }
    }
}

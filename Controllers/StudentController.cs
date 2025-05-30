﻿using Microsoft.AspNetCore.Identity;
using StudentSync.Interfaces;
using StudentSync.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;

namespace StudentSync.Controllers
{
    [TypeFilter(typeof(CustomExceptionFilter))]
    public class StudentController : Controller
    {
        private readonly IStudent _studentRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentController(IStudent studentRepo, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment)
        {

            _studentRepo = studentRepo;
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;


        }

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
                viewResult = View(PaginatedList<Student>.Create(
                    _studentRepo.GetStudents(searchString, sortOrder).AsNoTracking(), pageNumber ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                throw new Exception("No student records detected");
            }

            return viewResult;
        }

        public IActionResult Details(string id)
        {
            ViewResult viewDetail;
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

        [HttpGet]
        [Authorize(Roles = "Student, Consumer")]
        public IActionResult Create()
        {
            Student student = new Student();
            string fileName = "default.PNG";
            student.Photo = fileName;
            return View(student);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student, Consumer")]
        public IActionResult Create(Student student)
        {
            var files = HttpContext.Request.Form.Files;
            string webRootPath = _webHostEnvironment.WebRootPath;
            string? upload = Path.Combine(_webHostEnvironment.WebRootPath, WebConstants.ImagePath);
            string fileName = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(files[0].FileName);

            using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension),
            FileMode.Create))
            {
                files[0].CopyTo(fileStream);
            }

            student.Photo = fileName + extension;

            try
            {
                if (ModelState.IsValid)
                {
                    _studentRepo.Create(student);

                }

            }
            catch (Exception ex)
            {
                if(ex.InnerException!= null && ex.InnerException.Message.Contains("SQLite Error 19") )
                    throw new Exception("This Student already exists please enter a different one.");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            try
            {
                var student = _studentRepo.Details(id);
                if (student == null)
                {
                    return NotFound("Student not found.");
                }
                return View(student);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving the student details.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student, string photoName)
        {
            if (student == null)
            {
                return BadRequest("Invalid student data.");
            }

            try
            {
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var files = HttpContext.Request.Form.Files;
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string upload = Path.Combine(webRootPath, WebConstants.ImagePath);
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    // Delete the old file if it exists
                    if (!string.IsNullOrEmpty(photoName))
                    {
                        var oldFile = Path.Combine(upload, photoName);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                    }

                    // Save the new file
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    student.Photo = fileName + extension;
                }
                else
                {
                    student.Photo = photoName;
                }

                if (ModelState.IsValid)
                {
                    _studentRepo.Edit(student);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(student);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the student record.");
            }
        }


        [HttpGet]
        [Authorize(Roles ="Admin")]
        public IActionResult Delete(string id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public IActionResult Delete([Bind("StudentNumber, FirstName, Surname, EnrollmentDate")] Student student)
        {
            try
            {
                _studentRepo.Delete(student);
            }
            catch (Exception ex)
            {
                throw new Exception("Student could not be deleted");
            }

            return RedirectToAction(nameof(Index));
        }
        //end mothod


        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Student");
                
            }
            
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

            var result =
                await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Dashboard", "Student");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        // [Authorize(Roles = "Student")]
        public IActionResult Register()
        {
            return View();
        }

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
                    // Automatically assign "Student" role
                    await _userManager.AddToRoleAsync(user, "Student");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Dashboard", "Student");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            return View(model);
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }

        public IActionResult Courses()
        {
            throw new NotImplementedException();
        }

        public IActionResult Elements()
        {
            throw new NotImplementedException();
        }

        public IActionResult CourseDetails()
        {
            throw new NotImplementedException();
        }

        public IActionResult Enroll()
        {
            throw new NotImplementedException();
        }
    }
}

using StudentSync.Interfaces;
using StudentSync.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StudentSync.Controllers
{
    public class ConsumerController : Controller
    {
        private readonly IConsumer _consumerRepo;

        public ConsumerController(IConsumer consumerRepo)
        {
            _consumerRepo = consumerRepo ?? throw new ArgumentNullException(nameof(consumerRepo), "Constructor not initialized - IConsumer consumerRepo");
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
        }

        public IActionResult Details(string id)
        {
            var consumer = _consumerRepo.Details(id);
            if (consumer == null)
                return NotFound("Consumer detail not found");
            return View(consumer);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ConsumerID, FirstName, Surname, EnrollmentDate, Email")] Consumer consumer)
        {
            if (ModelState.IsValid)
            {
                _consumerRepo.Create(consumer);
                return RedirectToAction(nameof(Index));
            }

            return View(consumer);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var consumer = _consumerRepo.Details(id);
            if (consumer == null)
                return NotFound("Consumer detail not found");

            return View(consumer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public IActionResult Delete(string id)
        {
            var consumer = _consumerRepo.Details(id);
            if (consumer == null)
                return NotFound("Consumer detail not found");

            return View(consumer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([Bind("ConsumerID, FirstName, Surname, EnrollmentDate, Email")] Consumer consumer)
        {
            _consumerRepo.Delete(consumer);
            return RedirectToAction(nameof(Index));
        }
    }
}

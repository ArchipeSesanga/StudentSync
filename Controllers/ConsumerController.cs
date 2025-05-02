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
            try
            {
                _consumerRepo = consumerRepo;
            }
            catch (Exception ex)
            {
                throw new Exception("Constructor not initialized - IConsumer consumerRepo");
            }

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

            ViewResult viewResult = View();

            try
            {
                viewResult = View(PaginatedList<Consumer>.Create(_consumerRepo.GetConsumer(searchString, sortOrder).AsNoTracking(), pageNumber ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                throw new Exception("No consumer records detected");
            }

            return viewResult;
        }
        public IActionResult Details(string id)
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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ConsumerID, FirstName, Surname, EnrollmentDate, Email")] Consumer consumer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _consumerRepo.Create(consumer);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Consumer could not be created");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(string id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Consumer consumer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _consumerRepo.Edit(consumer);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Consumer detail could not be edited");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            ViewResult viewDetail = View();
            try
            {
                viewDetail = View(_consumerRepo.Details(id));
            }
            catch (Exception ex)
            {
                throw new Exception("consumer detail not found");
            }
            return viewDetail;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([Bind("ConsumerID, FirstName, Surname, EnrollmentDate, Email")] Consumer consumer)
        {
            try
            {
                _consumerRepo.Delete(consumer);
            }
            catch (Exception ex)
            {
                throw new Exception("Student could not be deleted");
            }

            return RedirectToAction(nameof(Index));
        }

        private class PaginatedList<T>
        {
            internal static string? Create(IQueryable<Consumer> consumers, int v, int pageSize)
            {
                throw new NotImplementedException();
            }
        }
    }

    internal class CustomExceptionFilter
    {
    }
}


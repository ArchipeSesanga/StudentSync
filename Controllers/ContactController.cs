namespace StudentSync.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models; 

public class ContactController : Controller
{
    [HttpGet]
    public IActionResult Contact()
    {
        return View(new ContactViewModel());
    }

    [HttpPost]
    public IActionResult SendMessage(ContactViewModel model)
    {
        if (ModelState.IsValid)
        {
            // You can log, save to database, or send email here
            TempData["SuccessMessage"] = "Your message has been sent successfully!";

            // Redirect to avoid duplicate submissions on refresh
            return RedirectToAction("MessageSent");
        }

        // If form is invalid, return the same view to show validation messages
        return View("Contact", model);
    }

   
    public IActionResult MessageSent()
    {
        return View();
    }
}

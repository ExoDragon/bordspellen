using BordSpellen.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BordSpellen.Controllers;

public class PersonController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
    [AllowAnonymous]
    public IActionResult Details(int id)
    {
        return View();
    }

    public IActionResult Update(int id)
    {
        return View();
    }

    [HttpPut]
    public IActionResult Update(int id, PersonViewModel viewModel)
    {
        return RedirectToAction("Details", id);
    }
}
using BordSpellen.Models;
using Core.Domain.Data.Entities;
using Core.Domain.Security.Roles;
using Core.Repositories.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BordSpellen.Controllers;

[Authorize(Roles = UserRoles.Organiser)]
public class DietController : Controller
{
    private readonly IDietRepository _dietRepository;

    public DietController(IDietRepository dietRepository)
    {
        _dietRepository = dietRepository;
    }
    
    // GET
    public async Task<IActionResult> Index()
    {
        var diets = await _dietRepository.GetAll();
        return View(diets);
    }
    public async Task<IActionResult> Details(int id)
    {
        var diet = await _dietRepository.GetById(id);
        if (diet == null || diet.Id != id || String.IsNullOrEmpty(id.ToString())) return View("NotFound");
        return View(diet);
    }
    public IActionResult Create()
    {
        return View(new DietViewModel());
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DietViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var newDiet = new Diet
        {
            Name = viewModel.Name,
            Description = viewModel.Description
        };

        await _dietRepository.Create(newDiet);
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id)
    {
        var dietResponse = await _dietRepository.GetById(id);
        if (dietResponse == null && id != dietResponse?.Id) return View("NotFound");
        
        var viewModel = new DietViewModel
        {
            Id = dietResponse.Id,
            Name = dietResponse.Name,
            Description = dietResponse.Description
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DietViewModel viewModel)
    {
        if (id != viewModel.Id) return View("NotFound");
        
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var newDiet = new Diet
        {
            Id = viewModel.Id,
            Name = viewModel.Name,
            Description = viewModel.Description
        };
        
        await _dietRepository.Update(newDiet);
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id)
    {
        var diet = await _dietRepository.GetById(id);
        if (diet == null) return View("NotFound");

        await _dietRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}
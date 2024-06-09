using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BordSpellen.Models;
using Core.Repositories.Data.Entities;

namespace BordSpellen.Controllers;

public class HomeController : Controller
{
    private readonly IGameEventRepository _gameEventRepository;

    public HomeController(
        IGameEventRepository gameEventRepository)
    {
        _gameEventRepository = gameEventRepository;
    }
    
    public async Task<IActionResult> Index()
    {
        var gameEvents = await _gameEventRepository.GetAll(x => x.GamerGameEvents!);
        var gameEvent = gameEvents.Where(x => x.EventDate > DateTime.Now)
            .OrderBy(i => i.EventDate)
            .First();
        
        return View(gameEvent);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}
using BordSpellen.Models;
using Core.Domain.Security;
using Core.Domain.Security.Roles;
using Core.Repositories.Data.Actors;
using Core.Repositories.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BordSpellen.Controllers;

[Authorize(Roles = UserRoles.User)]
public class ReviewController : Controller
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IGameEventRepository _gameEventRepository;
    private readonly UserManager<BoardGameUser> _userManager;

    public ReviewController(IReviewRepository reviewRepository,
        IPersonRepository personRepository,
        IGameEventRepository gameEventRepository,
        UserManager<BoardGameUser> userManager)
    {
        _reviewRepository = reviewRepository;
        _personRepository = personRepository;
        _gameEventRepository = gameEventRepository;
        _userManager = userManager;
    }
    
    // GET
    public async Task<IActionResult> Index()
    {
        var identityUser = await _userManager.GetUserAsync(User);
        var person = await _personRepository.GetOneByEmail(identityUser.Email);
        if (person == null)
        {
            return View("CannotAccess");
        }
        
        var result =  await _gameEventRepository.GetReviewedGameEvents(person);
        var gameEventList = result.Distinct().ToList();
        return View(gameEventList);
    }
    public async Task<IActionResult> Create(int id)
    {
        var gameEvent = await _gameEventRepository.GetById(id, g => g.GamerGameEvents!);
        if (gameEvent == null)
        {
            return View("NotFound");
        }

        var identityUser = await _userManager.GetUserAsync(User);
        var person = await _personRepository.GetOneByEmail(identityUser.Email);

        if (gameEvent.GamerGameEvents != null)
        {
            var result = gameEvent.GamerGameEvents.FirstOrDefault(r => r.GameEventId == id && r.PersonId == person!.Id);
            if (result == null || gameEvent.EventDate > DateTime.Now)
            {
                return View("CannotAccess");
            }
        }
        else
        {
            return View("NotFound");
        }

        return View(nameof(Create), new ReviewViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int id, ReviewViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Create), viewModel);
        }
        
        var identityUser = await _userManager.GetUserAsync(User);
        var person = await _personRepository.GetOneByEmail(identityUser.Email);
        var gameEvent = await _gameEventRepository.GetById(id);

        if (person == null || gameEvent == null)
        {
            return View("NotFound");
        }
        
        var response = await _reviewRepository.CreateReview(viewModel.ToDto(viewModel), person, gameEvent);
        if (response != null) return RedirectToAction(nameof(Index));
        
        TempData["error"] = "Couldn't create review try again later";
        return View(nameof(Create), viewModel);
    }
    public async Task<IActionResult> Update(int id)
    {
        var review = await _reviewRepository.GetById(id);
        if (review == null)
        {
            return View("NotFound");
        }

        var gameEvent = await _gameEventRepository.GetById(review.EventId);
        
        var identityUser = await _userManager.GetUserAsync(User);
        var person = await _personRepository.GetOneByEmail(identityUser.Email);
        
        if (review.ReviewPosterId != person!.Id || gameEvent?.EventDate > DateTime.Now)
        {
            return View("CannotAccess");
        }
        
        ReviewViewModel viewModel = new ReviewViewModel
        {
            Id = review.Id,
            Rating = review.Rating,
            ReviewDescription = review.ReviewDescription
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, ReviewViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }
        
        var review = await _reviewRepository.GetById(id);
        if (review == null)
        {
            return View("NotFound");
        }
        
        var identityUser = await _userManager.GetUserAsync(User);
        var person = await _personRepository.GetOneByEmail(identityUser.Email);
        var gameEvent = await _gameEventRepository.GetById(review.EventId);

        viewModel.Id = review.Id;

        if (person == null || gameEvent == null)
        {
            return View("NotFound");
        }
        
        var response = await _reviewRepository.UpdateReview(viewModel.ToDto(viewModel), person, gameEvent);
        return response != null ? RedirectToAction(nameof(Index)) : RedirectToAction("Details", "GameEvent", new { Id = id });
    }
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _reviewRepository.GetById(id);
        if (review == null || String.IsNullOrEmpty(id.ToString()))
        {
            return View("NotFound");
        }
        
        var identityUser = await _userManager.GetUserAsync(User);
        var person = await _personRepository.GetOneByEmail(identityUser.Email);
        
        if (review.ReviewPosterId != person!.Id)
        {
            return View("CannotAccess");
        }
        

        await _reviewRepository.Delete(id);
        return RedirectToAction("Index", "GameEvent");
    }
}
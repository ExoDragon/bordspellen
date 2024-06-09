using BordSpellen.Models;
using BordSpellen.Models.ViewModels;
using Core.Domain.Security;
using Core.Domain.Security.Roles;
using Core.Infrastructure.Helper;
using Core.Repositories.Data.Actors;
using Core.Repositories.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BordSpellen.Controllers;

public class GameEventController : Controller
{
    private readonly IGameEventRepository _gameEventRepository;
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly IDietRepository _dietRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly UserManager<BoardGameUser> _userManager;

    public GameEventController(
        IGameEventRepository gameEventRepository,
        IBoardGameRepository boardGameRepository,
        IDietRepository dietRepository,
        IPersonRepository personRepository,
        IReviewRepository reviewRepository,
        UserManager<BoardGameUser> userManager)
    {
        _gameEventRepository = gameEventRepository;
        _boardGameRepository = boardGameRepository;
        _dietRepository = dietRepository;
        _personRepository = personRepository;
        _userManager = userManager;
        _reviewRepository = reviewRepository;
    }
    
    
    [AllowAnonymous]
    // GET
    public async Task<IActionResult> Index()
    {
        var events = await _gameEventRepository.GetAll(
            r => r.GamerGameEvents!,
            r => r.Organiser!);
        return View(nameof(Index), events);
    }
    
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var gameEventDetails = await _gameEventRepository.GetById(id);
        if (gameEventDetails == null || gameEventDetails.Id != id || String.IsNullOrEmpty(id.ToString())) return View("NotFound");
        
        if (User is {Identity.IsAuthenticated: true})
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var person = await _personRepository.GetOneByEmail(identityUser.Email);
            var gameEvent = await _gameEventRepository.GetById(id, g => g.GamerGameEvents!);
            var subscription = gameEvent?.GamerGameEvents!.FirstOrDefault(ge =>
                ge.PersonId == person!.Id && ge.GameEventId == gameEvent.Id);

            if (subscription == null)
            {
                TempData["Subscription"] = "false";
            }
            else
            {
                TempData["Subscription"] = "true";
            }
        }

        TempData["Average"] = await _reviewRepository.GetAverageOrganiserRating(gameEventDetails.Organiser!);
        
        return View(nameof(Details), gameEventDetails);
    }
    
    [Authorize(Roles = UserRoles.Organiser)]
    public async Task<IActionResult> Create()
    {
        var userEmail = await _userManager.GetEmailAsync(await _userManager.GetUserAsync(User));
        var person = await _personRepository.GetOneByEmail(userEmail);

        if (person == null)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        DateTime date = person.DateOfBirth ?? DateTime.Today;
        if (Helper.Age(date) < 18)
        {
            TempData["error"] = "You must be 18 year or older to organise an Event!";
            return RedirectToAction(nameof(Index));
        }


        var dropdownData = await GetDropdownViewModel();

        ViewBag.boardGames = new SelectList(dropdownData.BoardGameEvents, "Id", "Name");
        ViewBag.diets = new SelectList(dropdownData.AvailableFoodTypes, "Id", "Name");
        
        return View(nameof(Create),new GameEventViewModel());
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Organiser)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GameEventViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var dropdownData = await GetDropdownViewModel();

            ViewBag.boardGames = new SelectList(dropdownData.BoardGameEvents, "Id", "Name");
            ViewBag.diets = new SelectList(dropdownData.AvailableFoodTypes, "Id", "Name");
            
            return View(nameof(Create), viewModel);
        }

        var organiser = await _personRepository.GetOneByEmail(
            await _userManager.GetEmailAsync(
                await _userManager.GetUserAsync(User)
            )
        );

        if (organiser == null) return View("NotFound");
        
        viewModel.Organiser = organiser;
        await _gameEventRepository.Create(viewModel.ToDto(viewModel));
        return RedirectToAction(nameof(Index));
    }
    
    [Authorize(Roles = UserRoles.Organiser)]
    public async Task<IActionResult> Edit(int id)
    {
        var gameEvent =  await _gameEventRepository.GetById(id, g => g.Organiser!,
            g => g.AvailableFoodTypes!,
            g => g.GamerGameEvents!,
            g => g.ReviewsRecieved!,
            g => g.BoardGameEvents!);
        if (gameEvent == null || id != gameEvent.Id || String.IsNullOrEmpty(id.ToString())) return View("NotFound");
        if (await _userManager.GetEmailAsync(await _userManager.GetUserAsync(User)) != gameEvent.Organiser!.Email)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        if (gameEvent.EventDate < DateTime.Now)
        {
            TempData["error"] = "Cannot edit expired events!";
            return RedirectToAction("Index");
        }

        if (gameEvent.GamerGameEvents?.Count > 0 ||
            gameEvent.ReviewsRecieved?.Count > 0)
            return View("CannotAccess");

        var response = new GameEventViewModel
        {
            Id = gameEvent.Id,
            Name = gameEvent.Name,
            Description = gameEvent.Description,
            Street = gameEvent.Street,
            HouseNumber = gameEvent.HouseNumber,
            PostalCode = gameEvent.PostalCode,
            City = gameEvent.City,
            EventDate = gameEvent.EventDate,
            IsAdultEvent = gameEvent.IsAdultEvent,
            MaxAmountOfPlayers = gameEvent.MaxAmountOfPlayers,
            Organiser = gameEvent.Organiser,
            BoardGameIds = gameEvent.BoardGameEvents?.Select(b => b.BoardGameId).ToList(),
            DietIds = gameEvent.AvailableFoodTypes?.Select(b => b.DietId).ToList()
        };

        var dropdownData = await GetDropdownViewModel();

        ViewBag.boardGames = new SelectList(dropdownData.BoardGameEvents, "Id", "Name");
        ViewBag.diets = new SelectList(dropdownData.AvailableFoodTypes, "Id", "Name");
        
        return View(nameof(Edit),response);
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Organiser)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GameEventViewModel viewModel)
    {
        if (id != viewModel.Id || String.IsNullOrEmpty(id.ToString())) return View("NotFound");

        if (!ModelState.IsValid)
        {
            var dropdownData = await GetDropdownViewModel();

            ViewBag.boardGames = new SelectList(dropdownData.BoardGameEvents, "Id", "Name");
            ViewBag.diets = new SelectList(dropdownData.AvailableFoodTypes, "Id", "Name");
            return View(nameof(Edit), viewModel);
        }

        await _gameEventRepository.Update(viewModel.ToDto(viewModel));
        return RedirectToAction(nameof(Index));
    }
    
    [Authorize(Roles = UserRoles.Organiser)]
    public async Task<IActionResult> Delete(int id) 
    {
        var gameEvent = await _gameEventRepository.GetById(id, g => g.Organiser!,
            g=> g.GamerGameEvents!);
        if (gameEvent == null || gameEvent.Id != id || String.IsNullOrEmpty(id.ToString())) return View("NotFound");

        if (gameEvent.GamerGameEvents?.Count > 0)
            return View("CannotAccess");
        
        if (await _userManager.GetEmailAsync(await _userManager.GetUserAsync(User)) != gameEvent.Organiser!.Email)
           return RedirectToAction("AccessDenied", "Account");
        

        await _gameEventRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }
    
    [Authorize(Roles = UserRoles.User)]
    public async Task<IActionResult> Subscribe(int id)
    {
        var identityUser = await _userManager.GetUserAsync(User);
        var person = await _personRepository.GetOneByEmail(identityUser.Email);
        var gameEvent = await _gameEventRepository.GetById(id);
        
        if (person == null || gameEvent == null)
        {
            return View("CannotAccess");
        }

        if (gameEvent.EventDate < DateTime.Now)
        {
            return View("CannotAccess");
        }
        
        if (person.Id == gameEvent.OrganiserId)
        {
            TempData["error"] = "You are the organiser of this event";
            return RedirectToAction(nameof(Details), new { id = gameEvent.Id });
        }
        
        if (gameEvent.IsAdultEvent)
        {
            DateTime date = person.DateOfBirth ?? DateTime.Today;
            var personAge = Helper.Age(date);
            if (personAge < 18)
            {
                TempData["error"] = "This event is for 18 or older";
                return RedirectToAction(nameof(Details), new { id = gameEvent.Id });
            }
        }
        
        if (gameEvent.GamerGameEvents?.Count + 1 > gameEvent.MaxAmountOfPlayers)
        {
            TempData["error"] = "Subscribe limit exceeded";
            return RedirectToAction(nameof(Details), new { id = gameEvent.Id });
        }

        if (await _gameEventRepository.CheckSubscription(gameEvent, person))
        {
            TempData["error"] = "You cannot be subscribed to multiple events on the same day";
            return RedirectToAction(nameof(Details), new { id = gameEvent.Id }); 
        }

        var response = await _gameEventRepository.Subscribe(gameEvent, person);
        if (response)
        {
            if (!await _gameEventRepository.CheckFoodAvailability(gameEvent, person))
            {
                TempData["food"] = "This event doesn't provide food or drinks suitable for your dietary preferences";
            }
            
            TempData["SubscriptionResponse"] = "You are subscribed from this event";
            return RedirectToAction(nameof(Details), new { id = gameEvent.Id });
        }

        TempData["SubscriptionResponse"] = "You are unsubscribed from this event";
        return RedirectToAction(nameof(Details), new { id = gameEvent.Id });
    }
    
    [Authorize(Roles = UserRoles.User)]
    public async Task<IActionResult> Unsubscribe(int id)
    {
        var identityUser = await _userManager.GetUserAsync(User);
        var person = await _personRepository.GetOneByEmail(identityUser.Email);
        var gameEvent = await _gameEventRepository.GetById(id);
        
        if (person == null || gameEvent == null)
        {
            return View("CannotAccess");
        }
        
        if (gameEvent.EventDate < DateTime.Now)
        {
            return View("CannotAccess");
        }

        var subscriptions = await _gameEventRepository.GetSubscribedEvents(person);

        if (!subscriptions.Any())
        {
            TempData["error"] = "You are not subscribed for this event";
            return RedirectToAction(nameof(Details), new { id = gameEvent.Id });
        }
        
        var response = await _gameEventRepository.Subscribe(gameEvent, person);
        if (response)
        {
            if (!await _gameEventRepository.CheckFoodAvailability(gameEvent, person))
            {
                TempData["food"] = "This event doesn't provide food or drinks suitable for your dietary preferences";
            }
            
            TempData["SubscriptionResponse"] = "You are subscribed from this event";
            return RedirectToAction(nameof(Details), new { id = gameEvent.Id });
        }
        
        TempData["SubscriptionResponse"] = "You are unsubscribed from this event";
        return RedirectToAction(nameof(Details), new { id = gameEvent.Id });
    }
    
    public async Task<IActionResult> MyGameEvent()
    {
        var identityUser = await _userManager.GetUserAsync(User);
        var person = await _personRepository.GetOneByEmail(identityUser.Email);
        var gameEvents = await _gameEventRepository.GetAll(
            g => g.Organiser!,
            g => g.GamerGameEvents!);
        
        if (gameEvents == null || person == null)
        {
            return View("CannotAccess");
        }
        
        var response = gameEvents.Where(g => g.OrganiserId == person.Id);
        return View(nameof(MyGameEvent), response);
    }
    
    [Authorize(Roles = UserRoles.User)]
    public async Task<IActionResult> OpedGameEvent()
    {
        var identityUser = await _userManager.GetUserAsync(User);
        var person = await _personRepository.GetOneByEmail(identityUser.Email);

        if (person == null)
        {
            return View("CannotAccess");
        }

        var gameEvents = await _gameEventRepository.GetSubscribedEvents(person);
        return View(nameof(OpedGameEvent), gameEvents);
    }
    public async Task<GameEventDropdownViewModel> GetDropdownViewModel()
    {
        var dropdownData = new GameEventDropdownViewModel();

        var boardGames = await _boardGameRepository.GetAll();
        var diets = await _dietRepository.GetAll();
        dropdownData.BoardGameEvents = boardGames.ToList();
        dropdownData.AvailableFoodTypes = diets.ToList();

        return dropdownData;
    }
}
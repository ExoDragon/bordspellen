using System.Security.Claims;
using Core.Domain.Data.Entities;
using Core.Domain.Security;
using Core.Domain.Security.Roles;
using Core.Infrastructure.Helper;
using Core.Repositories.Data.Actors;
using Core.Repositories.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BordSpellenApi.Controllers;

[Route("api/v1/gameevent")]
[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
public class GameEventController : Controller
{
    private readonly IGameEventRepository _gameEventRepository;
    private readonly IPersonRepository _personRepository;
    private readonly UserManager<BoardGameUser> _userManager; 

    public GameEventController(IGameEventRepository gameEventRepository, 
        IPersonRepository personRepository,
        UserManager<BoardGameUser> userManager)
    {
        _gameEventRepository = gameEventRepository;
        _personRepository = personRepository;
        _userManager = userManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var events = await _gameEventRepository.GetAll(
            r => r.GamerGameEvents!,
            r => r.Organiser!);
        
        return Ok(events);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGameEvent(int id)
    {
        return Ok(await _gameEventRepository.GetById(id));
    }

    [Authorize(Roles = UserRoles.User)]
    [HttpPost("/subscribe/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Subscribe(int id)
    {
        if (String.IsNullOrEmpty(id.ToString()))
        {
            return BadRequest("id cannot be null");
        }

        try
        {
            var email = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role != UserRoles.User)
            {
                return Unauthorized("Organisers cannot subscribe to an event");
            }
            
            var person = await _personRepository.GetOneByEmail(email!);
            if (person == null)
            {
                return NotFound("Person not found");
            }

            var gameEvent = await _gameEventRepository.GetById(id);
            if (gameEvent == null)
            {
                return NotFound("GameEvent not found");
            }
            
            if (gameEvent.EventDate < DateTime.Now)
            {
                return BadRequest("The event date has exceeded");
            }
        
            if (gameEvent.IsAdultEvent)
            {
                DateTime date = person.DateOfBirth ?? DateTime.Today;
                var personAge = Helper.Age(date);
                if (personAge < 18)
                {
                    return Unauthorized("This event is for 18 or older");
                }
            }
        
            if (gameEvent.GamerGameEvents?.Count + 1 > gameEvent.MaxAmountOfPlayers)
            {
                return BadRequest("Subscription amount Exceeded");
            }

            var response = await _gameEventRepository.Subscribe(gameEvent, person);
            if (!response) return Ok("You have been unsubscribed from this event");

            var warning = "";
            if (!await _gameEventRepository.CheckFoodAvailability(gameEvent, person))
            {
                warning = "This event doesn't provide food or drinks suitable for your dietary preferences";
            }

            var message = "You are subscribed from this event";
            return Ok(new { warning, message });

        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}
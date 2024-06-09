using System.Security.Claims;
using BordSpellen.Controllers;
using BordSpellenTests.Util;
using Core.Domain.Data.Actors;
using Core.Domain.Data.Entities;
using Core.Domain.Data.Link;
using Core.Domain.Enums;
using Core.Domain.Security;
using Core.Repositories.Data.Actors;
using Core.Repositories.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BordSpellenTests;

public class UserStory4
{
    private GameEventController _gameEventController;
    private Mock<IPersonRepository> _personRepository;
    private Mock<IGameEventRepository> _gameEventRepository;
    private readonly List<BoardGameUser> _users = new ()
    {
        new BoardGameUser
        {
            Id = new Guid().ToString(),
            UserName = "to.terlaak@outlook.com",
            Email = "to.terlaak@outlook.com",
            EmailConfirmed = true
        },
        new BoardGameUser
        {
            Id = new Guid().ToString(),
            UserName = "t.ester2@gmail.com",
            Email = "t.ester2@gmail.com",
            EmailConfirmed = true
        }
    };    
    
    #region BoardGames

    private readonly BoardGame _boardGame1 = new()
    {
        Id = 1,
        Name = "Card game bluff",
        Description = "This is a description for Card game bluff",
        Genre = BoardGameGenre.BLUFFING,
        HasAgeRestriction = false,
        Image = null,
        ImageFormat = "",
        Type = BoardGameType.CARD
    };

    private readonly BoardGame _boardGame2 = new()
    {
        Id = 2,
        Name = "Warhammer Underworlds",
        Description = "This is a description for Warhammer Underworlds",
        Genre = BoardGameGenre.MINIATURE,
        HasAgeRestriction = true,
        Image = null,
        ImageFormat = "",
        Type = BoardGameType.BOARD
    };

    #endregion

    #region GameEvents

    private readonly GameEvent _gameEvent1 = new()
    {
        Id  = 1,
        Name = "Cool Game Event",
        Description = "This is a cool game event",
        Street = "Vogelven",
        HouseNumber = "45",
        PostalCode = "4631KH",
        City = "Hoogerheide",
        IsAdultEvent = false,
        EventDate = new DateTime(2022, 10, 30),
        MaxAmountOfPlayers = 2,
        OrganiserId = 1,
        GamerGameEvents = new List<PersonGameEvents>
        {
            new ()
            {
                GameEventId = 1,
                PersonId = 2
            },
            new ()
            {
                GameEventId = 1,
                PersonId = 4
            }
        }
    };
    
    private readonly GameEvent _gameEvent2 = new()
    {
        Id  = 2,
        Name = "Cool Game Event",
        Description = "This is a cool game event",
        Street = "Vogelven",
        HouseNumber = "45",
        PostalCode = "4631KH",
        City = "Hoogerheide",
        IsAdultEvent = true,
        EventDate = new DateTime(2022, 10, 30),
        MaxAmountOfPlayers = 10,
        OrganiserId = 1
    };

    #endregion

    public UserStory4()
    {
        _gameEventRepository = new Mock<IGameEventRepository>();
        _personRepository = new Mock<IPersonRepository>();
        var boardGameRepository = new Mock<IBoardGameRepository>();
        var dietRepository = new Mock<IDietRepository>();
        var reviewRepository = new Mock<IReviewRepository>();
        var userManager = Helper.MockUserManager(_users);
        
        ITempDataProvider tempDataProvider = Mock.Of<ITempDataProvider>();
        TempDataDictionaryFactory tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
        ITempDataDictionary tempData = tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());

        var user = new Mock<ClaimsPrincipal>(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, "1")
        }));

        userManager.Setup(x => x.GetUserAsync(user.Object)).ReturnsAsync(_users[0]);
        userManager.Setup(x => x.GetEmailAsync(_users[0])).ReturnsAsync(_users[0].Email);

        _gameEventController = new GameEventController(_gameEventRepository.Object,
            boardGameRepository.Object,
            dietRepository.Object,
            _personRepository.Object,
            reviewRepository.Object,
            userManager.Object)
        {
            TempData = tempData
        };
        
        _gameEventController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user.Object
            }
        };
    }

    [Fact]
    public async Task US4_01_UserCannotJoinEventMaxUsersExceeded()
    {
        //Arrange
        Person person1 = new()
        {
            Id = 3,
            FirstName = "Thomas",
            LastName = "Terlaak",
            Email = "to.terlaak@outlook.com",
            Gender = GenderEnum.MALE,
            DateOfBirth = new DateTime(1999, 11, 8),
            Street = "Vogelven",
            HouseNumber = "16",
            PostalCode = "4631MP",
            City = "Hoogerheide",
            PhoneNumber = "06-123456789"
        };
        
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>())).ReturnsAsync(() => person1);
        _gameEventRepository.Setup(x => x.GetById(2)).ReturnsAsync(() => _gameEvent1);
        _gameEventRepository.Setup(x => x.Subscribe(_gameEvent2, person1)).ReturnsAsync(true);

        //Act
        var result = await _gameEventController.Subscribe(_gameEvent2.Id) as RedirectToActionResult;
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(nameof(_gameEventController.Details), result.ActionName);
        Assert.Equal("Subscribe limit exceeded", _gameEventController.TempData["error"]);
    }
    
    [Fact]
    public async Task US4_01_UserCanJoinEvent()
    {
        //Arrange
        Person person1 = new()
        {
            Id = 3,
            FirstName = "Thomas",
            LastName = "Terlaak",
            Email = "to.terlaak@outlook.com",
            Gender = GenderEnum.MALE,
            DateOfBirth = new DateTime(1999, 11, 8),
            Street = "Vogelven",
            HouseNumber = "16",
            PostalCode = "4631MP",
            City = "Hoogerheide",
            PhoneNumber = "06-123456789"
        };
        
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>())).ReturnsAsync(() => person1);
        _gameEventRepository.Setup(x => x.GetById(2)).ReturnsAsync(() => _gameEvent2);
        _gameEventRepository.Setup(x => x.Subscribe(_gameEvent2, person1)).ReturnsAsync(true);

        //Act
        var result = await _gameEventController.Subscribe(_gameEvent2.Id) as RedirectToActionResult;
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(nameof(_gameEventController.Details), result.ActionName);
        Assert.Equal("You are subscribed from this event", _gameEventController.TempData["SubscriptionResponse"]);
    }
    
    [Fact]
    public async Task US4_03_UserHasSubscriptionAlready()
    {
        //Arrange
        Person person1 = new()
        {
            Id = 3,
            FirstName = "Thomas",
            LastName = "Terlaak",
            Email = "to.terlaak@outlook.com",
            Gender = GenderEnum.MALE,
            DateOfBirth = new DateTime(1999, 11, 8),
            Street = "Vogelven",
            HouseNumber = "16",
            PostalCode = "4631MP",
            City = "Hoogerheide",
            PhoneNumber = "06-123456789"
        };
        
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>())).ReturnsAsync(() => person1);
        _gameEventRepository.Setup(x => x.GetById(2)).ReturnsAsync(() => _gameEvent2);
        _gameEventRepository.Setup(x => x.Subscribe(_gameEvent2, person1)).ReturnsAsync(true);
        _gameEventRepository.Setup(x => x.CheckSubscription(_gameEvent2, person1)).ReturnsAsync(true);

        //Act
        var result = await _gameEventController.Subscribe(_gameEvent2.Id) as RedirectToActionResult;
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(nameof(_gameEventController.Details), result.ActionName);
        Assert.Equal("You cannot be subscribed to multiple events on the same day", _gameEventController.TempData["error"]);
    }
    
    [Fact]
    public async Task US4_04_UserHasNoSubscriptions()
    {
        //Arrange
        Person person1 = new()
        {
            Id = 3,
            FirstName = "Thomas",
            LastName = "Terlaak",
            Email = "to.terlaak@outlook.com",
            Gender = GenderEnum.MALE,
            DateOfBirth = new DateTime(1999, 11, 8),
            Street = "Vogelven",
            HouseNumber = "16",
            PostalCode = "4631MP",
            City = "Hoogerheide",
            PhoneNumber = "06-123456789"
        };
        
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>())).ReturnsAsync(() => person1);
        _gameEventRepository.Setup(x => x.GetById(2)).ReturnsAsync(() => _gameEvent2);
        _gameEventRepository.Setup(x => x.Subscribe(_gameEvent2, person1)).ReturnsAsync(true);
        _gameEventRepository.Setup(x => x.CheckSubscription(_gameEvent2, person1)).ReturnsAsync(false);

        //Act
        var result = await _gameEventController.Subscribe(_gameEvent2.Id) as RedirectToActionResult;
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(nameof(_gameEventController.Details), result.ActionName);
        Assert.Equal("You are subscribed from this event", _gameEventController.TempData["SubscriptionResponse"]);
    }
    
    
}
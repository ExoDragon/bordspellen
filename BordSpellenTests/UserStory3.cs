using System.Security.Claims;
using BordSpellen.Controllers;
using BordSpellen.Models;
using BordSpellenTests.Util;
using Core.Domain.Data.Actors;
using Core.Domain.Data.Entities;
using Core.Domain.Dto.Entities;
using Core.Domain.Enums;
using Core.Domain.Security;
using Core.Infrastructure.Context;
using Core.Infrastructure.Repository.Entities;
using Core.Repositories.Data.Actors;
using Core.Repositories.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace BordSpellenTests;

public class UserStory3
{
    private GameEventController _gameEventController;
    private Mock<IPersonRepository> _personRepository;
    private Mock<IGameEventRepository> _gameEventRepository;
    private readonly BoardGameDatabaseRepository _boardGameDatabaseRepository;
    private readonly GameEventDatabaseRepository _gameEventDatabaseRepository;
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
        ImageFormat = ".png",
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
        MaxAmountOfPlayers = 10,
        OrganiserId = 1
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
    
    
    public UserStory3()
    {
        DbContextOptionsBuilder<EntityDatabaseContext> dbOptions = new DbContextOptionsBuilder<EntityDatabaseContext>()
            .UseInMemoryDatabase(
                Guid.NewGuid().ToString()
            );
        var context = new EntityDatabaseContext(dbOptions.Options);

        _boardGameDatabaseRepository = new BoardGameDatabaseRepository(context);
        _gameEventDatabaseRepository = new GameEventDatabaseRepository(context);
        
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
        

        #pragma warning disable CS4014
                SetupDatabase();
        #pragma warning restore CS4014
    }

    private async Task SetupDatabase()
    {
        await _boardGameDatabaseRepository.Create(_boardGame1);
        await _boardGameDatabaseRepository.Create(_boardGame2);

        await _gameEventDatabaseRepository.Create(_gameEvent1);
    }

    [Fact]
    public void US3_01_BoardGameHasAdultOnlyProperty()
    {
        //Arrange
        
        //Act
        
        //Assert
        Assert.NotNull(typeof(BoardGame).GetProperty("HasAgeRestriction"));
        Assert.NotNull(typeof(GameEvent).GetProperty("IsAdultEvent"));
        Assert.NotNull(typeof(BoardGameViewModel).GetProperty("HasAgeRestriction"));
        Assert.NotNull(typeof(GameEventViewModel).GetProperty("IsAdultEvent"));
    }

    [Fact]
    public async Task US3_02_GameEventHasAdultBoardGames()
    {
        //Arrange
        var dto = new GameEventDto
        {
            Id = 1,
            Name = "Tester",
            Description = "Test Events",
            Street = "hello",
            HouseNumber = "12A",
            PostalCode = "4651PS",
            City = "Hoogerheide",
            IsAdultEvent = false,
            EventDate = new DateTime(2022,11, 08),
            MaxAmountOfPlayers = 9,
            ReviewIds = new List<int>(),
            BoardGameIds = new List<int> { 1, 2 },
            DietIds = new List<int>(),
            GamerIds = new List<int>()
        };
        
        //Act
        var result = await _gameEventDatabaseRepository.Update(dto);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsAdultEvent);
    }
    
    [Fact]
    public async Task US3_03_GameEventHasNoAdultBoardGames()
    {
        //Arrange
        var dto = new GameEventDto
        {
            Id = 1,
            Name = "Tester",
            Description = "Test Events",
            Street = "hello",
            HouseNumber = "12A",
            PostalCode = "4651PS",
            City = "Hoogerheide",
            IsAdultEvent = false,
            EventDate = new DateTime(2022,11, 08),
            MaxAmountOfPlayers = 9,
            ReviewIds = new List<int>(),
            BoardGameIds = new List<int> { 1 },
            DietIds = new List<int>(),
            GamerIds = new List<int>()
        };
        
        //Act
        var result = await _gameEventDatabaseRepository.Update(dto);

        //Assert
        Assert.NotNull(result);
        Assert.False(result.IsAdultEvent);
    }

    [Fact]
    public async Task US3_04_UserCannotJoinAdultEvent()
    {
        //Arrange
        Person person2 = new()
        {
            Id = 4,
            FirstName = "Wouter",
            LastName = "Terlaak",
            Email = "wa.terlaak@outlook.com",
            Gender = GenderEnum.MALE,
            DateOfBirth = new DateTime(2010, 9, 8),
            Street = "Vogelven",
            HouseNumber = "16",
            PostalCode = "4631MP",
            City = "Hoogerheide",
            PhoneNumber = "06-123456789"
        };
        
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>())).ReturnsAsync(() => person2);
        _gameEventRepository.Setup(x => x.GetById(2)).ReturnsAsync(() => _gameEvent2);
        _gameEventRepository.Setup(x => x.Subscribe(_gameEvent2, person2)).ReturnsAsync(true);

        //Act
        var result = await _gameEventController.Subscribe(_gameEvent2.Id) as RedirectToActionResult;
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(nameof(_gameEventController.Details), result.ActionName);
        Assert.Equal("This event is for 18 or older", _gameEventController.TempData["error"]);
    }
    
    [Fact]
    public async Task US3_05_UserCanJoinAdultEvent()
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
}
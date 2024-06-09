using System.Security.Claims;
using BordSpellen.Controllers;
using BordSpellenTests.Util;
using Core.Domain.Data.Actors;
using Core.Domain.Data.Entities;
using Core.Domain.Enums;
using Core.Domain.Security;
using Core.Repositories.Data.Actors;
using Core.Repositories.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BordSpellenTests;

public class UserStory1
{
    private readonly GameEventController _gameEventController;
    private readonly Mock<IGameEventRepository> _gameEventRepository;
    private readonly Mock<IPersonRepository> _personRepository;
    private readonly List<BoardGameUser> _users = new ()
    {
        new BoardGameUser
        {
            Id = new Guid().ToString(),
            UserName = "t.ester@gmail.com",
            Email = "t.ester@gmail.com",
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

    #region GameEvents

    private readonly GameEvent _gameEvent1 = new()
    {
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
        Name = "Another Cool Game Event",
        Description = "This is another cool game event",
        Street = "Vogelven",
        HouseNumber = "45",
        PostalCode = "4631KH",
        City = "Hoogerheide",
        IsAdultEvent = true,
        EventDate = new DateTime(2022, 11, 8),
        MaxAmountOfPlayers = 15,
        OrganiserId = 2
    };
    
    #endregion

    #region Persons

    private readonly Person _person1 = new()
    {
        Id = 1,
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

    #endregion
    
    public UserStory1()
    {
        _gameEventRepository = new Mock<IGameEventRepository>();
        _personRepository = new Mock<IPersonRepository>();
        var boardGameRepository = new Mock<IBoardGameRepository>();
        var dietRepository = new Mock<IDietRepository>();
        var reviewRepository = new Mock<IReviewRepository>();
        var userManager = Helper.MockUserManager(_users);
        
        var user = new Mock<ClaimsPrincipal>(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, "1")
        }));

        userManager.Setup(x => x.GetUserAsync(user.Object)).ReturnsAsync(_users[0]);

        _gameEventController = new GameEventController(_gameEventRepository.Object,
            boardGameRepository.Object,
            dietRepository.Object,
            _personRepository.Object,
            reviewRepository.Object,
            userManager.Object);
        
        _gameEventController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user.Object
            }
        };
    }

    [Fact]
    public async Task US1_01_GameEventPageNoEvents()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetAll()).ReturnsAsync(() => new List<GameEvent>());
        
        //Act
        var gameEventResult = await _gameEventController.Index() as ViewResult;
        var model = gameEventResult?.Model as IEnumerable<GameEvent>;
        
        //Assert
        Assert.Equal("Index", gameEventResult?.ViewName);
        Assert.NotNull(model);
        Assert.Empty(model);
    }

    [Fact]
    public async Task US1_02_GameEventPageWithEvents()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetAll(
                r => r.GamerGameEvents!,
                r => r.Organiser!))
            .ReturnsAsync(() => new List<GameEvent> { _gameEvent1, _gameEvent2 });
        
        //Act
        var gameEventResult = await _gameEventController.Index() as ViewResult;
        var model = gameEventResult?.Model as IEnumerable<GameEvent>;
        
        //Assert
        Assert.Equal("Index", gameEventResult?.ViewName);
        Assert.NotNull(model);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task US1_03_GameEventOrganiserNoEvents()
    {
        //Arrange
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>()))
            .ReturnsAsync(() => _person1);
        
        _gameEventRepository.Setup(x => x.GetAll(
                g => g.Organiser!,
                g => g.GamerGameEvents!))
            .ReturnsAsync(() => new List<GameEvent>());

        //Act
        var gameEventResult = await _gameEventController.MyGameEvent() as ViewResult;
        var model = gameEventResult?.Model as IEnumerable<GameEvent>;

        //Assert
        Assert.Equal("MyGameEvent", gameEventResult?.ViewName);
        Assert.NotNull(model);
        Assert.Empty(model);
    }
    
    [Fact]
    public async Task US1_04_GameEventOrganiserWithOneEvent()
    {
        //Arrange
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>()))
            .ReturnsAsync(() => _person1);
        
        _gameEventRepository.Setup(x => x.GetAll(
                g => g.Organiser!,
                g => g.GamerGameEvents!))
            .ReturnsAsync(() => new List<GameEvent> { _gameEvent1, _gameEvent2 });

        //Act
        var gameEventResult = await _gameEventController.MyGameEvent() as ViewResult;
        var model = gameEventResult?.Model as IEnumerable<GameEvent>;

        //Assert
        Assert.Equal("MyGameEvent", gameEventResult?.ViewName);
        Assert.NotNull(model);
        Assert.Equal(1, model.Count());
    }
    
    [Fact]
    public async Task US1_05_GameEventOpedNoEvents()
    {
        //Arrange
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>()))
            .ReturnsAsync(() => _person1);
        
        _gameEventRepository.Setup(x => x.GetSubscribedEvents(It.IsAny<Person>()))
            .ReturnsAsync(() => new List<GameEvent>());

        //Act
        var gameEventResult = await _gameEventController.OpedGameEvent() as ViewResult;
        var model = gameEventResult?.Model as IEnumerable<GameEvent>;

        //Assert
        Assert.Equal("OpedGameEvent", gameEventResult?.ViewName);
        Assert.NotNull(model);
        Assert.Empty(model);
    }
    
    [Fact]
    public async Task US1_05_GameEventOpedWithOneEvent()
    {
        //Arrange
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>()))
            .ReturnsAsync(() => _person1);
        
        _gameEventRepository.Setup(x => x.GetSubscribedEvents(It.IsAny<Person>()))
            .ReturnsAsync(() => new List<GameEvent> { _gameEvent1 });

        //Act
        var gameEventResult = await _gameEventController.OpedGameEvent() as ViewResult;
        var model = gameEventResult?.Model as IEnumerable<GameEvent>;

        //Assert
        Assert.Equal("OpedGameEvent", gameEventResult?.ViewName);
        Assert.NotNull(model);
        Assert.Equal(1, model.Count());
    }
    
}
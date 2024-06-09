using System.Security.Claims;
using BordSpellen.Controllers;
using BordSpellen.Models;
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

public class UserStory2
{
    private readonly GameEventController _gameEventController;
    private readonly Mock<IGameEventRepository> _gameEventRepository;
    private readonly Mock<IPersonRepository> _personRepository;
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

    #region Persons

    private Person _person1 = new()
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

    private Person _person2 = new()
    {
        Id = 2,
        FirstName = "Wouter",
        LastName = "Terlaak",
        Email = "wa.terlaak@outlook.com",
        Gender = GenderEnum.MALE,
        DateOfBirth = new DateTime(2003, 9, 8),
        Street = "Vogelven",
        HouseNumber = "16",
        PostalCode = "4631MP",
        City = "Hoogerheide",
        PhoneNumber = "06-123456789"
    };

    #endregion
    
    #region GameEvents

    private readonly GameEvent _gameEvent1 = new()
    {
        Id = 1,
        Name = "Cool Game Event",
        Description = "This is a cool game event",
        Street = "Vogelven",
        HouseNumber = "45",
        PostalCode = "4631KH",
        City = "Hoogerheide",
        IsAdultEvent = false,
        EventDate = new DateTime(2030, 10, 30),
        MaxAmountOfPlayers = 10,
        Organiser = new ()
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
        }
    };

    private readonly GameEvent _gameEvent2 = new()
    {
        Id = 2,
        Name = "Cool Game Event",
        Description = "This is a cool game event",
        Street = "Vogelven",
        HouseNumber = "45",
        PostalCode = "4631KH",
        City = "Hoogerheide",
        IsAdultEvent = false,
        EventDate = new DateTime(2030, 10, 30),
        MaxAmountOfPlayers = 10,
        Organiser = new ()
        {
            Id = 2,
            FirstName = "Wouter",
            LastName = "Terlaak",
            Email = "wa.terlaak@outlook.com",
            Gender = GenderEnum.MALE,
            DateOfBirth = new DateTime(2003, 9,8),
            Street = "Vogelven",
            HouseNumber = "16",
            PostalCode = "4631MP",
            City = "Hoogerheide",
            PhoneNumber = "06-123456789"
        }
    };
    
    private readonly GameEvent _gameEvent3 = new()
    {
        Id = 3,
        Name = "Cool Game Event",
        Description = "This is a cool game event",
        Street = "Vogelven",
        HouseNumber = "45",
        PostalCode = "4631KH",
        City = "Hoogerheide",
        IsAdultEvent = false,
        EventDate = new DateTime(2030, 10, 30),
        MaxAmountOfPlayers = 10,
        Organiser = new ()
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
        },
        GamerGameEvents = new List<PersonGameEvents>
        {
            new ()
            {
                PersonId = 1,
                GameEventId = 1
            }
        }
    };
    
    private readonly GameEvent _gameEvent4 = new()
    {
        Id = 1,
        Name = "Cool Game Event",
        Description = "This is a cool game event",
        Street = "Vogelven",
        HouseNumber = "45",
        PostalCode = "4631KH",
        City = "Hoogerheide",
        IsAdultEvent = false,
        EventDate = new DateTime(2022, 10, 08),
        MaxAmountOfPlayers = 10,
        Organiser = new ()
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
        }
    };
    
    #endregion

    #region ViewModel

    private GameEventViewModel _viewModel = new()
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
        BoardGameIds = new List<int>(),
        DietIds = new List<int>()
    };

    #endregion
    
    public UserStory2()
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
        
        ITempDataProvider tempDataProvider = Mock.Of<ITempDataProvider>();
        TempDataDictionaryFactory tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
        ITempDataDictionary tempData = tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());

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
    public async Task US2_01_GameEventDetailPageNoEvent()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetById(It.IsAny<int>()))
            .ReturnsAsync(() => null);
        
        //Act
        var gameEventResult = await _gameEventController.Details(0) as ViewResult;

        //Assert
        Assert.Equal("NotFound", gameEventResult?.ViewName);
    }
    
    [Fact]
    public async Task US2_02_GameEventDetailPageWithEvent()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetById(It.IsAny<int>()))
            .ReturnsAsync(() => _gameEvent1);
        
        //Act
        var gameEventResult = await _gameEventController.Details(1) as ViewResult;
        var model = gameEventResult?.Model as GameEvent;

        //Assert
        Assert.Equal("Details", gameEventResult?.ViewName);
        Assert.NotNull(model);
        Assert.IsType<GameEvent>(model);
        Assert.Equal(_gameEvent1.Id, model.Id);
    }

    [Fact]
    public async Task US2_03_GameEventCreatePage()
    {
        //Arrange
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>())).ReturnsAsync(_person1);
        
        //Act
        var gameEventResult = await _gameEventController.Create() as ViewResult;

        //Assert
        Assert.Equal(nameof(_gameEventController.Create), gameEventResult?.ViewName);
        Assert.IsType<GameEventViewModel>(gameEventResult?.Model);
    }

    [Fact]
    public async Task US2_04_GameEventCreateInvalidModel()
    {
        //Arrange
        GameEventViewModel viewModel = new GameEventViewModel { EventDate = new DateTime(1999, 11 ,08) };
        _gameEventController.ValidateModel(viewModel);
        
        //Act
        var gameEventResult = await _gameEventController.Create(viewModel) as ViewResult;
        
        //Assert
        Assert.Equal(nameof(_gameEventController.Create), gameEventResult?.ViewName);
        Assert.False(gameEventResult?.ViewData.ModelState.IsValid);
    }

    [Fact]
    public async Task US2_05_GameEventCreateValidModel()
    {
        //Arrange
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>()))
            .ReturnsAsync(_person1);
        
        _gameEventController.ValidateModel(_viewModel);
        
        //Act
        var gameEventResult = await _gameEventController.Create(_viewModel) as RedirectToActionResult;
        
        //Assert
        Assert.Equal(nameof(_gameEventController.Index), gameEventResult?.ActionName);
    }

    [Fact]
    public async Task US2_06_GameEventEditNoEvent()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetById(It.IsAny<int>(),
                g => g.Organiser!,
                g => g.AvailableFoodTypes!,
                g => g.BoardGameEvents!))
            .ReturnsAsync(() => null);
        
        //Act
        var gameEventResult = await _gameEventController.Edit(1) as ViewResult;

        //Assert
        Assert.Equal("NotFound", gameEventResult?.ViewName);
    }
    
    [Fact]
    public async Task US2_07_GameEventEditNotAllowed()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetById(2,
                g => g.Organiser!,
                g => g.AvailableFoodTypes!,
                g => g.GamerGameEvents!,
                g => g.ReviewsRecieved!,
                g => g.BoardGameEvents!))
            .ReturnsAsync(() => _gameEvent2);
        
        //Act
        var gameEventResult = await _gameEventController.Edit(2) as RedirectToActionResult;

        //Assert
        Assert.Equal("AccessDenied", gameEventResult?.ActionName);
    }
    
    [Fact]
    public async Task US2_08_GameEventEditHasSubscriptions()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetById(It.IsAny<int>(),
                g => g.Organiser!,
                g => g.AvailableFoodTypes!,
                g => g.GamerGameEvents!,
                g => g.ReviewsRecieved!,
                g => g.BoardGameEvents!))
            .ReturnsAsync(() => _gameEvent3);
        
        //Act
        var gameEventResult = await _gameEventController.Edit(3) as ViewResult;

        //Assert
        Assert.Equal("CannotAccess", gameEventResult?.ViewName);
    }
    
    [Fact]
    public async Task US2_09_GameEventEdit()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetById(1,
                g => g.Organiser!,
                g => g.AvailableFoodTypes!,
                g => g.GamerGameEvents!,
                g => g.ReviewsRecieved!,
                g => g.BoardGameEvents!))
            .ReturnsAsync(() => _gameEvent1);
        
        //Act
        var gameEventResult = await _gameEventController.Edit(1) as ViewResult;
        var model = gameEventResult?.Model as GameEventViewModel;

        //Assert
        Assert.Equal(nameof(_gameEventController.Edit), gameEventResult?.ViewName);
        Assert.NotNull(model);
        Assert.Equal(_gameEvent1.Name, model.Name);
    }
    
    [Fact]
    public async Task US2_10_GameEventEditExpiredEvent()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetById(1,
                g => g.Organiser!,
                g => g.AvailableFoodTypes!,
                g => g.GamerGameEvents!,
                g => g.ReviewsRecieved!,
                g => g.BoardGameEvents!))
            .ReturnsAsync(() => _gameEvent4);
        
        //Act
        var gameEventResult = await _gameEventController.Edit(1) as RedirectToActionResult;

        //Assert
        Assert.Equal(nameof(_gameEventController.Index), gameEventResult?.ActionName);
        Assert.Equal(_gameEventController.TempData["error"], "Cannot edit expired events!");
    }

    [Fact]
    public async Task US2_11_GameEventEditInvalidModel()
    {
        //Arrange
        GameEventViewModel viewModel = new GameEventViewModel
        {
            Id = 1,
            EventDate = new DateTime(1999, 11 ,08)
        };
        _gameEventController.ValidateModel(viewModel);
        
        //Act
        var gameEventResult = await _gameEventController.Edit(viewModel.Id, viewModel) as ViewResult;

        //Assert
        Assert.Equal(nameof(_gameEventController.Edit), gameEventResult?.ViewName);
        Assert.False(gameEventResult?.ViewData.ModelState.IsValid);
    }

    [Fact]
    public async Task US2_12_GameEventEditValidModel()
    {
        //Arrange
        _gameEventController.ValidateModel(_viewModel);
        
        //Act
        var gameEventResult = await _gameEventController.Edit(_viewModel.Id, _viewModel) as RedirectToActionResult;
        
        //Assert
        Assert.Equal(nameof(_gameEventController.Index), gameEventResult?.ActionName);
    }

    [Fact]
    public async Task US2_13_GameEventDeleteNoEvent()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetById(It.IsAny<int>(),
                g => g.Organiser!,
                g=> g.GamerGameEvents!))
            .ReturnsAsync(() => null);
        
        //Act
        var gameEventResult = await _gameEventController.Delete(1) as ViewResult;

        //Assert
        Assert.Equal("NotFound", gameEventResult?.ViewName);
    }
    
    [Fact]
    public async Task US2_14_GameEventDeleteCannotAccess()
    {
        _gameEventRepository.Setup(x => x.GetById(It.IsAny<int>(),
                g => g.Organiser!,
                g=> g.GamerGameEvents!))
            .ReturnsAsync(() => _gameEvent3);
        
        //Act
        var gameEventResult = await _gameEventController.Delete(3) as ViewResult;

        //Assert
        Assert.Equal("CannotAccess", gameEventResult?.ViewName);
    }
    
    [Fact]
    public async Task US2_15_GameEventDelete()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetById(1,
                g => g.Organiser!,
                g=> g.GamerGameEvents!))
            .ReturnsAsync(() => _gameEvent1);
        
        //Act
        var gameEventResult = await _gameEventController.Delete(1) as RedirectToActionResult;

        //Assert
        Assert.Equal(nameof(_gameEventController.Index), gameEventResult?.ActionName);
    }
}
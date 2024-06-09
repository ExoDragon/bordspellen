using System.Security.Claims;
using BordSpellen.Controllers;
using BordSpellen.Models.DtoModels;
using BordSpellenTests.Util;
using Core.Domain.Data.Actors;
using Core.Domain.Data.Entities;
using Core.Domain.Data.Link;
using Core.Domain.Enums;
using Core.Domain.Security;
using Core.Repositories.Data.Actors;
using Core.Repositories.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BordSpellenTests;

public class FakeSignInManager : SignInManager<BoardGameUser>
{
    public FakeSignInManager()
        : base(new FakeUserManager(),
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<BoardGameUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<BoardGameUser>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<BoardGameUser>>().Object)
    { }        
}
public class FakeUserManager : UserManager<BoardGameUser>
{
    public FakeUserManager()
        : base(new Mock<IUserStore<BoardGameUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<BoardGameUser>>().Object,
            new IUserValidator<BoardGameUser>[0],
            new IPasswordValidator<BoardGameUser>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<BoardGameUser>>>().Object)
    { }

    public override Task<IdentityResult> CreateAsync(BoardGameUser user, string password)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<IdentityResult> AddToRoleAsync(BoardGameUser user, string role)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<string> GenerateEmailConfirmationTokenAsync(BoardGameUser user)
    {
        return Task.FromResult(Guid.NewGuid().ToString());
    }

}

public class UserStory6
{
    private readonly GameEventController _gameEventController;
    private readonly AccountController _accountController;
    private readonly Mock<IPersonRepository> _personRepository;
    private readonly Mock<IGameEventRepository> _gameEventRepository;
    private readonly Mock<IDietRepository> _dietRepository;
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
        BoardGameEvents = new List<BoardGameEvents>
        {
            new ()
            {
                GameEventId = 1,
                BoardGame = new BoardGame
                {
                    Id = 1,
                    Name = "Card game bluff",
                    Description = "This is a description for Card game bluff",
                    Genre = BoardGameGenre.BLUFFING,
                    HasAgeRestriction = false,
                    Image = null,
                    ImageFormat = "",
                    Type = BoardGameType.CARD
                }
            },
            new ()
            {
                GameEventId = 1,
                BoardGame = new BoardGame
                {
                    Id = 2,
                    Name = "Card game bluff",
                    Description = "This is a description for Card game bluff",
                    Genre = BoardGameGenre.BLUFFING,
                    HasAgeRestriction = true,
                    Image = null,
                    ImageFormat = "",
                    Type = BoardGameType.CARD
                }
            }
        },
        AvailableFoodTypes = new List<GameEventDiets>
        {
            new ()
            {
                GameEventId = 1,
                Diet = new Diet
                {
                    Name = "Lactose Intolerant",
                    Description = "Cannot consume dairy products"
                }
            },
            new ()
            {
                GameEventId = 1,
                Diet = new Diet
                {
                    Name = "Vegetarian",
                    Description = "Cannot consume meat or dairy products"
                }
            },
            new ()
            {
                GameEventId = 1,
                Diet = new Diet
                {
                    Name = "Non Alcoholic",
                    Description = "Cannot or won't Drink alcohol"
                }
            },
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

    public UserStory6()
    {
        _gameEventRepository = new Mock<IGameEventRepository>();
        _personRepository = new Mock<IPersonRepository>();
        var boardGameRepository = new Mock<IBoardGameRepository>();
        _dietRepository = new Mock<IDietRepository>();
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
            _dietRepository.Object,
            _personRepository.Object,
            reviewRepository.Object,
            userManager.Object)
        {
            TempData = tempData
        };

        var signInManager = new Mock<FakeSignInManager>();
        _accountController = new AccountController(userManager.Object, 
            signInManager.Object, 
            _personRepository.Object,
            _dietRepository.Object)
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
    public async Task US6_01_GameEventHasDietaryOptions()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetById(1))
            .ReturnsAsync(_gameEvent1);

        //Act
        var result = await _gameEventController.Details(1) as ViewResult;
        var model = result?.Model as GameEvent;

        //Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(3, model.AvailableFoodTypes?.Count);
        Assert.Equal("Lactose Intolerant", model.AvailableFoodTypes?[0].Diet.Name);
        Assert.Equal("Vegetarian", model.AvailableFoodTypes?[1].Diet.Name);
        Assert.Equal("Non Alcoholic", model.AvailableFoodTypes?[2].Diet.Name);
    }
    
    [Fact]
    public async Task US6_02_PersonGetsWarningForDietaryOptions()
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
        _gameEventRepository.Setup(x => x.Subscribe(_gameEvent1, person1)).ReturnsAsync(true);
        _gameEventRepository.Setup(x => x.CheckSubscription(_gameEvent1, person1)).ReturnsAsync(false);
        _gameEventRepository.Setup(x => x.CheckFoodAvailability(_gameEvent1, person1)).ReturnsAsync(false);

        //Act
        var result = await _gameEventController.Subscribe(_gameEvent2.Id) as RedirectToActionResult;
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(nameof(_gameEventController.Details), result.ActionName);
        Assert.Equal("You are subscribed from this event", _gameEventController.TempData["SubscriptionResponse"]);
        Assert.Equal("This event doesn't provide food or drinks suitable for your dietary preferences", _gameEventController.TempData["food"]);
    }
    
    [Fact]
    public async Task US6_03_PersonGetsNoWarning()
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
        _gameEventRepository.Setup(x => x.Subscribe(_gameEvent1, person1)).ReturnsAsync(true);
        _gameEventRepository.Setup(x => x.CheckSubscription(_gameEvent1, person1)).ReturnsAsync(false);
        _gameEventRepository.Setup(x => x.CheckFoodAvailability(_gameEvent1, person1)).ReturnsAsync(true);

        //Act
        var result = await _gameEventController.Subscribe(_gameEvent2.Id) as RedirectToActionResult;
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(nameof(_gameEventController.Details), result.ActionName);
        Assert.Equal("You are subscribed from this event", _gameEventController.TempData["SubscriptionResponse"]);
        Assert.Null(_gameEventController.TempData["food"]);
    }

    [Fact]
    public async Task US6_04_UsersCanChooseDietaryPreferences()
    {
        //Arrange
        _dietRepository.Setup(x => x.GetAll()).ReturnsAsync(() => new List<Diet>
        {
            new ()
            {
                Id = 1,
                Name = "Lactose Intolerant",
                Description = "Cannot consume dairy products"
            },
            new ()
            {
                Id = 2,
                Name = "Gluten Allergy",
                Description = "Cannot consume products with gluten in it"
            },
            new ()
            {
                Id = 3,
                Name = "Vegetarian",
                Description = "Cannot consume meat or dairy products"
            },
            new ()
            {
                Id = 4,
                Name = "Vegan",
                Description = "Cannot consume animal products"
            },
            new ()
            {
                Id = 5,
                Name = "Non Alcoholic",
                Description = "Cannot or won't Drink alcohol"
            }
        });
        //Act

        var result = await _accountController.Register() as ViewResult;
        var model = result?.Model as RegisterViewModel;
        var diets = result?.ViewData["diets"] as SelectList;

        //Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(nameof(_accountController.Register), result.ViewName);
        Assert.NotNull(result.ViewData["diets"]);
        Assert.NotNull(diets);
        Assert.Equal(5, diets.Count());
    }
}
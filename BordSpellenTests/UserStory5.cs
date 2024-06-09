using System.Security.Claims;
using BordSpellen.Controllers;
using BordSpellenTests.Util;
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

public class UserStory5
{
    private readonly GameEventController _gameEventController;
    private readonly Mock<IGameEventRepository> _gameEventRepository;
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

    public UserStory5()
    {
        _gameEventRepository = new Mock<IGameEventRepository>();
        var personRepository = new Mock<IPersonRepository>();
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
            personRepository.Object,
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
    public async Task US5_01_GameEventHasBoardGames()
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
        Assert.Equal(2, model.BoardGameEvents?.Count);
    }
    
    [Fact]
    public async Task US5_02_BoardGamesHasDetailProperties()
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
        Assert.Equal(typeof(BoardGame), model.BoardGameEvents?[0].BoardGame.GetType());
    }
}
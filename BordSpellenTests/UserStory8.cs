using System.Security.Claims;
using BordSpellen.Controllers;
using BordSpellen.Models;
using BordSpellenTests.Util;
using Core.Domain.Data.Actors;
using Core.Domain.Data.Entities;
using Core.Domain.Data.Link;
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

public class UserStory8
{
    private readonly ReviewDatabaseRepository _reviewDatabaseRepository;
    private readonly GameEventDatabaseRepository _gameEventDatabaseRepository;
    private readonly ReviewController _reviewController;
    private readonly Mock<IReviewRepository> _mock;
    private readonly Mock<IPersonRepository> _personRepository;
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

    #region Reviews

    private Review _review1 = new()
    {
        Rating = 4,
        ReviewDescription = "It was a good evening. Overall had a great time.",
        EventId = 1,
        ReviewPosterId = 2
    };

    private Review _review2 = new()
    {
        Rating = 3,
        ReviewDescription =
            "Great evening but, i had a run in with other people. Wish that could be resolved next time",
        EventId = 1,
        ReviewPosterId = 1
    };

    private Review _review3 = new()
    {
        Rating = 2,
        ReviewDescription = "Had a Great time. Next time we need to play Cards against Humanity",
        EventId = 1,
        ReviewPosterId = 3
    };
    
    private Review _review4 = new()
    {
        Rating = 1,
        ReviewDescription = "It was a good evening. Overall had a great time.",
        EventId = 2,
        ReviewPosterId = 2
    };

    private Review _review5 = new()
    {
        Rating = 1,
        ReviewDescription =
            "Great evening but, i had a run in with other people. Wish that could be resolved next time",
        EventId = 2,
        ReviewPosterId = 1
    };

    private Review _review6 = new()
    {
        Rating = 3,
        ReviewDescription = "Had a Great time. Next time we need to play Cards against Humanity",
        EventId = 2,
        ReviewPosterId = 3
    };

    #endregion
    
    #region Persons
        private readonly Person _person2 = new()
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
        OrganiserId = 1,
        GamerGameEvents = new List<PersonGameEvents>
        {
            new PersonGameEvents
            {
                GameEventId = 1,
                PersonId = 2
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
        IsAdultEvent = false,
        EventDate = new DateTime(2022, 10, 30),
        MaxAmountOfPlayers = 10,
        OrganiserId = 1
    };
    
    private readonly GameEvent _gameEvent3 = new()
    {
        Id  = 3,
        Name = "Cool Game Event",
        Description = "This is a cool game event",
        Street = "Vogelven",
        HouseNumber = "45",
        PostalCode = "4631KH",
        City = "Hoogerheide",
        IsAdultEvent = false,
        EventDate = new DateTime(2022, 10, 08),
        MaxAmountOfPlayers = 10,
        OrganiserId = 1,
        GamerGameEvents = new List<PersonGameEvents>
        {
            new PersonGameEvents
            {
                GameEventId = 3,
                PersonId = 2
            }
        }
    };

    #endregion

    public UserStory8()
    {
        DbContextOptionsBuilder<EntityDatabaseContext> dbOptions = new DbContextOptionsBuilder<EntityDatabaseContext>()
            .UseInMemoryDatabase(
                Guid.NewGuid().ToString()
            );
        
        var user = new Mock<ClaimsPrincipal>(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, "1")
        }));
        
        ITempDataProvider tempDataProvider = Mock.Of<ITempDataProvider>();
        TempDataDictionaryFactory tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
        ITempDataDictionary tempData = tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
        
        var context = new EntityDatabaseContext(dbOptions.Options);
        _reviewDatabaseRepository = new ReviewDatabaseRepository(context);
        _gameEventDatabaseRepository = new GameEventDatabaseRepository(context);

        _personRepository = new Mock<IPersonRepository>();
        _gameEventRepository = new Mock<IGameEventRepository>();
        _mock = new Mock<IReviewRepository>();
        
        var userManager = Helper.MockUserManager(_users);
        userManager.Setup(x => x.GetUserAsync(user.Object)).ReturnsAsync(_users[0]);
        userManager.Setup(x => x.GetEmailAsync(_users[0])).ReturnsAsync(_users[0].Email);
        
        _reviewController = new ReviewController(
            _mock.Object,
            _personRepository.Object,
            _gameEventRepository.Object,
            userManager.Object)
        {
            TempData = tempData
        };
        
        _reviewController.ControllerContext = new ControllerContext
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
        await _reviewDatabaseRepository.Create(_review1);
        await _reviewDatabaseRepository.Create(_review2);
        await _reviewDatabaseRepository.Create(_review3);
        await _reviewDatabaseRepository.Create(_review4);
        await _reviewDatabaseRepository.Create(_review5);
        await _reviewDatabaseRepository.Create(_review6);
        
        await _gameEventDatabaseRepository.Create(_gameEvent1);
        await _gameEventDatabaseRepository.Create(_gameEvent2);
        await _gameEventDatabaseRepository.Create(_gameEvent3);
    }
    
    [Fact]
    public void US8_01_ReviewHasProperties()
    {
        //Arrange
        
        //Act
        
        //Assert
        Assert.NotNull(typeof(Review).GetProperty("Rating"));
        Assert.NotNull(typeof(Review).GetProperty("ReviewDescription"));
    }
    
    [Fact]
    public void US8_02_ReviewHasPersonReference()
    {
        //Arrange
        var review = new Review
        {
            EventId = 1,
            Rating = 5,
            ReviewDescription = "This is a cool event",
            ReviewPosterId = 1,
            Person = new ()
            {
                Id = 1,
                FirstName = "Thomas",
                LastName = "Terlaak",
                Email = "to.terlaak@outlook.com",
                Gender = GenderEnum.MALE,
                DateOfBirth = new DateTime(1999, 11,8),
                Street = "Vogelven",
                HouseNumber = "16",
                PostalCode = "4631MP",
                City = "Hoogerheide",
                PhoneNumber = "06-123456789"
            }
        };

        //Act
        
        //Assert
        Assert.NotNull(review.Person);
        Assert.Equal("Thomas", review.Person.FirstName);
    }
    
    [Fact]
    public async Task US8_02_OrganiserHasAverageRating()
    {
        //Arrange
        var person = new Person
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

        //Act
        var result = await _reviewDatabaseRepository.GetAverageOrganiserRating(person);

        //Assert
        Assert.Equal(2, result);
    }
    
    [Fact]
    public async Task US8_03_CreateReviewCannotAccess()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetById(1, g => g.GamerGameEvents!))
            .ReturnsAsync(_gameEvent1);

        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>())).ReturnsAsync(_person2);

        //Act
        var result = await _reviewController.Create(1) as ViewResult;

        //Assert
        Assert.Equal("CannotAccess", result?.ViewName);
    }
    
    [Fact]
    public async Task US8_04_CreateReviewPage()
    {
        //Arrange
        _gameEventRepository.Setup(x => x.GetById(3, g => g.GamerGameEvents!))
            .ReturnsAsync(_gameEvent3);

        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>())).ReturnsAsync(_person2);

        //Act
        var result = await _reviewController.Create(3) as ViewResult;

        //Assert
        Assert.Equal(nameof(_reviewController.Create), result?.ViewName);
    }
    
    [Fact]
    public async Task US8_05_CreateReview()
    {
        //Arrange

        var review = new ReviewViewModel
        {
            Rating = 3,
            ReviewDescription = "Tester"
        };

        var reviewDomain = new Review
        {
            Rating = review.Rating,
            ReviewDescription = review.ReviewDescription,
            ReviewPosterId = _person2.Id
        };
        
        _gameEventRepository.Setup(x => x.GetById(3))
            .ReturnsAsync(_gameEvent3);
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>())).ReturnsAsync(_person2);

        _mock.Setup(x => x.CreateReview(It.IsAny<ReviewDto>(), _person2, _gameEvent3))
            .ReturnsAsync(reviewDomain);

        //Act
        var result = await _reviewController.Create(3, review) as RedirectToActionResult;
        
        //Assert
        Assert.Equal(nameof(_reviewController.Index), result?.ActionName);
    }
    
    [Fact]
    public async Task US8_06_CreateReviewInvalidModel()
    {
        //Arrange

        var review = new ReviewViewModel
        {
            Rating = 100,
            ReviewDescription = "Tester"
        };
        
        _reviewController.ValidateModel(review);

        var reviewDomain = new Review
        {
            Rating = review.Rating,
            ReviewDescription = review.ReviewDescription,
            ReviewPosterId = _person2.Id
        };
        
        _gameEventRepository.Setup(x => x.GetById(3))
            .ReturnsAsync(_gameEvent3);
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>())).ReturnsAsync(_person2);

        _mock.Setup(x => x.CreateReview(It.IsAny<ReviewDto>(), _person2, _gameEvent3))
            .ReturnsAsync(reviewDomain);

        //Act
        var result = await _reviewController.Create(3, review) as ViewResult;
        
        //Assert
        Assert.Equal(nameof(_reviewController.Create), result?.ViewName);
        Assert.False(result?.ViewData.ModelState.IsValid);
    }
    
    [Fact]
    public async Task US8_07_CreateReviewGameEventNotFound()
    {
        //Arrange

        var review = new ReviewViewModel
        {
            Rating = 3,
            ReviewDescription = "Tester"
        };

        _reviewController.ValidateModel(review);
        
        var reviewDomain = new Review
        {
            Rating = review.Rating,
            ReviewDescription = review.ReviewDescription,
            ReviewPosterId = _person2.Id
        };
        
        _gameEventRepository.Setup(x => x.GetById(3))
            .ReturnsAsync((GameEvent?) null);
        _personRepository.Setup(x => x.GetOneByEmail(It.IsAny<string>())).ReturnsAsync(_person2);

        _mock.Setup(x => x.CreateReview(It.IsAny<ReviewDto>(), _person2, _gameEvent3))
            .ReturnsAsync(reviewDomain);

        //Act
        var result = await _reviewController.Create(3, review) as ViewResult;
        
        //Assert
        Assert.Equal("NotFound", result?.ViewName);
    }
}
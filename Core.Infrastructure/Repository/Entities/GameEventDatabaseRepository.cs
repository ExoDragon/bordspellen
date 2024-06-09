using Core.Domain.Data.Actors;
using Core.Domain.Data.Entities;
using Core.Domain.Data.Link;
using Core.Domain.Dto.Entities;
using Core.Infrastructure.Context;
using Core.Infrastructure.Repository.Generic;
using Core.Repositories.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository.Entities;

public class GameEventDatabaseRepository : GenericDatabaseRepository<GameEvent, GameEventDto>, IGameEventRepository
{
    public GameEventDatabaseRepository(EntityDatabaseContext dbContext) : base(dbContext)
    {
    }

    public override async Task<GameEvent?> GetById(int id)
    {
        var gameEventDetail = await DbContext.GameEvents
            .Include(o => o.Organiser!)
            .Include(b => b.BoardGameEvents!).ThenInclude(bg => bg.BoardGame)
            .Include(d => d.AvailableFoodTypes!).ThenInclude(gd => gd.Diet)
            .Include(p => p.GamerGameEvents!).ThenInclude(gg => gg.Person)
            .Include(r => r.ReviewsRecieved!).ThenInclude(rp => rp.Person)
            .FirstOrDefaultAsync(g => g.Id == id);
            
        return gameEventDetail!;
    }
    public override async Task<GameEvent> Create(GameEventDto entity)
    {
        var transferGameEvent = new GameEvent
        {
            Name = entity.Name,
            Description = entity.Description,
            Street = entity.Street,
            HouseNumber = entity.HouseNumber,
            PostalCode = entity.PostalCode,
            City = entity.City,
            IsAdultEvent = entity.IsAdultEvent,
            EventDate = entity.EventDate,
            MaxAmountOfPlayers = entity.MaxAmountOfPlayers,
            Organiser = entity.Organiser
        };

        var newGameEvent = await Create(transferGameEvent);
        await DbContext.SaveChangesAsync();

        if (entity.BoardGameIds != null)
        {
            foreach (var boardGameId in entity.BoardGameIds)
            {
                var newBoardGameEvent = new BoardGameEvents
                {
                    GameEventId = newGameEvent.Id,
                    BoardGameId = boardGameId
                };

                await DbContext.Set<BoardGameEvents>().AddAsync(newBoardGameEvent);
            }
            
            foreach (var boardGameId in entity.BoardGameIds)
            {
                var boardGame = await DbContext.BoardGames.FirstOrDefaultAsync(b => b.Id == boardGameId);
                if (boardGame is {HasAgeRestriction: true})
                {
                    newGameEvent.IsAdultEvent = true;
                }
            }
        }

        if (entity.DietIds != null)
        {
            foreach (var dietId in entity.DietIds)
            {
                var newDietGameEvent = new GameEventDiets
                {
                    GameEventId = newGameEvent.Id,
                    DietId = dietId
                };

                await DbContext.Set<GameEventDiets>().AddAsync(newDietGameEvent);
            }
        }

        await DbContext.SaveChangesAsync();
        return newGameEvent;
    }
    public override async Task<GameEvent> Update(GameEventDto entity)
    {
        var dbGameEvent = await DbContext.GameEvents.FirstOrDefaultAsync(g => g.Id == entity.Id);

        if (dbGameEvent == null) return null!;

        dbGameEvent.Name = entity.Name;
        dbGameEvent.Description = entity.Description;
        dbGameEvent.Street = entity.Street;
        dbGameEvent.HouseNumber = entity.HouseNumber;
        dbGameEvent.PostalCode = entity.PostalCode;
        dbGameEvent.City = entity.City;
        dbGameEvent.IsAdultEvent = entity.IsAdultEvent;
        dbGameEvent.EventDate = entity.EventDate;
        dbGameEvent.MaxAmountOfPlayers = entity.MaxAmountOfPlayers;
        await DbContext.SaveChangesAsync();

        var existingBoardGames = DbContext.BoardGameEvents.Where(bge => bge.GameEventId == dbGameEvent.Id);
        DbContext.BoardGameEvents.RemoveRange(existingBoardGames);
        await DbContext.SaveChangesAsync();

        var existingDiet = DbContext.GameEventDiets.Where(bge => bge.GameEventId == dbGameEvent.Id);
        DbContext.GameEventDiets.RemoveRange(existingDiet);
        await DbContext.SaveChangesAsync();


        if (entity.BoardGameIds != null)
        {
            foreach (var boardGameId in entity.BoardGameIds)
            {
                var newBoardGameEvent = new BoardGameEvents
                {
                    GameEventId = dbGameEvent.Id,
                    BoardGameId = boardGameId
                };

                await DbContext.Set<BoardGameEvents>().AddAsync(newBoardGameEvent);
            }

            foreach (var boardGameId in entity.BoardGameIds)
            {
                var boardGame = await DbContext.BoardGames.FirstOrDefaultAsync(b => b.Id == boardGameId);
                if (boardGame!.HasAgeRestriction)
                {
                    dbGameEvent.IsAdultEvent = true;
                }
            }
        }

        if (entity.DietIds != null)
        {
            foreach (var dietId in entity.DietIds)
            {
                var newDietGameEvent = new GameEventDiets
                {
                    GameEventId = dbGameEvent.Id,
                    DietId = dietId
                };

                await DbContext.Set<GameEventDiets>().AddAsync(newDietGameEvent);
            }
        }

        await DbContext.SaveChangesAsync();
        return dbGameEvent;
    }
    public async Task<IEnumerable<GameEvent>> GetReviewedGameEvents(Person person)
    {
        List<GameEvent> gameEventsList = new List<GameEvent>();

        var reviews = await DbContext.Set<Review>().Where(r => r.ReviewPosterId == person.Id).ToListAsync();

        if (reviews.Count == 0)
        {
            return gameEventsList;
        }

        foreach (var review in reviews)
        {
            var addGameEvent = await DbContext.GameEvents
                .Include(g => g.GamerGameEvents)
                .FirstOrDefaultAsync(g => g.Id == review.EventId);
            
            if (addGameEvent != null)
            {
                gameEventsList.Add(addGameEvent);
            }
        }
        return gameEventsList;
    }
    public async Task<bool> Subscribe(GameEvent entity, Person person)
    {
        var existingSubscription =  await DbContext.PersonGameEvents.FirstOrDefaultAsync(
            pg =>  pg.GameEventId == entity.Id && pg.PersonId == person.Id);

        if (existingSubscription != null)
        {
            return await Unsubscribe(existingSubscription);
        }

        var subscription = new PersonGameEvents
        {
            GameEventId = entity.Id,
            PersonId = person.Id
        };

        await DbContext.Set<PersonGameEvents>().AddAsync(subscription);
        await DbContext.SaveChangesAsync();
        return true;
    }
    public async Task<bool> Unsubscribe(PersonGameEvents entity)
    {
        DbContext.PersonGameEvents.Remove(entity);
        await DbContext.SaveChangesAsync();
        
        return false;
    }
    public async Task<IEnumerable<GameEvent>> GetSubscribedEvents(Person person)
    {
        var personGameEvents = await DbContext.GameEvents
            .Include(b => b.BoardGameEvents!).ThenInclude(bg => bg.BoardGame)
            .Include(d => d.AvailableFoodTypes!).ThenInclude(gd => gd.Diet)
            .Include(p => p.GamerGameEvents!).ThenInclude(gg => gg.Person)
            .Include(r => r.ReviewsRecieved!).ThenInclude(rp => rp.Person).ToListAsync();

        var result = personGameEvents.Where(ge => ge.GamerGameEvents!.Any(gge => gge.PersonId == person.Id));
        return result;
    }
    public async Task<bool> CheckFoodAvailability(GameEvent entity, Person person)
    {
        var gameEvent = await DbContext.GameEvents
            .Include(d => d.AvailableFoodTypes!).ThenInclude(gd => gd.Diet)
            .FirstOrDefaultAsync(g => g.Id == entity.Id);

        var personEntity = await DbContext.Persons
            .Include(p => p.DietList)!.ThenInclude(pd => pd.Diet)
            .FirstOrDefaultAsync(p => p.Id == person.Id);

        if (gameEvent == null || 
            personEntity == null || 
            gameEvent.AvailableFoodTypes == null)
        {
            return false;
        }

        return person.DietList!.All(
            personDiet => gameEvent.AvailableFoodTypes.Exists(
                gd => gd.DietId == personDiet.DietId));
    }
    public async Task<bool> CheckSubscription(GameEvent gameEvent, Person person)
    {
        var subscriptions = await GetSubscribedEvents(person);
        var result = subscriptions.Where(gge => gge.EventDate!.Value.Date == gameEvent.EventDate!.Value.Date);
        return result.Any();
    }
}
using Core.Domain;
using Core.Domain.Data.Actors;
using Core.Domain.Data.Entities;
using Core.Domain.Data.Link;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Core.Infrastructure.Context;

public class EntityDatabaseContext : DbContext
{
    //Actors
    public DbSet<Person> Persons { get; set; }
    //public DbSet<Gamer> Gamers { get; set; }

    //Entities
    public DbSet<BoardGame> BoardGames { get; set; }
    public DbSet<GameEvent> GameEvents { get; set; }
    public DbSet<BoardGameEvents> BoardGameEvents { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Diet> Diets { get; set; }
    public DbSet<PersonDiets> PersonDiets { get; set; }
    public DbSet<GameEventDiets> GameEventDiets { get; set; }
    public DbSet<PersonGameEvents> PersonGameEvents { get; set; }

#pragma warning disable CS8618
    public EntityDatabaseContext(DbContextOptions<EntityDatabaseContext> contextOptions) : base(contextOptions)
#pragma warning restore CS8618
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Many to Many Relation BoardGames & Events
        modelBuilder.Entity<BoardGameEvents>().HasKey(be => new
        {
            be.BoardGameId, be.GameEventId
        });

        //BoardGame GameEvent Many To Many Relation
        modelBuilder.Entity<BoardGameEvents>()
            .HasOne(b => b.BoardGame)
            .WithMany(be => be.BoardGameEvents)
            .HasForeignKey(be => be.BoardGameId);
        
        modelBuilder.Entity<BoardGameEvents>()
            .HasOne(b => b.GameEvent)
            .WithMany(be => be.BoardGameEvents)
            .HasForeignKey(be => be.GameEventId);

        //Person Diet Many To Many Relation
        modelBuilder.Entity<PersonDiets>()
            .HasOne(pd => pd.Person)
            .WithMany(p => p.DietList)
            .HasForeignKey(pd => pd.PersonId);
        
        modelBuilder.Entity<PersonDiets>()
            .HasOne(pd => pd.Diet)
            .WithMany(d => d.Persons)
            .HasForeignKey(pd => pd.DietId);
        
        //GameEvent Diet Many To Many Relation
        modelBuilder.Entity<GameEventDiets>()
            .HasOne(pd => pd.GameEvent)
            .WithMany(p => p.AvailableFoodTypes)
            .HasForeignKey(pd => pd.GameEventId);
        
        modelBuilder.Entity<GameEventDiets>()
            .HasOne(pd => pd.Diet)
            .WithMany(d => d.GameEventDiets)
            .HasForeignKey(pd => pd.DietId);
        
        //Person GameEvents Many To Many Relation
        modelBuilder.Entity<PersonGameEvents>()
            .HasOne(pg => pg.Person)
            .WithMany(p => p.PersonGameEvents)
            .HasForeignKey(pg => pg.PersonId)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<PersonGameEvents>()
            .HasOne(pg => pg.GameEvent)
            .WithMany(g => g.GamerGameEvents)
            .HasForeignKey(pg => pg.GameEventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        //Review Person Reletions
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Person)
            .WithMany(p => p.ReviewsPosted)
            .HasForeignKey(r => r.ReviewPosterId)
            .OnDelete(DeleteBehavior.ClientSetNull);
        
        modelBuilder.Entity<Review>()
            .HasOne(r => r.GameEvent)
            .WithMany(p => p.ReviewsRecieved)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
    

    //Update or add date to entity on submit.
    public override int SaveChanges()
    {
        IEnumerable<EntityEntry> entries = ChangeTracker.Entries().Where(
                entry => entry.Entity is Entity && (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            );
        
        foreach (EntityEntry entityEntry in entries)
        {
            Entity entity = (Entity) entityEntry.Entity;
            entity.DateUpdated = DateTime.Now;
            entity.DateCreated = entityEntry.State == EntityState.Added ? DateTime.Now : entity.DateCreated;
        }
        
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        IEnumerable<EntityEntry> entries = ChangeTracker.Entries().Where(
            entry => entry.Entity is Entity && (entry.State == EntityState.Added || entry.State == EntityState.Modified)
        );
        
        foreach (EntityEntry entityEntry in entries)
        {
            Entity entity = (Entity) entityEntry.Entity;
            entity.DateUpdated = DateTime.Now;
            entity.DateCreated = entityEntry.State == EntityState.Added ? DateTime.Now : entity.DateCreated;
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}
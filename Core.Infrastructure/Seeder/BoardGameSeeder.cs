using Core.Domain.Data.Entities;
using Core.Domain.Enums;
using Core.Infrastructure.Context;
using Core.Infrastructure.Seeder.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Seeder;

public static class BoardGameSeeder
{
    public static async Task Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<EntityDatabaseContext>();
            if (context == null) return;

            await context.Database.EnsureCreatedAsync();

            if (!context.BoardGames.Any())
            {
                context.BoardGames.AddRange(new List<BoardGame>
                {
                    new ()
                    {
                        Name = "King of the Valley",
                        Description = "A long time ago in a land far, far away, there was a thriving valley with various inhabitants. They wandered aimlessly in need of a true leader to guide them. Are you the king who will lead them the way? Do you have a strategy to acquire their trust, and are you cunning enough to challenge other would-be kings? They cannot be trusted as they will do anything to claim the crown you deserve. No sacrifice is too big or small for them. They may think being king is as easy as summoning subjects to their side, squeezing the gold from their pockets through taxes, and arranging some marriages, but only a true king will be able to keep up the morale and lead the inhabitants through the ever-changing circumstances in the valley and the hills.",
                        Genre = BoardGameGenre.EUROGAME,
                        HasAgeRestriction = false,
                        Image = BoardGameImageConverter.ConvertImageToBytes(Path.GetFullPath("wwwroot") + "/Images/King-of-the-valley-cover.jpg"),
                        ImageFormat = "image/jpeg",
                        Type = BoardGameType.BOARD
                    },
                    new ()
                    {
                        Name = "Warhammer Underworlds",
                        Description = "This is a description for Warhammer Underworlds",
                        Genre = BoardGameGenre.MINIATURE,
                        HasAgeRestriction = true,
                        Image = BoardGameImageConverter.ConvertImageToBytes(Path.GetFullPath("wwwroot") + "/Images/games-workshop-warhammer-underworlds-starter-set.jpg"),
                        ImageFormat = "image/jpeg",
                        Type = BoardGameType.BOARD
                    },
                    new ()
                    {
                        Name = "Magic - The Gathering",
                        Description = "This is a description for Magic - The Gathering",
                        Genre = BoardGameGenre.DECKBUILDING,
                        HasAgeRestriction = true,
                        Image = BoardGameImageConverter.ConvertImageToBytes(Path.GetFullPath("wwwroot") + "/Images/71ifCaWlPoL.jpg"),
                        ImageFormat = "image/jpeg",
                        Type = BoardGameType.CARD
                    },
                    new ()
                    {
                        Name = "Risk",
                        Description = "Risk is a area control game",
                        Genre = BoardGameGenre.AREACONTROL,
                        HasAgeRestriction = false,
                        Image = BoardGameImageConverter.ConvertImageToBytes(Path.GetFullPath("wwwroot") + "/Images/Risk.jpg"),
                        ImageFormat = "image/jpeg",
                        Type = BoardGameType.BOARD
                    },
                    new ()
                    {
                        Name = "Stardew valley",
                        Description = "A cooperative board game of farming and friendship based on the Stardew Valley video game by Eric Barone. Work together with your fellow farmers to save the Valley from the nefarious JojaMart Corporation! To do this, you'll need to farm, fish, friend and find all kinds of different resources to fulfill your Grandpa's Goals and restore the Community Center. Collect all kinds of items, raise animals, and explore the Mine. Gain powerful upgrades and skills and as the seasons pass see if you're able to protect the magic of Stardew Valley!",
                        Genre = BoardGameGenre.TILEPLACEMENT,
                        HasAgeRestriction = false,
                        Image = BoardGameImageConverter.ConvertImageToBytes(Path.GetFullPath("wwwroot") + "/Images/Stardew Valley.jpg"),
                        ImageFormat = "image/jpeg",
                        Type = BoardGameType.COOPERATIVE
                    },
                    new ()
                    {
                        Name = "Poker",
                        Description = "Poker is a family of comparing card games in which players wager over which hand is best according to that specific game's rules. While the earliest known form of the game was played with just 20 cards, today it is usually played with a standard deck, although in countries where short packs are common, it may be played with 32, 40 or 48 cards.",
                        Genre = BoardGameGenre.BLUFFING,
                        HasAgeRestriction = true,
                        Image = BoardGameImageConverter.ConvertImageToBytes(Path.GetFullPath("wwwroot") + "/Images/Poker.jpg"),
                        ImageFormat = "image/jpeg",
                        Type = BoardGameType.CARD
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
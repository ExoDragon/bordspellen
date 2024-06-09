using Core.Domain.Security;
using Core.Domain.Security.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Seeder;

public static class IdentitySeeder
{
    public static async Task Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            //Seed Roles
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(UserRoles.Organiser))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Organiser));

            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            //Seed Users
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<BoardGameUser>>();

            string gamerJaUserEmail = "ja.kantje@live.com";
            var gamerJaUser = await userManager.FindByEmailAsync(gamerJaUserEmail);
            if (gamerJaUser == null)
            {
                var newGamerJaUser = new BoardGameUser
                {
                    FullName = "Jantje Kantje",
                    UserName = "JantjeKantje",
                    Email = gamerJaUserEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newGamerJaUser, "GB3mh@vjXQjX8J");
                await userManager.AddToRoleAsync(newGamerJaUser, UserRoles.User);
            }
            
            string gamerWaUserEmail = "wa.terlaak@outlook.com";
            var gamerWaUser = await userManager.FindByEmailAsync(gamerWaUserEmail);
            if (gamerWaUser == null)
            {
                var newGamerWaUser = new BoardGameUser
                {
                    FullName = "Wouter Terlaak",
                    UserName = "WouterTerlaak",
                    Email = gamerWaUserEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newGamerWaUser, "sG&JQ5zm%fxo3J");
                await userManager.AddToRoleAsync(newGamerWaUser, UserRoles.User);
            }
            
            string gamerTaUserEmail = "to.anders@outlook.com";
            var gamerTaUser = await userManager.FindByEmailAsync(gamerTaUserEmail);
            if (gamerTaUser == null)
            {
                var newGamerToUser = new BoardGameUser
                {
                    FullName = "Thomas Anders",
                    UserName = "ThomasAnders",
                    Email = gamerTaUserEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newGamerToUser, "SU&%e6MPKGK@ij");
                await userManager.AddToRoleAsync(newGamerToUser, UserRoles.Organiser);
            }
            
            string gamerToUserEmail = "to.terlaak@outlook.com";
            var gamerToUser = await userManager.FindByEmailAsync(gamerToUserEmail);
            if (gamerToUser == null)
            {
                var newGamerToUser = new BoardGameUser
                {
                    FullName = "Thomas Terlaak",
                    UserName = "ThomasTerlaak",
                    Email = gamerToUserEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newGamerToUser, "SU&%e6MPKGK@ij");
                await userManager.AddToRoleAsync(newGamerToUser, UserRoles.Organiser);
            }
        }
    }
}
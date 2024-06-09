using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BordSpellenApi.Models;
using Core.Domain.Security;
using Core.Domain.Security.Roles;
using Core.Infrastructure.Helper;
using Core.Repositories.Data.Actors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BordSpellenApi.Controllers;

[Route("api/v1/account")]
[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
public class AccountController : Controller
{
    private readonly UserManager<BoardGameUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IPersonRepository _personRepository;

    public AccountController(
        UserManager<BoardGameUser> userManager,
        IConfiguration configuration,
        IPersonRepository personRepository)
    {
        _userManager = userManager;
        _configuration = configuration;
        _personRepository = personRepository;
    }
    
    [HttpPost]
    [Route("/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Login(LoginModel loginModel)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        BoardGameUser user = await _userManager.FindByEmailAsync(loginModel.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
        {
            return new UnauthorizedResult();
        }

        IList<Claim> claims = await _userManager.GetClaimsAsync(user);
        var role = await _userManager.GetRolesAsync(user);
        JwtSecurityToken token = CreateToken(user.Email, role[0], claims);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }
    
    [HttpPost]
    [Route("/register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Register(RegisterModel registerModel)
    {
        if (!ModelState.IsValid) return BadRequest(registerModel);
        var user = await _userManager.FindByEmailAsync(registerModel.Email);
        if (user != null)
        {
            return BadRequest("User already exists");
        }
        
        DateTime date = registerModel.DateOfBirth ?? DateTime.Today;
        if (Helper.Age(date) < 16)
        {
            return BadRequest("You need to be 16 or older");
        }

        var personDto = registerModel.ToDto(registerModel);
        var newIdentityUser = new BoardGameUser
        {
            FullName = personDto.FirstName + personDto.LastName,
            Email = registerModel.Email,
            UserName = registerModel.Email,
            EmailConfirmed = true
        };

        var newIdentityUserResponse = await _userManager.CreateAsync(newIdentityUser, registerModel.Password);
        if (!newIdentityUserResponse.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        Claim claim = new Claim(UserRoles.User, "true");
        IList<Claim> claims = new List<Claim> { claim };
        
        await _userManager.AddClaimAsync(newIdentityUser, new Claim(UserRoles.User, "true"));
        await _userManager.AddToRoleAsync(newIdentityUser, UserRoles.User);
        
        var resultUser = await _userManager.FindByEmailAsync(newIdentityUser.Email);
        var role = await _userManager.GetRolesAsync(resultUser);
        var result = await _personRepository.Create(personDto);
        if (result == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        JwtSecurityToken token = CreateToken(result.Email!, role[0], claims);
        
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }
    
    private JwtSecurityToken CreateToken(string username, string role, IList<Claim> claims)
    {
        claims.Add(new Claim(ClaimTypes.Name, username));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(ClaimTypes.Role, role));

        SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        JwtSecurityToken token = new JwtSecurityToken(
            expires: DateTime.Now.AddDays(7),
            claims: claims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}
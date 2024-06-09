using BordSpellen.Models;
using BordSpellen.Models.DtoModels;
using Core.Domain.Security;
using Core.Domain.Security.Roles;
using Core.Infrastructure.Helper;
using Core.Repositories.Data.Actors;
using Core.Repositories.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BordSpellen.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{

    private readonly UserManager<BoardGameUser> _userManager;
    private readonly SignInManager<BoardGameUser> _signInManager;
    private readonly IPersonRepository _personRepository;
    private readonly IDietRepository _dietRepository;

    public AccountController(
        UserManager<BoardGameUser> userManager,
        SignInManager<BoardGameUser> signInManager,
        IPersonRepository personRepository,
        IDietRepository dietRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _personRepository = personRepository;
        _dietRepository = dietRepository;
    }
    
    // GET
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
            
        var user = await _userManager.FindByEmailAsync(viewModel.EmailAddress);
        if (user != null)
        {
            var passwordCheck = await _userManager.CheckPasswordAsync(user, viewModel.Password);
            if (passwordCheck)
            {
                var result = await _signInManager.PasswordSignInAsync(user, viewModel.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
                
            TempData["Error"] = "Wrong Credentials. Please, try again!";
            return View(viewModel);
        }

        TempData["Error"] = "User doesn't exist. Please, try again!";
        return View(viewModel);
    }

    public async Task<IActionResult> Register()
    {
        var dropdownData = await GetDropdownViewModel();
        ViewBag.diets = new SelectList(dropdownData.Diets, "Id", "Name");
        
        return View(nameof(Register), new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel viewModel)
    {
        RegisterDropdownViewModel? dropdownData;
        if (!ModelState.IsValid)
        {
            dropdownData = await GetDropdownViewModel();
            ViewBag.diets = new SelectList(dropdownData.Diets, "Id", "Name");
            
            return View(viewModel);
        }
        
        var user = await _userManager.FindByEmailAsync(viewModel.Email);
        if (user != null)
        {
            TempData["Error"] = "This email address is already in use";
            dropdownData = await GetDropdownViewModel();
            ViewBag.diets = new SelectList(dropdownData.Diets, "Id", "Name");
            
            return View(nameof(Register), viewModel);
        }
        
        DateTime date = viewModel.DateOfBirth ?? DateTime.Today;
        if (Helper.Age(date) < 16)
        {
            TempData["Error"] = "You must be 16 years or older to sign up";
            dropdownData = await GetDropdownViewModel();
            ViewBag.diets = new SelectList(dropdownData.Diets, "Id", "Name");
            return View(nameof(Register),viewModel);
        }

        var personUser = viewModel.ToDto(viewModel);
        var newIdentityUser = new BoardGameUser
        {
            FullName = personUser.FirstName + personUser.LastName,
            Email = viewModel.Email,
            UserName = viewModel.Email,
            EmailConfirmed = true
        };

        var newIdentityUserResponse = await _userManager.CreateAsync(newIdentityUser, viewModel.Password);
        if (newIdentityUserResponse.Succeeded)
        {
            await _userManager.AddToRoleAsync(newIdentityUser, UserRoles.User);
            await _personRepository.Create(personUser);

            var result = await _signInManager.PasswordSignInAsync(newIdentityUser, viewModel.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            
            TempData["Error"] = "Account created but we couldn't log you in. Try again later!";
            return View("Login");
        }

        List<IdentityError> errorList = newIdentityUserResponse.Errors.ToList();
        var errors = string.Join(", ", errorList.Select(e => e.Description));
        TempData["Error"] = errors;
        
        dropdownData = await GetDropdownViewModel();
        ViewBag.diets = new SelectList(dropdownData.Diets, "Id", "Name");
        return View(nameof(Register), viewModel);
    }
    
    
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> Details(string id)
    {
        var user = _userManager.FindByIdAsync(id);
        var person = await _personRepository.GetOneWithDiet(user.Result.Email);
        if (user == null || person == null ||  String.IsNullOrEmpty(user.Result.Email) || user.Result.Email != person.Email) return View("NotFound");

        return View(person);
    }

    [Authorize]
    public async Task<IActionResult> UpdateUser(int id)
    {
        var person = await _personRepository.GetById(id, p => p.DietList!);
        if (person == null || id != person.Id || String.IsNullOrEmpty(id.ToString())) return View("NotFound");

        var response = new PersonViewModel
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Email = person.Email,
            Gender = person.Gender,
            DateOfBirth = person.DateOfBirth,
            PhoneNumber = person.PhoneNumber,
            Street = person.Street,
            HouseNumber = person.HouseNumber,
            PostalCode = person.PostalCode,
            City = person.City,
            DietPreferences = person.DietList?.Select(d => d.DietId).ToList()
        };
        
        var dropdownData = await GetDropdownViewModel();
        ViewBag.diets = new SelectList(dropdownData.Diets, "Id", "Name");
        
        return View(response);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUser(int id, PersonViewModel viewModel)
    {
        if (id != viewModel.Id || String.IsNullOrEmpty(id.ToString())) return View("NotFound");
        
        if (!ModelState.IsValid)
        {
            var dropdownData = await GetDropdownViewModel();
            ViewBag.diets = new SelectList(dropdownData.Diets, "Id", "Name");
            
            return View(viewModel);
        }
        
        var user = await _userManager.FindByEmailAsync(viewModel.Email);
        var person = await _personRepository.GetById(viewModel.Id);
        if (user == null || person == null || person.Email != viewModel.Email)
        {
            TempData["Error"] = "This user doesn't exist";
            return View(viewModel);
        }
        
        await _personRepository.Update(viewModel.ToDto(viewModel));
        return RedirectToAction("Index", "Home");
    }
    
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var person = await _personRepository.GetById(id);
        if (person == null || id != person.Id || String.IsNullOrEmpty(id.ToString())) return View("NotFound");

        var user = await _userManager.FindByEmailAsync(person.Email);
        if (user == null || user.Email != person.Email) return View("NotFound");
        
        await _signInManager.SignOutAsync();

        await _userManager.DeleteAsync(user);
        await _personRepository.Delete(person);
        
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied(string returnUrl)
    {
        return View();
    }

    public async Task<RegisterDropdownViewModel> GetDropdownViewModel()
    {
        var dropdownData = new RegisterDropdownViewModel();
        
        var diets = await _dietRepository.GetAll();
        dropdownData.Diets = diets.ToList();
        ViewBag.diets = new SelectList(dropdownData.Diets, "Id", "Name");

        return dropdownData;
    }
}
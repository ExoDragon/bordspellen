using BordSpellen.Models;
using Core.Domain.Data.Entities;
using Core.Domain.Security.Roles;
using Core.Repositories.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BordSpellen.Controllers;

[Authorize(Roles = UserRoles.Organiser)]
public class BoardGameController : Controller
{
    private readonly IBoardGameRepository _boardGameRepository;

    public BoardGameController(IBoardGameRepository boardGameRepository)
    {
        _boardGameRepository = boardGameRepository;
    }
    
    // GET
    public async Task<IActionResult> Index()
    {
        var boardgames = await _boardGameRepository.GetAll();
        return View(nameof(Index), boardgames);
    }
    
    public async  Task<IActionResult> Details(int id)
    {
        var boardGame = await _boardGameRepository.GetById(id);
        if (boardGame == null || boardGame.Id != id || String.IsNullOrEmpty(id.ToString())) return View("NotFound");
        
        return View(boardGame);
    }
    public IActionResult Create()
    {
        return View(nameof(Create), new BoardGameViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BoardGameViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var newBoardGame = new BoardGame
        {
            Name = viewModel.Name,
            Description = viewModel.Description,
            Genre = viewModel.Genre,
            HasAgeRestriction = viewModel.HasAgeRestriction,
            ImageFormat = viewModel.Image.ContentType,
            Type = viewModel.Type
        };

        var memoryStream = new MemoryStream();
        await viewModel.Image.CopyToAsync(memoryStream);
        newBoardGame.Image = memoryStream.ToArray();

        await _boardGameRepository.Create(newBoardGame);
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id)
    {
        var boardGameResponse = await _boardGameRepository.GetById(id);
        if (boardGameResponse == null || boardGameResponse.Id != id || String.IsNullOrEmpty(id.ToString()) || id != boardGameResponse.Id)
        {
            return View("NotFound");
        }

        BoardGameViewModel? viewModel;
        if (boardGameResponse.Image != null)
        {
            viewModel = new BoardGameViewModel
            {
                Id = boardGameResponse.Id,
                Name = boardGameResponse.Name,
                Description = boardGameResponse.Description,
                Picture = Convert.ToBase64String(boardGameResponse.Image),
                PictureFormat = boardGameResponse.ImageFormat,
                HasAgeRestriction = boardGameResponse.HasAgeRestriction,
                Genre = boardGameResponse.Genre,
                Type = boardGameResponse.Type
            };
            return View(viewModel);
        }
        
        viewModel = new BoardGameViewModel
        {
            Id = boardGameResponse.Id,
            Name = boardGameResponse.Name,
            Description = boardGameResponse.Description,
            HasAgeRestriction = boardGameResponse.HasAgeRestriction,
            Genre = boardGameResponse.Genre,
            Type = boardGameResponse.Type
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BoardGameViewModel viewModel)
    {
       
        if (id != viewModel.Id || String.IsNullOrEmpty(id.ToString())) return View("NotFound");
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var result = viewModel.HasAgeRestriction;
        
        var newBoardGame = new BoardGame
        {
            Id = viewModel.Id,
            Name = viewModel.Name,
            Description = viewModel.Description,
            HasAgeRestriction = viewModel.HasAgeRestriction,
            ImageFormat = viewModel.Image.ContentType,
            Genre = viewModel.Genre,
            Type = viewModel.Type
        };
        
        var memoryStream = new MemoryStream();
        await viewModel.Image.CopyToAsync(memoryStream);
        newBoardGame.Image = memoryStream.ToArray();
        
        await _boardGameRepository.Update(newBoardGame);
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id)
    {
        var boardGame = await _boardGameRepository.GetById(id);
        if (boardGame == null || boardGame.Id != id || String.IsNullOrEmpty(id.ToString())) return View("NotFound");

        await _boardGameRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}
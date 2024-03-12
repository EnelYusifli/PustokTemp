using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokTemp.DAL;
using PustokTemp.ViewModels;
using System.Diagnostics;

namespace PustokTemp.Controllers;

public class HomeController : Controller
{
    private readonly PustokDbContext _context;

    public HomeController(PustokDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        HomeViewModel homeViewModel = new HomeViewModel()
        {
            Sliders = await _context.Sliders.ToListAsync()
        };
        return View(homeViewModel);
    }
}

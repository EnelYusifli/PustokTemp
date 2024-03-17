using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokTemp.DAL;
using PustokTemp.Models;
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
            Sliders = await _context.Sliders.ToListAsync(),
            Books = await _context.Books.Include(x=>x.Author).Include(x=>x.Genre).Include(x=>x.BookImages).ToListAsync()
        };
        return View(homeViewModel);
    }
    public async Task<IActionResult> Detail(int id)
    {
        Book? book=await _context.Books
            .Include(x=>x.Author)
            .Include(x=>x.Genre)
            .Include(x=>x.BookImages)
            .FirstOrDefaultAsync(x=>x.Id==id);
        return View(book);
    }
}

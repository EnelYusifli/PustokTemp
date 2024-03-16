using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokTemp.Business.Interfaces;
using PustokTemp.CustomExceptions.BookExceptions;
using PustokTemp.DAL;
using PustokTemp.Models;

namespace PustokTemp.Areas.Admin.Controllers;

[Area("Admin")]
public class BookController : Controller
{
    private readonly IBookService _bookService;
    private readonly PustokDbContext _context;

    public BookController(IBookService bookService,PustokDbContext context)
    {
        _bookService = bookService;
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        List<Book> books = await _bookService.GetAllAsync();
        return View(books);
    }
    public IActionResult Create()
    {
        ViewBag.Genres = _context.Genres.ToList();
        ViewBag.Authors = _context.Authors.ToList();
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book)
    {
        ViewBag.Genres = _context.Genres.ToList();
        ViewBag.Authors = _context.Authors.ToList();
        if (!ModelState.IsValid) return View(book);
        try
        {
            await _bookService.CreateAsync(book);
        }
        catch (ImageCannotBeNullException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View(book);
        }
        catch (UnableContentTypeException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View(book);
        }
        catch (MoreThanMaxLengthException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View(book);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(book);
        }
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        ViewBag.Genres = _context.Genres.ToList();
        ViewBag.Authors = _context.Authors.ToList();
        Book? book = await _bookService.GetByIdAsync(id);
        if (book == null) throw new Exception();
        return View(book);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Book book)
    {
        ViewBag.Genres = await _context.Genres.ToListAsync();
        ViewBag.Authors = await _context.Authors.ToListAsync();
        if (!ModelState.IsValid) return View(book);
        try
        {
            await _bookService.UpdateAsync(book);
        }
        catch (ImageCannotBeNullException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View(book);
        }
        catch (UnableContentTypeException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View(book);
        }
        catch (MoreThanMaxLengthException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View(book);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(book);
        }
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _bookService.DeleteAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return NotFound();
        }
    }
}

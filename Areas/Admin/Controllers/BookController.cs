using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokTemp.Business.Implementations;
using PustokTemp.Business.Interfaces;
using PustokTemp.CustomExceptions.BookExceptions;
using PustokTemp.DAL;
using PustokTemp.Extensions;
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
    public IActionResult Index()
    {
        List<Book> books = _context.Books.Include(x => x.BookImages).Include(x => x.Author).Include(x => x.Genre).ToList();
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

    //public IActionResult Update(int id)
    //{
    //    ViewBag.Genres = _context.Genres.ToList();
    //    ViewBag.Authors = _context.Authors.ToList();
    //    Book? book = _context.Books.Include(x=>x.BookImages).FirstOrDefault(x => x.Id == id);
    //    if (book == null) throw new Exception();
    //    return View(book);
    //}
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Update(Book book)
    //{
    //    Book? existBook = await _context.Books
    //        .Include(x => x.BookImages)
    //        .FirstOrDefaultAsync(x => x.Id == book.Id);
    //    if (existBook is null) throw new Exception();
    //    ViewBag.Genres = await _context.Genres.ToListAsync();
    //    ViewBag.Authors = await _context.Authors.ToListAsync();
    //    if (!ModelState.IsValid) return View(book);
    //    if (book.HoverImgFile is not null)
    //    {
    //        if (!(book.HoverImgFile.ContentType == "image/jpeg" || book.HoverImgFile.ContentType == "image/png"))
    //        {
    //            ModelState.AddModelError("HoverImgFile", "Content type must be jpeg or png");
    //            return View();
    //        }
    //        if (book.HoverImgFile.Length > 2097152)
    //        {
    //            ModelState.AddModelError("HoverImgFile", "Size type must be less than 2mb");
    //            return View();
    //        }
    //        var file = await _context.BookImages
    //        .FirstOrDefaultAsync(x => x.BookId == existBook.Id && x.IsPoster == false);
    //        FileExtension.DeleteFile(_env.WebRootPath, "uploads/books", file.Url);
    //        BookImage hoverBookImage = new()
    //        {
    //            Book = book,
    //            Url = book.HoverImgFile.SaveFile(_env.WebRootPath, "uploads/books"),
    //            IsPoster = false
    //        };
    //        await _context.BookImages.AddAsync(hoverBookImage);
    //    }
    //    if(book.PosterImgFile is not null)
    //    {
    //        if (!(book.PosterImgFile.ContentType == "image/jpeg" || book.PosterImgFile.ContentType == "image/png"))
    //        {
    //            ModelState.AddModelError("PosterImgFile", "Content type must be jpeg or png");
    //            return View();
    //        }
    //        if (book.PosterImgFile.Length > 2097152)
    //        {
    //            ModelState.AddModelError("PosterImgFile", "Size type must be less than 2mb");
    //            return View();
    //        }
    //        var file = await _context.BookImages
    //        .FirstOrDefaultAsync(x => x.BookId == existBook.Id && x.IsPoster == false);
    //        FileExtension.DeleteFile(_env.WebRootPath, "uploads/books", file.Url);
    //        BookImage posterBookImage = new()
    //        {
    //            Book = book,
    //            Url = book.PosterImgFile.SaveFile(_env.WebRootPath, "uploads/books"),
    //            IsPoster = true
    //        };
    //        await _context.BookImages.AddAsync(posterBookImage);
    //    }
    //    if (book.DetailImgFiles is not null)
    //    {
    //        foreach (var item in book.DetailImgFiles)
    //        {
    //            if (!(item.ContentType == "image/jpeg" || item.ContentType == "image/png"))
    //            {
    //                ModelState.AddModelError("DetailImgFiles", "Content type must be jpeg or png");
    //                return View();
    //            }
    //            if (item.Length > 2097152)
    //            {
    //                ModelState.AddModelError("DetailImgFiles", "Size type must be less than 2mb");
    //                return View();
    //            }
    //            BookImage bookImage = new()
    //            {
    //                Book = book,
    //                Url = item.SaveFile(_env.WebRootPath, "uploads/books"),
    //                IsPoster = null
    //            };
    //            await _context.BookImages.AddAsync(bookImage);
    //        }
    //    }
    //    existBook.Title = book.Title;
    //    existBook.Desc = book.Desc;
    //    existBook.BookCode = book.BookCode;
    //    existBook.CostPrice = book.CostPrice;
    //    existBook.SalePrice = book.SalePrice;
    //    existBook.DiscountPercent = book.DiscountPercent;
    //    existBook.StockCount = book.StockCount;
    //    existBook.IsFeatured = book.IsFeatured;
    //    existBook.IsNew = book.IsNew;
    //    existBook.IsBestSeller = book.IsBestSeller;
    //    existBook.IsInStock = book.IsInStock;
    //    existBook.GenreId = book.GenreId;
    //    existBook.AuthorId = book.AuthorId;
    //    await _context.SaveChangesAsync();
    //    return RedirectToAction("Index");
    //}

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

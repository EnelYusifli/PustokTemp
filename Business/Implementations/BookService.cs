using Microsoft.EntityFrameworkCore;
using PustokTemp.Business.Interfaces;
using PustokTemp.CustomExceptions.BookExceptions;
using PustokTemp.CustomExceptions.Common;
using PustokTemp.DAL;
using PustokTemp.Extensions;
using PustokTemp.Models;
using System.Linq.Expressions;
using System.Net;

namespace PustokTemp.Business.Implementations;

public class BookService : IBookService
{
    private readonly PustokDbContext _context;
    private readonly IWebHostEnvironment _env;

    public BookService(PustokDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    public  async Task CreateAsync(Book book)
    {
        if (book.PosterImgFile is null)
            throw new ImageCannotBeNullException("Image must be uploaded", "PosterImgFile");
        if (book.HoverImgFile is null)
            throw new ImageCannotBeNullException("Image must be uploaded", "HoverImgFile");

        if (!(book.HoverImgFile.ContentType == "image/jpeg" || book.HoverImgFile.ContentType == "image/png"))
            throw new UnableContentTypeException("Content type must be jpeg or png", "HoverImgFile");
        if (book.HoverImgFile.Length > 2097152)
            throw new MoreThanMaxLengthException("Size type must be less than 2mb", "HoverImgFile");
        BookImage hoverBookImage = new()
        {
            Book = book,
            Url = book.HoverImgFile.SaveFile(_env.WebRootPath, "uploads/books"),
            IsPoster = false
        };
        await _context.BookImages.AddAsync(hoverBookImage);

        if (!(book.PosterImgFile.ContentType == "image/jpeg" || book.HoverImgFile.ContentType == "image/png"))
            throw new UnableContentTypeException("Content type must be jpeg or png", "PosterImgFile");
        if (book.PosterImgFile.Length > 2097152)
            throw new MoreThanMaxLengthException("Size type must be less than 2mb", "PosterImgFile");
        BookImage posterBookImage = new()
        {
            Book = book,
            Url = book.PosterImgFile.SaveFile(_env.WebRootPath, "uploads/books"),
            IsPoster = true
        };
        await _context.BookImages.AddAsync(posterBookImage);

        if (book.DetailImgFiles is not null)
        {
            foreach (var img in book.DetailImgFiles)
            {
                if (!(img.ContentType == "image/jpeg" || img.ContentType == "image/png"))
                    throw new UnableContentTypeException("Content type must be jpeg or png", "DetailImgFiles");
                if (img.Length > 2097152)
                    throw new MoreThanMaxLengthException("Size type must be less than 2mb", "DetailImgFiles");
                BookImage bookImage = new()
                {
                    Book = book,
                    Url = img.SaveFile(_env.WebRootPath, "uploads/books"),
                    IsPoster = null
                };
                await _context.BookImages.AddAsync(bookImage);
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existBook = await GetByIdAsync(id);
        if (existBook == null) throw new EntityCannotBeFoundException("Book cannot be found");
        foreach (var bookImg in existBook.BookImages)
        {
            FileExtension.DeleteFile(_env.WebRootPath, "uploads/books", bookImg.Url);
        }
        _context.Remove(existBook);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Book>> GetAllAsync(Expression<Func<Book, bool>>? expression = null)
    {
        var query = _context.Books
            .Include(x=>x.Author)
            .Include(x=>x.Genre)
            .Include(x=>x.BookImages)
            .AsQueryable();

        return expression is not null
                ? await query.Where(expression).ToListAsync()
                : await query.ToListAsync();
    }

    public async Task<Book> GetByIdAsync(int id)
    {
        var data = await _context.Books
            .Include(x => x.Author)
            .Include(x => x.Genre)
            .Include(x => x.BookImages)
            .FirstOrDefaultAsync(x=>x.Id==id);
        if (data is null) throw new EntityCannotBeFoundException();

        return data;
    }

    public async Task<Book> GetSingleAsync(Expression<Func<Book, bool>>? expression = null)
    {
        var query = _context.Books
            .Include(x => x.Author)
            .Include(x => x.Genre)
            .Include(x => x.BookImages)
            .AsQueryable();

        return expression is not null
                ? await query.Where(expression).FirstOrDefaultAsync()
                : await query.FirstOrDefaultAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var data = await _context.Books.FindAsync(id);
        if (data is null) throw new EntityCannotBeFoundException("Book Cannot be Found");
        data.IsDeactive = !data.IsDeactive;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        if (book is null)
            throw new ArgumentNullException(nameof(book));

        Book? existBook = await _context.Books
            .Include(x => x.BookImages)
            .FirstOrDefaultAsync(x => x.Id == book.Id);

        if (existBook is null)
            throw new EntityCannotBeFoundException("Book not found.");

        existBook.Title = book.Title;
        existBook.Desc = book.Desc;
        existBook.BookCode = book.BookCode;
        existBook.CostPrice = book.CostPrice;
        existBook.SalePrice = book.SalePrice;
        existBook.DiscountPercent = book.DiscountPercent;
        existBook.StockCount = book.StockCount;
        existBook.IsFeatured = book.IsFeatured;
        existBook.IsNew = book.IsNew;
        existBook.IsBestSeller = book.IsBestSeller;
        existBook.IsInStock = book.IsInStock;
        existBook.GenreId = book.GenreId;
        existBook.AuthorId = book.AuthorId;

        if (book.HoverImgFile is not null)
            await HandleBookImage(book.HoverImgFile, existBook.Id, false);

        if (book.PosterImgFile is not null)
            await HandleBookImage(book.PosterImgFile, existBook.Id, true);

        if (book.DetailImgFiles is not null)
        {
            foreach (var imgFile in book.DetailImgFiles)
            {
                if (!(imgFile.ContentType == "image/jpeg" || imgFile.ContentType == "image/png"))
                    throw new UnableContentTypeException("Content type must be jpeg or png.", nameof(imgFile));

                if (imgFile.Length > 2097152)
                    throw new MoreThanMaxLengthException("Size must be less than 2mb.", nameof(imgFile));
                BookImage bookImage = new BookImage
                {
                    BookId = existBook.Id,
                    Url = imgFile.SaveFile(_env.WebRootPath, "uploads/books"),
                    IsPoster = null
                };
                await _context.BookImages.AddAsync(bookImage);
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task HandleBookImage(IFormFile imgFile, int bookId, bool? isPoster)
    {
        if (!(imgFile.ContentType == "image/jpeg" || imgFile.ContentType == "image/png"))
            throw new UnableContentTypeException("Content type must be jpeg or png.", nameof(imgFile));

        if (imgFile.Length > 2097152)
            throw new MoreThanMaxLengthException("Size must be less than 2mb.", nameof(imgFile));

        var existingImage = await _context.BookImages.FirstOrDefaultAsync(x => x.BookId == bookId && x.IsPoster == isPoster);
        if (existingImage is not null)
        {
            FileExtension.DeleteFile(_env.WebRootPath, "uploads/books", existingImage.Url);
            _context.BookImages.Remove(existingImage);
        }

        BookImage bookImage = new BookImage
        {
            BookId = bookId,
            Url = imgFile.SaveFile(_env.WebRootPath, "uploads/books"),
            IsPoster = isPoster
        };
        await _context.BookImages.AddAsync(bookImage);
    }

    public async Task HandleDetailImage(string fileName)
    {
        BookImage bookImage = await _context.BookImages.FirstOrDefaultAsync(x => x.Url == fileName);
        FileExtension.DeleteFile(_env.WebRootPath, "uploads/books", bookImage.Url);
        _context.BookImages.Remove(bookImage);
    }

}

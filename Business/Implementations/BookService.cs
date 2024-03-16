using Microsoft.EntityFrameworkCore;
using PustokTemp.Business.Interfaces;
using PustokTemp.CustomExceptions.BookExceptions;
using PustokTemp.CustomExceptions.Common;
using PustokTemp.DAL;
using PustokTemp.Extensions;
using PustokTemp.Models;
using System.Linq.Expressions;

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
        var existBook = await _context.Books.Include(x => x.BookImages).FirstOrDefaultAsync(x => x.Id == id);
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

    public Task UpdateAsync(Book book)
    {
        throw new NotImplementedException();
    }
}

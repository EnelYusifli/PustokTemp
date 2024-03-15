using Microsoft.EntityFrameworkCore;
using PustokTemp.Business.Interfaces;
using PustokTemp.CustomExceptions.Common;
using PustokTemp.CustomExceptions.GenreExceptions;
using PustokTemp.DAL;
using PustokTemp.Models;
using System.Linq.Expressions;

namespace PustokTemp.Business.Implementations;

public class AuthorService:IAuthorService
{
    private readonly PustokDbContext _context;

    public AuthorService(PustokDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Author author)
    {
        if (_context.Authors.Any(x => x.FullName.ToLower() == author.FullName.ToLower()))
            throw new NameAlreadyExistException("FullName", "Author name is already exist!");

        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var data = await _context.Authors.FindAsync(id);
        if (data is null) throw new EntityCannotBeFoundException("Author not found!");

        _context.Remove(data);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Author>> GetAllAsync(Expression<Func<Author, bool>>? expression = null, params string[] includes)
    {
        var query = _context.Authors.AsQueryable();

        query = _getIncludes(query, includes);

        return expression is not null
                ? await query.Where(expression).ToListAsync()
                : await query.ToListAsync();
    }

    public async Task<Author> GetByIdAsync(int id)
    {
        var data = await _context.Authors.FindAsync(id);
        if (data is null) throw new EntityCannotBeFoundException();

        return data;
    }

    public async Task<Author> GetSingleAsync(Expression<Func<Author, bool>>? expression = null)
    {
        var query = _context.Authors.AsQueryable();

        return expression is not null
                ? await query.Where(expression).FirstOrDefaultAsync()
                : await query.FirstOrDefaultAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var data = await _context.Authors.FindAsync(id);
        if (data is null) throw new EntityCannotBeFoundException();
        data.IsDeactive = !data.IsDeactive;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Author author)
    {
        var existData = await _context.Authors.FindAsync(author.Id);
        if (existData is null) throw new EntityCannotBeFoundException("Author not found!");
        if (_context.Authors.Any(x => x.FullName.ToLower() == author.FullName.ToLower())
            && existData.FullName != author.FullName)
            throw new NameAlreadyExistException("Name", "Genre name is already exist!");

        existData.FullName = author.FullName;
        await _context.SaveChangesAsync();
    }


    private IQueryable<Author> _getIncludes(IQueryable<Author> query, params string[] includes)
    {
        if (includes is not null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return query;
    }
}

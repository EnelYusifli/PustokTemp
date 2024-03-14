using Microsoft.EntityFrameworkCore;
using PustokMVC.Business.Interfaces;
using PustokTemp.CustomExceptions.Common;
using PustokTemp.CustomExceptions.GenreExceptions;
using PustokTemp.Models;
using PustokTemp.DAL;
using System.Linq.Expressions;

namespace PustokMVC.Business.Implementations
{
    public class GenreService : IGenreService
    {
        private readonly PustokDbContext _context;

        public GenreService(PustokDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Genre genre)
        {
            if (_context.Genres.Any(x => x.Name.ToLower() == genre.Name.ToLower()))
                throw new NameAlreadyExistException("Name", "Genre name is already exist!");

            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var data = await _context.Genres.FindAsync(id);
            if (data is null) throw new GenreNotFoundException("Genre not found!");

            _context.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Genre>> GetAllAsync(Expression<Func<Genre, bool>>? expression = null, params string[] includes) // isdeleted = false
        {
            var query = _context.Genres.AsQueryable();]

            query = _getIncludes(query, includes);

            return expression is not null
                    ? await query.Where(expression).ToListAsync() 
                    : await query.ToListAsync(); 
        }

        public async Task<Genre> GetByIdAsync(int id)
        {
            var data = await _context.Genres.FindAsync(id);
            if (data is null) throw new GenreNotFoundException();

            return data;
        }

        public async Task<Genre> GetSingleAsync(Expression<Func<Genre, bool>>? expression = null)
        {
            var query = _context.Genres.AsQueryable();

            return expression is not null
                    ? await query.Where(expression).FirstOrDefaultAsync()
                    : await query.FirstOrDefaultAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var data = await _context.Genres.FindAsync(id);
            if (data is null) throw new GenreNotFoundException();
            data.IsDeactive = !data.IsDeactive;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Genre genre)
        {
            var existData = await _context.Genres.FindAsync(genre.Id);
            if (existData is null) throw new GenreNotFoundException("Genre not found!");
            if (_context.Genres.Any(x => x.Name.ToLower() == genre.Name.ToLower())
                && existData.Name != genre.Name)
                throw new NameAlreadyExistException("Name", "Genre name is already exist!");

            existData.Name = genre.Name;
            await _context.SaveChangesAsync();
        }


        private IQueryable<Genre> _getIncludes(IQueryable<Genre> query, params string[] includes)
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

        //private IQueryable<Genre> _getQuery(IQueryable<Genre> query, Expression<Func<Genre, bool>>? expression = null)
        //{
        //    if (expression is not null)
        //        query = query.Where(expression);

        //    return query;
        //}
    }
}

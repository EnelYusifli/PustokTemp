using PustokTemp.Models;
using System.Linq.Expressions;

namespace PustokMVC.Business.Interfaces;

public interface IGenreService
{
    Task<Genre> GetByIdAsync(int id);
    Task<Genre> GetSingleAsync(Expression<Func<Genre, bool>>? expression = null);
    Task<List<Genre>> GetAllAsync(Expression<Func<Genre, bool>>? expression = null, params string[] includes);
    Task CreateAsync(Genre genre);
    Task UpdateAsync(Genre genre);
    Task DeleteAsync(int id);
    Task SoftDeleteAsync(int id);
}

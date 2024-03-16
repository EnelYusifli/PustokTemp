using PustokTemp.Models;
using System.Linq.Expressions;

namespace PustokTemp.Business.Interfaces;

public interface IBookService
{
    Task<Book> GetByIdAsync(int id);
    Task<Book> GetSingleAsync(Expression<Func<Book, bool>>? expression = null);
    Task<List<Book>> GetAllAsync(Expression<Func<Book, bool>>? expression = null, params string[] includes);
    Task CreateAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(int id);
    Task SoftDeleteAsync(int id);
}

using System.ComponentModel.DataAnnotations;

namespace PustokTemp.Models;

public class Author:BaseEntity
{
    [StringLength(100)]
    public string FullName { get; set; }
    public List<Book>? Books { get; set; }
}

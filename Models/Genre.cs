using System.ComponentModel.DataAnnotations;

namespace PustokTemp.Models;

public class Genre:BaseEntity
{
    [StringLength(20)]
    public string Name { get; set; }
    public List<Book>? Books { get; set; }
}

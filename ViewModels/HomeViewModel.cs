using PustokTemp.Models;

namespace PustokTemp.ViewModels;

public class HomeViewModel
{
    public List<Slider>? Sliders { get; set; }
    public List<Book>? Books { get; set; }
    public List<Genre>? Genre { get; set; }
}

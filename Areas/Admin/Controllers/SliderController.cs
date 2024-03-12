using PustokTemp.DAL;
using PustokTemp.Models;
using Microsoft.AspNetCore.Mvc;

namespace EternaMVC.Areas.Admin.Controllers;
[Area("Admin")]
public class SliderController : Controller
{
    private readonly PustokDbContext _context;

    public SliderController(PustokDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        List<Slider> sliders = _context.Sliders.ToList();
        return View(sliders);
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Slider slider)
    {
        if (!ModelState.IsValid) return View();
        slider.CreatedDate=DateTime.UtcNow.AddHours(4);
        slider.ModifiedDate=DateTime.UtcNow.AddHours(4);
        _context.Sliders.Add(slider);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    public IActionResult Update(int id)
    {
        Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);
        if (slider == null)
        {
            return NotFound();
        }
        return View(slider);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(Slider slider)
    {
        if (!ModelState.IsValid) return View();
        Slider sld = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);
        if (sld == null)
        {
            return NotFound();
        }
        sld.Title1 = slider.Title1;
        sld.Title2 = slider.Title2;
        sld.Desc = slider.Desc;
        sld.RedirectUrl = slider.RedirectUrl;
        sld.ImageUrl = slider.ImageUrl;
        slider.ModifiedDate = DateTime.UtcNow.AddHours(4);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        Slider? sld = _context.Sliders.FirstOrDefault(x => x.Id == id);
        if (sld == null)
            return NotFound();
        _context.Sliders.Remove(sld);
        _context.SaveChanges();
        return Ok();
    }
}

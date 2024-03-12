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
    public async Task<IActionResult> Create(Slider slider)
    {
        if (!ModelState.IsValid) return View();
        if(!(slider.ImageFile.ContentType == "image/jpeg" || slider.ImageFile.ContentType == "image/png"))
        {
            ModelState.AddModelError("ImageFile","Content type must be jpeg or png");
            return View();
        }
        if (slider.ImageFile.Length > 2097152)
        {
            ModelState.AddModelError("ImageFile", "Size type must be less than 2mb");
            return View();
        }
        string fileName=slider.ImageFile.FileName;
        if (fileName.Length > 14)
        {
           fileName = fileName.Substring(fileName.Length - 14,14);
        }
           fileName=Guid.NewGuid().ToString()+fileName;
        string path = $"C:\\Users\\Enel\\source\\repos\\PustokTemp\\PustokTemp\\wwwroot\\uploads\\sliders\\{fileName}";
        using (FileStream fileStream = new(path, FileMode.Create))
        {
        slider.ImageFile.CopyTo(fileStream);
        }
        slider.CreatedDate=DateTime.UtcNow.AddHours(4);
        slider.ModifiedDate=DateTime.UtcNow.AddHours(4);
        slider.ImageUrl = fileName;
        await _context.Sliders.AddAsync(slider);
        await _context.SaveChangesAsync();
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
    public async Task<IActionResult> Update(Slider slider)
    {
        Slider sld = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);
        if (sld == null)
        {
            return NotFound();
        }
        if (!ModelState.IsValid) return View();
        if (!(slider.ImageFile.ContentType == "image/jpeg" || slider.ImageFile.ContentType == "image/png"))
        {
            ModelState.AddModelError("ImageFile", "Content type must be jpeg or png");
            return View();
        }
        if (slider.ImageFile.Length > 2097152)
        {
            ModelState.AddModelError("ImageFile", "Size type must be less than 2mb");
            return View();
        }
        string fileName = slider.ImageFile.FileName;
        if (fileName.Length > 14)
        {
            fileName = fileName.Substring(fileName.Length - 14, 14);
        }
        fileName = Guid.NewGuid().ToString() + fileName;
        string oldPath = $"C:\\Users\\Enel\\source\\repos\\PustokTemp\\PustokTemp\\wwwroot\\uploads\\sliders\\{sld.ImageUrl}";
        System.IO.File.Delete(oldPath);
        string newPath = $"C:\\Users\\Enel\\source\\repos\\PustokTemp\\PustokTemp\\wwwroot\\uploads\\sliders\\{fileName}";
        using (FileStream fileStream = new(newPath, FileMode.Create))
        {
            slider.ImageFile.CopyTo(fileStream);
        }
        sld.ModifiedDate = DateTime.UtcNow.AddHours(4);
        sld.ImageUrl = fileName;
        sld.Title1 = slider.Title1;
        sld.Title2 = slider.Title2;
        sld.Desc = slider.Desc;
        sld.RedirectUrlText = slider.RedirectUrlText;
        sld.RedirectUrl = slider.RedirectUrl;
        await _context.SaveChangesAsync();
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

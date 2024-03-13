using PustokTemp.DAL;
using PustokTemp.Models;
using Microsoft.AspNetCore.Mvc;
using PustokTemp.Extensions;

namespace EternaMVC.Areas.Admin.Controllers;
[Area("Admin")]
public class SliderController : Controller
{
    private readonly PustokDbContext _context;
    private readonly IWebHostEnvironment _env;

    public SliderController(PustokDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
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
        if (slider.ImageFile is null)
        {
            ModelState.AddModelError("ImageFile", "Image must be uploaded");
            return View();
        }
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
        slider.CreatedDate = DateTime.UtcNow.AddHours(4);
        slider.ModifiedDate = DateTime.UtcNow.AddHours(4);
        slider.ImageUrl = slider.ImageFile.SaveFile(_env.WebRootPath, "uploads/sliders");
        await _context.Sliders.AddAsync(slider);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    public IActionResult Update(int id)
    {
        Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);
        if (slider == null) throw new Exception();
        return View(slider);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Slider slider)
    {
        Slider sld = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);
        if (slider == null) throw new Exception();
        if (!ModelState.IsValid) return View();
        if (slider.ImageFile is not null)
        {
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
            FileExtension.DeleteFile(_env.WebRootPath, "uploads/sliders", sld.ImageUrl);
            sld.ImageUrl = slider.ImageUrl = slider.ImageFile.SaveFile(_env.WebRootPath, "uploads/sliders");
        }
        sld.ModifiedDate = DateTime.UtcNow.AddHours(4);
        sld.Title1 = slider.Title1;
        sld.Title2 = slider.Title2;
        sld.Desc = slider.Desc;
        sld.IsDeactive = slider.IsDeactive;
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
        FileExtension.DeleteFile(_env.WebRootPath, "uploads/sliders", sld.ImageUrl);
        _context.Sliders.Remove(sld);
        _context.SaveChanges();
        return Ok();
    }
}

using PustokTemp.DAL;
using PustokTemp.Models;
using Microsoft.AspNetCore.Mvc;
using PustokTemp.Extensions;
using PustokTemp.Business.Interfaces;
using PustokTemp.CustomExceptions.BookExceptions;
using PustokTemp.CustomExceptions.Common;

namespace EternaMVC.Areas.Admin.Controllers;
[Area("Admin")]
public class SliderController : Controller
{
    private readonly ISliderService _sliderService;

    public SliderController(ISliderService sliderService)
    {
        _sliderService = sliderService;
    }
    public async Task<IActionResult> Index()
    {
        List<Slider> sliders = await _sliderService.GetAllAsync();
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
        try
        {
            await _sliderService.CreateAsync(slider);
        }
        catch (ImageCannotBeNullException ex)
        {
            ModelState.AddModelError(ex.Property,ex.Message);
            return View();
        }
        catch (UnableContentTypeException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View();
        }
        catch (MoreThanMaxLengthException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("",ex.Message);
            return View();
        }
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Update(int id)
    {
        try
        {
        Slider slider = await _sliderService.GetByIdAsync(id);
        return View(slider);
        }
        catch (EntityCannotBeFoundException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
        catch(Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Slider slider)
    {
        if (!ModelState.IsValid) return View(slider);
        try
        {
            await _sliderService.UpdateAsync(slider);
        }
        catch (UnableContentTypeException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View();
        }
        catch (MoreThanMaxLengthException ex)
        {
            ModelState.AddModelError(ex.Property, ex.Message);
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _sliderService.DeleteAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return NotFound();
        }
    }
}

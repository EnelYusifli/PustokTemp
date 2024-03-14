using Microsoft.AspNetCore.Mvc;
using PustokMVC.Business.Interfaces;
using PustokTemp.CustomExceptions.Common;
using PustokTemp.CustomExceptions.GenreExceptions;
using PustokTemp.Models;

namespace PustokMVC.Areas.Admin.Controllers
{
    [Area("admin")]
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public async Task<IActionResult> Index()
            => View(await _genreService.GetAllAsync(x => x.IsDeactive == false, "Books"));

        public IActionResult Create()
            => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Genre genre)
        {
            if (!ModelState.IsValid) return View();

            try
            {
                await _genreService.CreateAsync(genre);
            }
            catch (NameAlreadyExistException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            Genre genre = null;
            try
            {
                genre = await _genreService.GetByIdAsync(id);
            }
            catch (GenreNotFoundException ex)
            {
                return View("Error");
            }
            catch (Exception)
            {

                throw;
            }

            return View(genre);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Genre genre)
        {
            if (!ModelState.IsValid) return View();

            try
            {
                await _genreService.UpdateAsync(genre);
            }
            catch (NameAlreadyExistException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View();
            }
            catch (GenreNotFoundException ex)
            {
                return View("Error");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _genreService.DeleteAsync(id);
            }
            catch (GenreNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return NotFound();
            }
            return Ok();
        }

    }
}

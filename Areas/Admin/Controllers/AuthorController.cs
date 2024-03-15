namespace PustokTemp.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using PustokTemp.Business.Interfaces;
    using PustokTemp.CustomExceptions.Common;
    using PustokTemp.CustomExceptions.GenreExceptions;
    using PustokTemp.Models;

    namespace PustokMVC.Areas.Admin.Controllers
    {
        [Area("admin")]
        public class AuthorController : Controller
        {
            private readonly IAuthorService _authorService;

            public AuthorController(IAuthorService authorService)
            {
                _authorService = authorService;
            }

            public async Task<IActionResult> Index()
                => View(await _authorService.GetAllAsync(x => x.IsDeactive == false, "Books"));

            public IActionResult Create()
                => View();

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(Author author)
            {
                if (!ModelState.IsValid) return View();

                try
                {
                    await _authorService.CreateAsync(author);
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
                Author author = null;
                try
                {
                    author = await _authorService.GetByIdAsync(id);
                }
                catch (GenreNotFoundException ex)
                {
                    return View("Error");
                }
                catch (Exception)
                {

                    throw;
                }

                return View(author);
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Update(Author author)
            {
                if (!ModelState.IsValid) return View();

                try
                {
                    await _authorService.UpdateAsync(author);
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
                    await _authorService.DeleteAsync(id);
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
}

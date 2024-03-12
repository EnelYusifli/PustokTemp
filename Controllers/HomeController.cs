using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PustokTemp.Controllers;

public class HomeController : Controller
{

    public IActionResult Index()
    {
        return View();
    }
}

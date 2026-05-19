using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Models;
using ReservasFondoXYZ.Web.Models;

namespace ReservasFondoXYZ.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ISitioService _sitioService;

    public HomeController(ILogger<HomeController> logger, ISitioService sitioService)
    {
        _logger = logger;
        _sitioService = sitioService;
    }

    public async Task<IActionResult> Index()
    {
        var sitios = await _sitioService.ObtenerTodosAsync();
        return View(sitios);
    }

    public async Task<IActionResult> Ver(int id)
    {
        var sitio = await _sitioService.ObtenerPorIdAsync(id);
        if (sitio == null)
        {
            return NotFound();
        }
        return View(sitio);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

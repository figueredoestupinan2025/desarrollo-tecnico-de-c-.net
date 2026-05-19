using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Web.Controllers;

[Authorize]
public class SitiosController : Controller
{
    private readonly ISitioService _sitioService;

    public SitiosController(ISitioService sitioService)
    {
        _sitioService = sitioService;
    }

    // GET: Sitios
    public async Task<IActionResult> Index()
    {
        var sitios = await _sitioService.ObtenerTodosAsync();
        return View(sitios);
    }

    // GET: Sitios/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sitio = await _sitioService.ObtenerPorIdAsync(id.Value);
        if (sitio == null)
        {
            return NotFound();
        }

        return View(sitio);
    }

    // GET: Sitios/Create
    public async Task<IActionResult> Create()
    {
        var tiposSitio = await _sitioService.ObtenerTiposSitioAsync();
        ViewBag.TipoSitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposSitio, "Id", "Nombre");
        return View();
    }

    // POST: Sitios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Sitio sitio)
    {
        if (ModelState.IsValid)
        {
            await _sitioService.CrearAsync(sitio);
            return RedirectToAction(nameof(Index));
        }
        var tiposSitio = await _sitioService.ObtenerTiposSitioAsync();
        ViewBag.TipoSitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposSitio, "Id", "Nombre", sitio.TipoSitioId);
        return View(sitio);
    }

    // GET: Sitios/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sitio = await _sitioService.ObtenerPorIdAsync(id.Value);
        if (sitio == null)
        {
            return NotFound();
        }
        var tiposSitio = await _sitioService.ObtenerTiposSitioAsync();
        ViewBag.TipoSitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposSitio, "Id", "Nombre", sitio.TipoSitioId);
        return View(sitio);
    }

    // POST: Sitios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Sitio sitio)
    {
        if (id != sitio.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _sitioService.ActualizarAsync(sitio);
            return RedirectToAction(nameof(Index));
        }
        var tiposSitio = await _sitioService.ObtenerTiposSitioAsync();
        ViewBag.TipoSitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposSitio, "Id", "Nombre", sitio.TipoSitioId);
        return View(sitio);
    }

    // GET: Sitios/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sitio = await _sitioService.ObtenerPorIdAsync(id.Value);
        if (sitio == null)
        {
            return NotFound();
        }

        return View(sitio);
    }

    // POST: Sitios/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _sitioService.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }
}

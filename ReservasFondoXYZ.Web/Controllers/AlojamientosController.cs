using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Web.Controllers;

[Authorize]
public class AlojamientosController : Controller
{
    private readonly IAlojamientoService _alojamientoService;
    private readonly ISitioService _sitioService;

    public AlojamientosController(IAlojamientoService alojamientoService, ISitioService sitioService)
    {
        _alojamientoService = alojamientoService;
        _sitioService = sitioService;
    }

    // GET: Alojamientos
    public async Task<IActionResult> Index()
    {
        var alojamientos = await _alojamientoService.ObtenerTodosAsync();
        return View(alojamientos);
    }

    // GET: Alojamientos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var alojamiento = await _alojamientoService.ObtenerPorIdAsync(id.Value);
        if (alojamiento == null)
        {
            return NotFound();
        }

        return View(alojamiento);
    }

    // GET: Alojamientos/Create
    public async Task<IActionResult> Create()
    {
        var sitios = await _sitioService.ObtenerTodosAsync();
        var tiposAlojamiento = await _alojamientoService.ObtenerTiposAlojamientoAsync();
        ViewBag.SitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(sitios, "Id", "Nombre");
        ViewBag.TipoAlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposAlojamiento, "Id", "Nombre");
        return View();
    }

    // POST: Alojamientos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Alojamiento alojamiento)
    {
        if (ModelState.IsValid)
        {
            alojamiento.Activo = true;
            await _alojamientoService.CrearAsync(alojamiento);
            return RedirectToAction(nameof(Index));
        }
        var sitios = await _sitioService.ObtenerTodosAsync();
        var tiposAlojamiento = await _alojamientoService.ObtenerTiposAlojamientoAsync();
        ViewBag.SitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(sitios, "Id", "Nombre", alojamiento.SitioId);
        ViewBag.TipoAlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposAlojamiento, "Id", "Nombre", alojamiento.TipoAlojamientoId);
        return View(alojamiento);
    }

    // GET: Alojamientos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var alojamiento = await _alojamientoService.ObtenerPorIdAsync(id.Value);
        if (alojamiento == null)
        {
            return NotFound();
        }
        var sitios = await _sitioService.ObtenerTodosAsync();
        var tiposAlojamiento = await _alojamientoService.ObtenerTiposAlojamientoAsync();
        ViewBag.SitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(sitios, "Id", "Nombre", alojamiento.SitioId);
        ViewBag.TipoAlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposAlojamiento, "Id", "Nombre", alojamiento.TipoAlojamientoId);
        return View(alojamiento);
    }

    // POST: Alojamientos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Alojamiento alojamiento)
    {
        if (id != alojamiento.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _alojamientoService.ActualizarAsync(alojamiento);
            return RedirectToAction(nameof(Index));
        }
        var sitios = await _sitioService.ObtenerTodosAsync();
        var tiposAlojamiento = await _alojamientoService.ObtenerTiposAlojamientoAsync();
        ViewBag.SitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(sitios, "Id", "Nombre", alojamiento.SitioId);
        ViewBag.TipoAlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposAlojamiento, "Id", "Nombre", alojamiento.TipoAlojamientoId);
        return View(alojamiento);
    }

    // GET: Alojamientos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var alojamiento = await _alojamientoService.ObtenerPorIdAsync(id.Value);
        if (alojamiento == null)
        {
            return NotFound();
        }

        return View(alojamiento);
    }

    // POST: Alojamientos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _alojamientoService.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
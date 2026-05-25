using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Models;
using ReservasFondoXYZ.Web.ViewModels;

namespace ReservasFondoXYZ.Web.Controllers;

[Authorize(Roles = "Admin")]
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
    public async Task<IActionResult> Create(CrearAlojamientoViewModel model)
    {
        if (ModelState.IsValid)
        {
            var alojamiento = new Alojamiento
            {
                SitioId = model.SitioId,
                TipoAlojamientoId = model.TipoAlojamientoId,
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                CapacidadMaxima = model.CapacidadMaxima,
                NumeroHabitaciones = model.NumeroHabitaciones,
                Activo = true
            };
            await _alojamientoService.CrearAsync(alojamiento);
            return RedirectToAction(nameof(Index));
        }
        var sitios = await _sitioService.ObtenerTodosAsync();
        var tiposAlojamiento = await _alojamientoService.ObtenerTiposAlojamientoAsync();
        ViewBag.SitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(sitios, "Id", "Nombre", model.SitioId);
        ViewBag.TipoAlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposAlojamiento, "Id", "Nombre", model.TipoAlojamientoId);
        return View(model);
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
        
        var model = new EditarAlojamientoViewModel
        {
            Id = alojamiento.Id,
            SitioId = alojamiento.SitioId,
            TipoAlojamientoId = alojamiento.TipoAlojamientoId,
            Nombre = alojamiento.Nombre,
            Descripcion = alojamiento.Descripcion,
            CapacidadMaxima = alojamiento.CapacidadMaxima,
            NumeroHabitaciones = alojamiento.NumeroHabitaciones,
            Activo = alojamiento.Activo
        };
        
        var sitios = await _sitioService.ObtenerTodosAsync();
        var tiposAlojamiento = await _alojamientoService.ObtenerTiposAlojamientoAsync();
        ViewBag.SitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(sitios, "Id", "Nombre", model.SitioId);
        ViewBag.TipoAlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposAlojamiento, "Id", "Nombre", model.TipoAlojamientoId);
        return View(model);
    }

    // POST: Alojamientos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditarAlojamientoViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var alojamiento = await _alojamientoService.ObtenerPorIdAsync(id);
            if (alojamiento == null)
            {
                return NotFound();
            }
            
            alojamiento.SitioId = model.SitioId;
            alojamiento.TipoAlojamientoId = model.TipoAlojamientoId;
            alojamiento.Nombre = model.Nombre;
            alojamiento.Descripcion = model.Descripcion;
            alojamiento.CapacidadMaxima = model.CapacidadMaxima;
            alojamiento.NumeroHabitaciones = model.NumeroHabitaciones;
            alojamiento.Activo = model.Activo;
            
            await _alojamientoService.ActualizarAsync(alojamiento);
            return RedirectToAction(nameof(Index));
        }
        var sitios = await _sitioService.ObtenerTodosAsync();
        var tiposAlojamiento = await _alojamientoService.ObtenerTiposAlojamientoAsync();
        ViewBag.SitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(sitios, "Id", "Nombre", model.SitioId);
        ViewBag.TipoAlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposAlojamiento, "Id", "Nombre", model.TipoAlojamientoId);
        return View(model);
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

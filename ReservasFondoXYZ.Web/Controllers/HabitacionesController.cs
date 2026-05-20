using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Web.Controllers;

[Authorize]
public class HabitacionesController : Controller
{
    private readonly IHabitacionService _habitacionService;
    private readonly IAlojamientoService _alojamientoService;

    public HabitacionesController(IHabitacionService habitacionService, IAlojamientoService alojamientoService)
    {
        _habitacionService = habitacionService;
        _alojamientoService = alojamientoService;
    }

    // GET: Habitaciones
    public async Task<IActionResult> Index()
    {
        var habitaciones = await _habitacionService.ObtenerTodosAsync();
        return View(habitaciones);
    }

    // GET: Habitaciones/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var habitacion = await _habitacionService.ObtenerPorIdAsync(id.Value);
        if (habitacion == null)
        {
            return NotFound();
        }

        return View(habitacion);
    }

    // GET: Habitaciones/Create
    public async Task<IActionResult> Create()
    {
        var alojamientos = await _alojamientoService.ObtenerTodosAsync();
        ViewBag.AlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(alojamientos, "Id", "Nombre");
        return View();
    }

    // POST: Habitaciones/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Habitacion habitacion)
    {
        if (ModelState.IsValid)
        {
            habitacion.Activo = true;
            await _habitacionService.CrearAsync(habitacion);
            return RedirectToAction(nameof(Index));
        }
        var alojamientos = await _alojamientoService.ObtenerTodosAsync();
        ViewBag.AlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(alojamientos, "Id", "Nombre", habitacion.AlojamientoId);
        return View(habitacion);
    }

    // GET: Habitaciones/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var habitacion = await _habitacionService.ObtenerPorIdAsync(id.Value);
        if (habitacion == null)
        {
            return NotFound();
        }
        var alojamientos = await _alojamientoService.ObtenerTodosAsync();
        ViewBag.AlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(alojamientos, "Id", "Nombre", habitacion.AlojamientoId);
        return View(habitacion);
    }

    // POST: Habitaciones/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Habitacion habitacion)
    {
        if (id != habitacion.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _habitacionService.ActualizarAsync(habitacion);
            return RedirectToAction(nameof(Index));
        }
        var alojamientos = await _alojamientoService.ObtenerTodosAsync();
        ViewBag.AlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(alojamientos, "Id", "Nombre", habitacion.AlojamientoId);
        return View(habitacion);
    }

    // GET: Habitaciones/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var habitacion = await _habitacionService.ObtenerPorIdAsync(id.Value);
        if (habitacion == null)
        {
            return NotFound();
        }

        return View(habitacion);
    }

    // POST: Habitaciones/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _habitacionService.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
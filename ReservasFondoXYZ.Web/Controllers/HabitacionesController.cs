using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Models;
using ReservasFondoXYZ.Web.ViewModels;

namespace ReservasFondoXYZ.Web.Controllers;

[Authorize(Roles = "Admin")]
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
    public async Task<IActionResult> Create(CrearHabitacionViewModel model)
    {
        if (ModelState.IsValid)
        {
            var habitacion = new Habitacion
            {
                AlojamientoId = model.AlojamientoId,
                Numero = model.Numero,
                Descripcion = model.Descripcion,
                CapacidadMaxima = model.CapacidadMaxima,
                Activo = true
            };
            await _habitacionService.CrearAsync(habitacion);
            return RedirectToAction(nameof(Index));
        }
        var alojamientos = await _alojamientoService.ObtenerTodosAsync();
        ViewBag.AlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(alojamientos, "Id", "Nombre", model.AlojamientoId);
        return View(model);
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
        
        var model = new EditarHabitacionViewModel
        {
            Id = habitacion.Id,
            AlojamientoId = habitacion.AlojamientoId,
            Numero = habitacion.Numero,
            Descripcion = habitacion.Descripcion,
            CapacidadMaxima = habitacion.CapacidadMaxima,
            Activo = habitacion.Activo
        };
        
        var alojamientos = await _alojamientoService.ObtenerTodosAsync();
        ViewBag.AlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(alojamientos, "Id", "Nombre", model.AlojamientoId);
        return View(model);
    }

    // POST: Habitaciones/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditarHabitacionViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var habitacion = await _habitacionService.ObtenerPorIdAsync(id);
            if (habitacion == null)
            {
                return NotFound();
            }
            
            habitacion.AlojamientoId = model.AlojamientoId;
            habitacion.Numero = model.Numero;
            habitacion.Descripcion = model.Descripcion;
            habitacion.CapacidadMaxima = model.CapacidadMaxima;
            habitacion.Activo = model.Activo;
            
            await _habitacionService.ActualizarAsync(habitacion);
            return RedirectToAction(nameof(Index));
        }
        var alojamientos = await _alojamientoService.ObtenerTodosAsync();
        ViewBag.AlojamientoId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(alojamientos, "Id", "Nombre", model.AlojamientoId);
        return View(model);
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

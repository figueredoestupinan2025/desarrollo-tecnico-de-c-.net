using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Models;
using ReservasFondoXYZ.Web.ViewModels;

namespace ReservasFondoXYZ.Web.Controllers;

[Authorize(Roles = "Admin")]
public class TarifasController : Controller
{
    private readonly ITarifaService _tarifaService;
    private readonly ISitioService _sitioService;
    private readonly IAlojamientoService _alojamientoService;

    public TarifasController(ITarifaService tarifaService, ISitioService sitioService, IAlojamientoService alojamientoService)
    {
        _tarifaService = tarifaService;
        _sitioService = sitioService;
        _alojamientoService = alojamientoService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _tarifaService.ObtenerTodosAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var tarifa = await _tarifaService.ObtenerPorIdAsync(id);
        return tarifa == null ? NotFound() : View(tarifa);
    }

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new TarifaFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TarifaFormViewModel model)
    {
        ValidarRangos(model);
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(model);
            return View(model);
        }

        await _tarifaService.CrearAsync(ToEntity(model));
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var tarifa = await _tarifaService.ObtenerPorIdAsync(id);
        if (tarifa == null)
        {
            return NotFound();
        }

        var model = ToViewModel(tarifa);
        await CargarListasAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TarifaFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        ValidarRangos(model);
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(model);
            return View(model);
        }

        var updated = await _tarifaService.ActualizarAsync(ToEntity(model));
        return updated == null ? NotFound() : RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var tarifa = await _tarifaService.ObtenerPorIdAsync(id);
        return tarifa == null ? NotFound() : View(tarifa);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _tarifaService.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private void ValidarRangos(TarifaFormViewModel model)
    {
        if (model.NumeroPersonasMin > model.NumeroPersonasMax)
        {
            ModelState.AddModelError("", "El mínimo de personas no puede ser mayor al máximo.");
        }
    }

    private async Task CargarListasAsync(TarifaFormViewModel? model = null)
    {
        ViewBag.SitioId = new SelectList(await _sitioService.ObtenerTodosAsync(), "Id", "Nombre", model?.SitioId);
        ViewBag.AlojamientoId = new SelectList(await _alojamientoService.ObtenerTodosAsync(), "Id", "Nombre", model?.AlojamientoId);
        ViewBag.TipoTemporadaId = new SelectList(await _sitioService.ObtenerTiposTemporadaAsync(), "Id", "Nombre", model?.TipoTemporadaId);
    }

    private static Tarifa ToEntity(TarifaFormViewModel model)
    {
        return new Tarifa
        {
            Id = model.Id,
            SitioId = model.SitioId,
            AlojamientoId = model.AlojamientoId,
            TipoTemporadaId = model.TipoTemporadaId,
            NumeroPersonasMin = model.NumeroPersonasMin,
            NumeroPersonasMax = model.NumeroPersonasMax,
            NumeroHabitaciones = model.NumeroHabitaciones,
            PrecioBase = model.PrecioBase,
            PrecioPersonaAdicional = model.PrecioPersonaAdicional,
            EsTarifaEspecial = model.EsTarifaEspecial,
            Descripcion = model.Descripcion,
            Activo = model.Activo
        };
    }

    private static TarifaFormViewModel ToViewModel(Tarifa tarifa)
    {
        return new TarifaFormViewModel
        {
            Id = tarifa.Id,
            SitioId = tarifa.SitioId,
            AlojamientoId = tarifa.AlojamientoId,
            TipoTemporadaId = tarifa.TipoTemporadaId,
            NumeroPersonasMin = tarifa.NumeroPersonasMin,
            NumeroPersonasMax = tarifa.NumeroPersonasMax,
            NumeroHabitaciones = tarifa.NumeroHabitaciones,
            PrecioBase = tarifa.PrecioBase,
            PrecioPersonaAdicional = tarifa.PrecioPersonaAdicional,
            EsTarifaEspecial = tarifa.EsTarifaEspecial,
            Descripcion = tarifa.Descripcion,
            Activo = tarifa.Activo
        };
    }
}

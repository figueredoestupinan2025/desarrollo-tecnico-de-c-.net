using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Models;
using ReservasFondoXYZ.Web.ViewModels;

namespace ReservasFondoXYZ.Web.Controllers;

[Authorize(Roles = "Admin")]
public class TemporadasController : Controller
{
    private readonly ITemporadaService _temporadaService;
    private readonly ISitioService _sitioService;

    public TemporadasController(ITemporadaService temporadaService, ISitioService sitioService)
    {
        _temporadaService = temporadaService;
        _sitioService = sitioService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _temporadaService.ObtenerTodosAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var temporada = await _temporadaService.ObtenerPorIdAsync(id);
        return temporada == null ? NotFound() : View(temporada);
    }

    public async Task<IActionResult> Create()
    {
        await CargarTiposAsync();
        return View(new TemporadaFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TemporadaFormViewModel model)
    {
        ValidarFechas(model);
        if (!ModelState.IsValid)
        {
            await CargarTiposAsync(model.TipoTemporadaId);
            return View(model);
        }

        await _temporadaService.CrearAsync(ToEntity(model));
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var temporada = await _temporadaService.ObtenerPorIdAsync(id);
        if (temporada == null)
        {
            return NotFound();
        }

        var model = ToViewModel(temporada);
        await CargarTiposAsync(model.TipoTemporadaId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TemporadaFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        ValidarFechas(model);
        if (!ModelState.IsValid)
        {
            await CargarTiposAsync(model.TipoTemporadaId);
            return View(model);
        }

        var updated = await _temporadaService.ActualizarAsync(ToEntity(model));
        return updated == null ? NotFound() : RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var temporada = await _temporadaService.ObtenerPorIdAsync(id);
        return temporada == null ? NotFound() : View(temporada);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _temporadaService.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private void ValidarFechas(TemporadaFormViewModel model)
    {
        if (model.FechaInicio > model.FechaFin)
        {
            ModelState.AddModelError("", "La fecha inicial no puede ser posterior a la fecha final.");
        }
    }

    private async Task CargarTiposAsync(int? selected = null)
    {
        ViewBag.TipoTemporadaId = new SelectList(await _sitioService.ObtenerTiposTemporadaAsync(), "Id", "Nombre", selected);
    }

    private static Temporada ToEntity(TemporadaFormViewModel model)
    {
        return new Temporada
        {
            Id = model.Id,
            TipoTemporadaId = model.TipoTemporadaId,
            Nombre = model.Nombre,
            FechaInicio = model.FechaInicio,
            FechaFin = model.FechaFin,
            Descripcion = model.Descripcion,
            Activo = model.Activo
        };
    }

    private static TemporadaFormViewModel ToViewModel(Temporada temporada)
    {
        return new TemporadaFormViewModel
        {
            Id = temporada.Id,
            TipoTemporadaId = temporada.TipoTemporadaId,
            Nombre = temporada.Nombre,
            FechaInicio = temporada.FechaInicio,
            FechaFin = temporada.FechaFin,
            Descripcion = temporada.Descripcion,
            Activo = temporada.Activo
        };
    }
}

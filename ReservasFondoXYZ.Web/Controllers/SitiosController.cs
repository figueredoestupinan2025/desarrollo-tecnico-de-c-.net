using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Models;
using ReservasFondoXYZ.Web.ViewModels;

namespace ReservasFondoXYZ.Web.Controllers;

[Authorize(Roles = "Admin")]
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
    public async Task<IActionResult> Create(CrearSitioViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var sitio = new Sitio
            {
                TipoSitioId = viewModel.TipoSitioId,
                Nombre = viewModel.Nombre,
                Ubicacion = viewModel.Ubicacion,
                Descripcion = viewModel.Descripcion,
                CapacidadTotal = viewModel.CapacidadTotal,
                Activo = true
            };
            
            await _sitioService.CrearAsync(sitio);
            return RedirectToAction(nameof(Index));
        }
        var tiposSitio = await _sitioService.ObtenerTiposSitioAsync();
        ViewBag.TipoSitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposSitio, "Id", "Nombre", viewModel.TipoSitioId);
        return View(viewModel);
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
        
        var viewModel = new EditarSitioViewModel
        {
            Id = sitio.Id,
            TipoSitioId = sitio.TipoSitioId,
            Nombre = sitio.Nombre,
            Ubicacion = sitio.Ubicacion,
            Descripcion = sitio.Descripcion,
            CapacidadTotal = sitio.CapacidadTotal,
            Activo = sitio.Activo
        };
        
        var tiposSitio = await _sitioService.ObtenerTiposSitioAsync();
        ViewBag.TipoSitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposSitio, "Id", "Nombre", sitio.TipoSitioId);
        return View(viewModel);
    }

    // POST: Sitios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditarSitioViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var sitio = new Sitio
            {
                Id = viewModel.Id,
                TipoSitioId = viewModel.TipoSitioId,
                Nombre = viewModel.Nombre,
                Ubicacion = viewModel.Ubicacion,
                Descripcion = viewModel.Descripcion,
                CapacidadTotal = viewModel.CapacidadTotal,
                Activo = viewModel.Activo
            };
            
            await _sitioService.ActualizarAsync(sitio);
            return RedirectToAction(nameof(Index));
        }
        var tiposSitio = await _sitioService.ObtenerTiposSitioAsync();
        ViewBag.TipoSitioId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(tiposSitio, "Id", "Nombre", viewModel.TipoSitioId);
        return View(viewModel);
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Models;
using ReservasFondoXYZ.Web.ViewModels;

namespace ReservasFondoXYZ.Web.Controllers;

[Authorize]
public class ReservaController : Controller
{
    private readonly IReservaService _reservaService;
    private readonly ISitioService _sitioService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReservaController(IReservaService reservaService, ISitioService sitioService, UserManager<ApplicationUser> userManager)
    {
        _reservaService = reservaService;
        _sitioService = sitioService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Disponibilidad()
    {
        ViewBag.Sitios = await _sitioService.ObtenerTodosAsync();
        ViewBag.TiposTemporada = await _sitioService.ObtenerTiposTemporadaAsync();
        return View(new DisponibilidadViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Disponibilidad(DisponibilidadViewModel model)
    {
        ViewBag.Sitios = await _sitioService.ObtenerTodosAsync();
        ViewBag.TiposTemporada = await _sitioService.ObtenerTiposTemporadaAsync();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.FechaInicio >= model.FechaFin)
        {
            ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");
            return View(model);
        }

        var habitaciones = await _reservaService.ObtenerHabitacionesDisponiblesPorPersonasAsync(model.FechaInicio, model.FechaFin, model.NumeroPersonas);

        if (model.SitioId.HasValue)
        {
            habitaciones = habitaciones.Where(h => h.SitioId == model.SitioId.Value).ToList();
        }

        ViewBag.Habitaciones = habitaciones;
        ViewBag.FechaInicio = model.FechaInicio;
        ViewBag.FechaFin = model.FechaFin;
        ViewBag.NumeroPersonas = model.NumeroPersonas;

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Crear(int sitioId, int alojamientoId, DateTime fechaInicio, DateTime fechaFin, int numeroPersonas)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var sitio = await _sitioService.ObtenerPorIdAsync(sitioId);
        if (sitio == null)
        {
            return NotFound();
        }

        var alojamiento = sitio.Alojamientos.FirstOrDefault(a => a.Id == alojamientoId);
        if (alojamiento == null)
        {
            return NotFound();
        }

        var tipoTemporadaId = await _reservaService.ObtenerTipoTemporadaPorFechaAsync(fechaInicio);
        var tiposTemporada = await _sitioService.ObtenerTiposTemporadaAsync();
        if (!tiposTemporada.Any())
        {
            ModelState.AddModelError("", "No hay temporadas configuradas en la base de datos.");
            return RedirectToAction(nameof(Disponibilidad));
        }

        var tarifas = await _reservaService.ObtenerTarifasAsync(sitioId, tipoTemporadaId, numeroPersonas, alojamientoId);

        var reserva = new Reserva
        {
            UsuarioId = user.Id,
            SitioId = sitioId,
            AlojamientoId = alojamientoId,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            NumeroPersonas = numeroPersonas,
            NumeroHabitaciones = alojamiento.NumeroHabitaciones,
            FechaReserva = DateTime.Now
        };

        if (tarifas.Any())
        {
            var tarifa = tarifas.First();
            reserva.TarifaTotal = await _reservaService.CalcularTarifaTotalAsync(
                sitioId, alojamiento.NumeroHabitaciones, numeroPersonas, alojamientoId,
                tipoTemporadaId, fechaInicio, fechaFin);
        }

        ViewBag.Sitio = sitio;
        ViewBag.Alojamiento = alojamiento;
        ViewBag.Tarifas = tarifas;

        return View(reserva);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearReservaViewModel model)
    {
        var tipoTemporadaId = await _reservaService.ObtenerTipoTemporadaPorFechaAsync(model.FechaInicio);

        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var reserva = new Reserva
                {
                    UsuarioId = user.Id,
                    SitioId = model.SitioId,
                    AlojamientoId = model.AlojamientoId,
                    FechaInicio = model.FechaInicio,
                    FechaFin = model.FechaFin,
                    NumeroPersonas = model.NumeroPersonas,
                    NumeroHabitaciones = model.NumeroHabitaciones,
                    EstadoReservaId = 1,
                    FechaReserva = DateTime.Now
                };

                reserva.TarifaTotal = await _reservaService.CalcularTarifaTotalAsync(
                    model.SitioId, model.NumeroHabitaciones, model.NumeroPersonas, model.AlojamientoId,
                    tipoTemporadaId, model.FechaInicio, model.FechaFin);

                await _reservaService.CrearReservaAsync(reserva);
                return RedirectToAction(nameof(MisReservas));
            }
        }

        var sitio = await _sitioService.ObtenerPorIdAsync(model.SitioId);
        var alojamiento = sitio?.Alojamientos.FirstOrDefault(a => a.Id == model.AlojamientoId);
        ViewBag.Sitio = sitio;
        ViewBag.Alojamiento = alojamiento;

        var reservaViewModel = new Reserva
        {
            SitioId = model.SitioId,
            AlojamientoId = model.AlojamientoId,
            FechaInicio = model.FechaInicio,
            FechaFin = model.FechaFin,
            NumeroPersonas = model.NumeroPersonas,
            NumeroHabitaciones = model.NumeroHabitaciones
        };

        reservaViewModel.TarifaTotal = await _reservaService.CalcularTarifaTotalAsync(
            model.SitioId, model.NumeroHabitaciones, model.NumeroPersonas, model.AlojamientoId,
            tipoTemporadaId, model.FechaInicio, model.FechaFin);

        return View(reservaViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> MisReservas()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var reservas = await _reservaService.ObtenerReservasPorUsuarioAsync(user.Id);
        return View(reservas);
    }

    [HttpGet]
    public async Task<IActionResult> Detalles(int id)
    {
        var reserva = await _reservaService.ObtenerReservaPorIdAsync(id);
        if (reserva == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null || reserva.UsuarioId != user.Id)
        {
            return Forbid();
        }

        return View(reserva);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Models;

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
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Disponibilidad(DateTime fechaInicio, DateTime fechaFin, int numeroPersonas, int? sitioId = null)
    {
        ViewBag.Sitios = await _sitioService.ObtenerTodosAsync();
        ViewBag.TiposTemporada = await _sitioService.ObtenerTiposTemporadaAsync();
        ViewBag.FechaInicio = fechaInicio;
        ViewBag.FechaFin = fechaFin;
        ViewBag.NumeroPersonas = numeroPersonas;

        if (fechaInicio >= fechaFin)
        {
            ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");
            return View();
        }

        var habitaciones = await _reservaService.ObtenerHabitacionesDisponiblesPorPersonasAsync(fechaInicio, fechaFin, numeroPersonas);

        if (sitioId.HasValue)
        {
            habitaciones = habitaciones.Where(h => h.SitioId == sitioId.Value).ToList();
        }

        return View(habitaciones);
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

        var tiposTemporada = await _sitioService.ObtenerTiposTemporadaAsync();
        var tarifas = await _reservaService.ObtenerTarifasAsync(sitioId, tiposTemporada.First().Id, numeroPersonas, alojamientoId);

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
                tarifa.TipoTemporadaId, fechaInicio, fechaFin);
        }

        ViewBag.Sitio = sitio;
        ViewBag.Alojamiento = alojamiento;
        ViewBag.Tarifas = tarifas;

        return View(reserva);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Reserva reserva)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                reserva.UsuarioId = user.Id;
                reserva.EstadoReservaId = 1;
                reserva.FechaReserva = DateTime.Now;
                await _reservaService.CrearReservaAsync(reserva);
                return RedirectToAction(nameof(MisReservas));
            }
        }

        var sitio = await _sitioService.ObtenerPorIdAsync(reserva.SitioId);
        var alojamiento = sitio?.Alojamientos.FirstOrDefault(a => a.Id == reserva.AlojamientoId);
        ViewBag.Sitio = sitio;
        ViewBag.Alojamiento = alojamiento;

        return View(reserva);
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

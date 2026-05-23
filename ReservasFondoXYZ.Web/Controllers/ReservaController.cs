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
    public async Task<IActionResult> Crear(int sitioId, int alojamientoId, DateTime fechaInicio, DateTime fechaFin, int numeroPersonas, int habitacionId)
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

        var tarifaTotal = await _reservaService.CalcularTarifaTotalAsync(
            sitioId, alojamiento.NumeroHabitaciones, numeroPersonas, alojamientoId,
            tipoTemporadaId, fechaInicio, fechaFin);

        var pagoViewModel = new PagoReservaViewModel
        {
            SitioId = sitioId,
            AlojamientoId = alojamientoId,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            NumeroPersonas = numeroPersonas,
            NumeroHabitaciones = alojamiento.NumeroHabitaciones,
            HabitacionId = habitacionId,
            TarifaTotal = tarifaTotal
        };

        ViewBag.Sitio = sitio;
        ViewBag.Alojamiento = alojamiento;
        ViewBag.Tarifas = tarifas;

        return View(pagoViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Pago(int sitioId, int alojamientoId, DateTime fechaInicio, DateTime fechaFin, int numeroPersonas, int numeroHabitaciones, decimal tarifaTotal)
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

        var pagoViewModel = new PagoReservaViewModel
        {
            SitioId = sitioId,
            AlojamientoId = alojamientoId,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            NumeroPersonas = numeroPersonas,
            NumeroHabitaciones = numeroHabitaciones,
            TarifaTotal = tarifaTotal
        };

        ViewBag.Sitio = sitio;
        ViewBag.Alojamiento = alojamiento;

        return View(pagoViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pago(PagoReservaViewModel model)
    {
        if (model.FechaInicio <= DateTime.Today)
        {
            ModelState.AddModelError("", "La fecha de inicio debe ser futura.");
        }

        if (model.FechaInicio >= model.FechaFin)
        {
            ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");
        }

        if (!ModelState.IsValid)
        {
            var sitio = await _sitioService.ObtenerPorIdAsync(model.SitioId);
            var alojamiento = sitio?.Alojamientos.FirstOrDefault(a => a.Id == model.AlojamientoId);
            ViewBag.Sitio = sitio;
            ViewBag.Alojamiento = alojamiento;
            return View(model);
        }

        var tipoTemporadaId = await _reservaService.ObtenerTipoTemporadaPorFechaAsync(model.FechaInicio);
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
                EstadoReservaId = 2,
                FechaReserva = DateTime.Now,
                TarifaTotal = model.TarifaTotal
            };

            await _reservaService.CrearReservaAsync(reserva, new List<int> { model.HabitacionId });
            return RedirectToAction(nameof(Confirmacion), new { id = reserva.Id });
        }

        return RedirectToAction(nameof(Disponibilidad));
    }

    [HttpGet]
    public async Task<IActionResult> Confirmacion(int id)
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

        var sitio = await _sitioService.ObtenerPorIdAsync(reserva.SitioId);
        var alojamiento = sitio?.Alojamientos.FirstOrDefault(a => a.Id == reserva.AlojamientoId);
        ViewBag.Sitio = sitio;
        ViewBag.Alojamiento = alojamiento;

        return View(reserva);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearReservaViewModel model)
    {
        if (model.FechaInicio <= DateTime.Today)
        {
            ModelState.AddModelError("", "La fecha de inicio debe ser futura.");
        }

        if (model.FechaInicio >= model.FechaFin)
        {
            ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");
        }

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
                    EstadoReservaId = 2,
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

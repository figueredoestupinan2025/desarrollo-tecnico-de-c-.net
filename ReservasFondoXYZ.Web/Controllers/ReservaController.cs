using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Models;
using ReservasFondoXYZ.Web.Services.Payments;
using ReservasFondoXYZ.Web.ViewModels;

namespace ReservasFondoXYZ.Web.Controllers;

[Authorize]
public class ReservaController : Controller
{
    private readonly IReservaService _reservaService;
    private readonly ISitioService _sitioService;
    private readonly IHabitacionService _habitacionService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPaymentGateway _paymentGateway;

    public ReservaController(
        IReservaService reservaService,
        ISitioService sitioService,
        IHabitacionService habitacionService,
        UserManager<ApplicationUser> userManager,
        IPaymentGateway paymentGateway)
    {
        _reservaService = reservaService;
        _sitioService = sitioService;
        _habitacionService = habitacionService;
        _userManager = userManager;
        _paymentGateway = paymentGateway;
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
    public async Task<IActionResult> Crear(int sitioId, int alojamientoId, DateTime fechaInicio, DateTime fechaFin, int numeroPersonas, List<int>? habitacionesIds, int habitacionId = 0)
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

        habitacionesIds ??= new List<int>();
        if (habitacionId > 0 && !habitacionesIds.Contains(habitacionId))
        {
            habitacionesIds.Add(habitacionId);
        }

        habitacionesIds = habitacionesIds.Distinct().ToList();
        if (!habitacionesIds.Any())
        {
            TempData["Error"] = "Debes seleccionar al menos una habitación.";
            return RedirectToAction(nameof(Disponibilidad));
        }

        var habitaciones = new List<HabitacionSeleccionadaViewModel>();
        var capacidadTotal = 0;
        foreach (var id in habitacionesIds)
        {
            var habitacion = await _habitacionService.ObtenerPorIdAsync(id);
            if (habitacion == null || habitacion.AlojamientoId != alojamientoId)
            {
                TempData["Error"] = "Todas las habitaciones seleccionadas deben pertenecer al mismo alojamiento.";
                return RedirectToAction(nameof(Disponibilidad));
            }

            capacidadTotal += habitacion.CapacidadMaxima;
            habitaciones.Add(new HabitacionSeleccionadaViewModel
            {
                Id = habitacion.Id,
                Numero = habitacion.Numero,
                Descripcion = habitacion.Descripcion ?? string.Empty,
                CapacidadMaxima = habitacion.CapacidadMaxima
            });
        }

        if (capacidadTotal < numeroPersonas)
        {
            TempData["Error"] = "La capacidad de las habitaciones seleccionadas no cubre el número de personas.";
            return RedirectToAction(nameof(Disponibilidad));
        }

        var disponibles = await _reservaService.HabitacionesDisponiblesAsync(habitacionesIds, fechaInicio, fechaFin);
        if (!disponibles)
        {
            TempData["Error"] = "Una o más habitaciones seleccionadas ya no están disponibles.";
            return RedirectToAction(nameof(Disponibilidad));
        }

        var tipoTemporadaId = await _reservaService.ObtenerTipoTemporadaPorFechaAsync(fechaInicio);
        var tarifas = await _reservaService.ObtenerTarifasAsync(sitioId, tipoTemporadaId, numeroPersonas, alojamientoId);

        var numeroHabitaciones = habitacionesIds.Count;
        var tarifaTotal = await _reservaService.CalcularTarifaTotalAsync(
            sitioId, numeroHabitaciones, numeroPersonas, alojamientoId,
            tipoTemporadaId, fechaInicio, fechaFin);

        var pagoViewModel = new PagoReservaViewModel
        {
            SitioId = sitioId,
            AlojamientoId = alojamientoId,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            NumeroPersonas = numeroPersonas,
            NumeroHabitaciones = numeroHabitaciones,
            HabitacionId = habitacionesIds.First(),
            HabitacionesIds = habitacionesIds,
            HabitacionesSeleccionadas = habitaciones,
            TarifaTotal = tarifaTotal
        };

        ViewBag.Sitio = sitio;
        ViewBag.Alojamiento = alojamiento;
        ViewBag.Tarifas = tarifas;

        return View("Pago", pagoViewModel);
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
            await CargarHabitacionesSeleccionadasAsync(model);
            ViewBag.Sitio = sitio;
            ViewBag.Alojamiento = alojamiento;
            return View(model);
        }

        var tipoTemporadaId = await _reservaService.ObtenerTipoTemporadaPorFechaAsync(model.FechaInicio);
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            model.HabitacionesIds = model.HabitacionesIds.Distinct().ToList();
            if (!model.HabitacionesIds.Any())
            {
                ModelState.AddModelError("", "Debes seleccionar al menos una habitación.");
                await CargarDatosPagoAsync(model);
                return View(model);
            }

            var disponibles = await _reservaService.HabitacionesDisponiblesAsync(model.HabitacionesIds, model.FechaInicio, model.FechaFin);
            if (!disponibles)
            {
                ModelState.AddModelError("", "Una o más habitaciones ya no están disponibles para las fechas seleccionadas.");
                await CargarDatosPagoAsync(model);
                return View(model);
            }

            var tarifaTotal = await _reservaService.CalcularTarifaTotalAsync(
                model.SitioId,
                model.HabitacionesIds.Count,
                model.NumeroPersonas,
                model.AlojamientoId,
                tipoTemporadaId,
                model.FechaInicio,
                model.FechaFin);

            var payment = await _paymentGateway.AuthorizeAsync(
                new PaymentRequest(
                    Amount: tarifaTotal,
                    Currency: "COP",
                    CardNumber: model.NumeroTarjeta,
                    CardHolderName: model.NombreTitular,
                    Expiration: model.FechaExpiracion,
                    Cvv: model.CVV,
                    Description: $"Reserva sitio={model.SitioId} alojamiento={model.AlojamientoId} {model.FechaInicio:yyyy-MM-dd}->{model.FechaFin:yyyy-MM-dd}"
                ),
                HttpContext.RequestAborted);

            if (!payment.Approved)
            {
                ModelState.AddModelError("", $"Pago rechazado ({payment.Provider}): {payment.Message ?? "sin detalle"}");
                await CargarDatosPagoAsync(model);
                return View(model);
            }

            var reserva = new Reserva
            {
                UsuarioId = user.Id,
                SitioId = model.SitioId,
                AlojamientoId = model.AlojamientoId,
                FechaInicio = model.FechaInicio,
                FechaFin = model.FechaFin,
                NumeroPersonas = model.NumeroPersonas,
                NumeroHabitaciones = model.HabitacionesIds.Count,
                EstadoReservaId = 2,
                FechaReserva = DateTime.Now,
                TarifaTotal = tarifaTotal,
                Observaciones = $"Pago aprobado ({payment.Provider}). Referencia: {payment.Reference}"
            };

            await _reservaService.CrearReservaAsync(reserva, model.HabitacionesIds);
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

    private async Task CargarDatosPagoAsync(PagoReservaViewModel model)
    {
        var sitio = await _sitioService.ObtenerPorIdAsync(model.SitioId);
        ViewBag.Sitio = sitio;
        ViewBag.Alojamiento = sitio?.Alojamientos.FirstOrDefault(a => a.Id == model.AlojamientoId);
        await CargarHabitacionesSeleccionadasAsync(model);
    }

    private async Task CargarHabitacionesSeleccionadasAsync(PagoReservaViewModel model)
    {
        model.HabitacionesSeleccionadas.Clear();
        foreach (var id in model.HabitacionesIds.Distinct())
        {
            var habitacion = await _habitacionService.ObtenerPorIdAsync(id);
            if (habitacion != null)
            {
                model.HabitacionesSeleccionadas.Add(new HabitacionSeleccionadaViewModel
                {
                    Id = habitacion.Id,
                    Numero = habitacion.Numero,
                    Descripcion = habitacion.Descripcion ?? string.Empty,
                    CapacidadMaxima = habitacion.CapacidadMaxima
                });
            }
        }
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
                    FechaReserva = DateTime.Now,
                    Observaciones = $"Pago MOCK aprobado. Referencia: MOCK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"
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

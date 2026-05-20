using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Dtos;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReservasApiController : ControllerBase
{
    private readonly IReservaService _reservaService;
    private readonly ISitioService _sitioService;

    public ReservasApiController(IReservaService reservaService, ISitioService sitioService)
    {
        _reservaService = reservaService;
        _sitioService = sitioService;
    }

    [HttpGet("sitios")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Sitio>>> GetSitios()
    {
        var sitios = await _sitioService.ObtenerTodosAsync();
        return Ok(sitios);
    }

    [HttpGet("sitios/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Sitio>> GetSitio(int id)
    {
        var sitio = await _sitioService.ObtenerPorIdAsync(id);
        if (sitio == null)
        {
            return NotFound();
        }
        return Ok(sitio);
    }

    [HttpGet("disponibilidad")]
    [AllowAnonymous]
    public async Task<ActionResult<List<HabitacionDisponibleDto>>> CheckDisponibilidad([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin, [FromQuery] int numeroPersonas, [FromQuery] int? sitioId = null)
    {
        if (fechaInicio >= fechaFin)
        {
            return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin.");
        }

        var habitaciones = await _reservaService.ObtenerHabitacionesDisponiblesPorPersonasAsync(fechaInicio, fechaFin, numeroPersonas);

        if (sitioId.HasValue)
        {
            habitaciones = habitaciones.Where(h => h.SitioId == sitioId.Value).ToList();
        }

        return Ok(habitaciones);
    }
}

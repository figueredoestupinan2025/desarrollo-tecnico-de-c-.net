using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Data;
using ReservasFondoXYZ.Data.Dtos;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Business.Services;

public class ReservaService : IReservaService
{
    private readonly ApplicationDbContext _context;

    public ReservaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HabitacionDisponibleDto>> ObtenerHabitacionesDisponiblesAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        try
        {
            var habitaciones = await _context.Set<HabitacionDisponibleDto>()
                .FromSqlRaw("EXEC ObtenerHabitacionesDisponiblesPorFechas @FechaInicio, @FechaFin",
                    new SqlParameter("@FechaInicio", fechaInicio),
                    new SqlParameter("@FechaFin", fechaFin))
                .ToListAsync();

            return habitaciones;
        }
        catch
        {
            var habitacionesOcupadas = await _context.ReservasHabitaciones
                .Where(rh =>
                    rh.Reserva.FechaInicio <= fechaFin &&
                    rh.Reserva.FechaFin >= fechaInicio &&
                    (rh.Reserva.EstadoReservaId == 1 || rh.Reserva.EstadoReservaId == 2))
                .Select(rh => rh.HabitacionId)
                .ToListAsync();

            var habitacionesDisponibles = await _context.Habitaciones
                .Include(h => h.Alojamiento)
                .ThenInclude(a => a.Sitio)
                .ThenInclude(s => s.TipoSitio)
                .Where(h =>
                    !habitacionesOcupadas.Contains(h.Id) &&
                    h.Activo &&
                    h.Alojamiento.Activo &&
                    h.Alojamiento.Sitio.Activo)
                .Select(h => new HabitacionDisponibleDto
                {
                    Id = h.Id,
                    Numero = h.Numero,
                    AlojamientoId = h.AlojamientoId,
                    AlojamientoNombre = h.Alojamiento.Nombre,
                    SitioId = h.Alojamiento.SitioId,
                    SitioNombre = h.Alojamiento.Sitio.Nombre,
                    CapacidadMaxima = h.CapacidadMaxima
                })
                .OrderBy(h => h.SitioNombre)
                .ThenBy(h => h.AlojamientoNombre)
                .ThenBy(h => h.Numero)
                .ToListAsync();

            return habitacionesDisponibles;
        }
    }

    public async Task<List<HabitacionDisponibleDto>> ObtenerHabitacionesDisponiblesPorPersonasAsync(DateTime fechaInicio, DateTime fechaFin, int numeroPersonas)
    {
        try
        {
            var habitaciones = await _context.Set<HabitacionDisponibleDto>()
                .FromSqlRaw("EXEC ObtenerHabitacionesDisponiblesPorFechasYPersonas @FechaInicio, @FechaFin, @NumeroPersonas",
                    new SqlParameter("@FechaInicio", fechaInicio),
                    new SqlParameter("@FechaFin", fechaFin),
                    new SqlParameter("@NumeroPersonas", numeroPersonas))
                .ToListAsync();

            return habitaciones;
        }
        catch
        {
            var habitacionesOcupadas = await _context.ReservasHabitaciones
                .Where(rh =>
                    rh.Reserva.FechaInicio <= fechaFin &&
                    rh.Reserva.FechaFin >= fechaInicio &&
                    (rh.Reserva.EstadoReservaId == 1 || rh.Reserva.EstadoReservaId == 2))
                .Select(rh => rh.HabitacionId)
                .ToListAsync();

            var habitacionesDisponibles = await _context.Habitaciones
                .Include(h => h.Alojamiento)
                .ThenInclude(a => a.Sitio)
                .ThenInclude(s => s.TipoSitio)
                .Where(h =>
                    !habitacionesOcupadas.Contains(h.Id) &&
                    h.Activo &&
                    h.Alojamiento.Activo &&
                    h.Alojamiento.Sitio.Activo &&
                    h.CapacidadMaxima >= numeroPersonas)
                .Select(h => new HabitacionDisponibleDto
                {
                    Id = h.Id,
                    Numero = h.Numero,
                    AlojamientoId = h.AlojamientoId,
                    AlojamientoNombre = h.Alojamiento.Nombre,
                    SitioId = h.Alojamiento.SitioId,
                    SitioNombre = h.Alojamiento.Sitio.Nombre,
                    CapacidadMaxima = h.CapacidadMaxima
                })
                .OrderBy(h => h.SitioNombre)
                .ThenBy(h => h.AlojamientoNombre)
                .ThenBy(h => h.Numero)
                .ToListAsync();

            return habitacionesDisponibles;
        }
    }

    public async Task<List<TarifaDto>> ObtenerTarifasAsync(int sitioId, int tipoTemporadaId, int numeroPersonas, int? alojamientoId = null)
    {
        try
        {
            var alojamientoParam = alojamientoId.HasValue 
                ? new SqlParameter("@AlojamientoId", alojamientoId.Value) 
                : new SqlParameter("@AlojamientoId", DBNull.Value);

            var tarifas = await _context.Set<TarifaDto>()
                .FromSqlRaw("EXEC ObtenerTarifas @SitioId, @TipoTemporadaId, @NumeroPersonas, @AlojamientoId",
                    new SqlParameter("@SitioId", sitioId),
                    new SqlParameter("@TipoTemporadaId", tipoTemporadaId),
                    new SqlParameter("@NumeroPersonas", numeroPersonas),
                    alojamientoParam)
                .ToListAsync();

            return tarifas;
        }
        catch
        {
            var query = _context.Tarifas
                .Include(t => t.Sitio)
                .Include(t => t.Alojamiento)
                .Include(t => t.TipoTemporada)
                .Where(t =>
                    t.Activo &&
                    t.SitioId == sitioId &&
                    t.TipoTemporadaId == tipoTemporadaId &&
                    numeroPersonas >= t.NumeroPersonasMin &&
                    numeroPersonas <= t.NumeroPersonasMax);

            if (alojamientoId.HasValue)
            {
                query = query.Where(t => t.AlojamientoId == null || t.AlojamientoId == alojamientoId.Value);
            }

            var tarifas = await query
                .Select(t => new TarifaDto
                {
                    Id = t.Id,
                    AlojamientoId = t.AlojamientoId,
                    AlojamientoNombre = t.Alojamiento != null ? t.Alojamiento.Nombre : null,
                    SitioId = t.SitioId,
                    SitioNombre = t.Sitio.Nombre,
                    TipoTemporadaId = t.TipoTemporadaId,
                    TipoTemporadaNombre = t.TipoTemporada.Nombre,
                    NumeroPersonasMin = t.NumeroPersonasMin,
                    NumeroPersonasMax = t.NumeroPersonasMax,
                    NumeroHabitaciones = t.NumeroHabitaciones,
                    PrecioBase = t.PrecioBase,
                    PrecioPersonaAdicional = t.PrecioPersonaAdicional,
                    EsTarifaEspecial = t.EsTarifaEspecial,
                    Descripcion = t.Descripcion
                })
                .OrderBy(t => t.EsTarifaEspecial)
                .ThenBy(t => t.PrecioBase)
                .ToListAsync();

            return tarifas;
        }
    }

    public async Task<decimal> CalcularTarifaTotalAsync(int sitioId, int numeroHabitaciones, int numeroPersonas, int? alojamientoId, int tipoTemporadaId, DateTime fechaInicio, DateTime fechaFin)
    {
        try
        {
            var tarifaTotalParam = new SqlParameter("@TarifaTotal", System.Data.SqlDbType.Decimal)
            {
                Direction = System.Data.ParameterDirection.Output,
                Precision = 18,
                Scale = 2
            };

            var alojamientoParam = alojamientoId.HasValue 
                ? new SqlParameter("@AlojamientoId", alojamientoId.Value) 
                : new SqlParameter("@AlojamientoId", DBNull.Value);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC CalcularTarifaTotal @SitioId, @NumeroHabitaciones, @NumeroPersonas, @AlojamientoId, @TipoTemporadaId, @FechaInicio, @FechaFin, @TarifaTotal OUTPUT",
                new SqlParameter("@SitioId", sitioId),
                new SqlParameter("@NumeroHabitaciones", numeroHabitaciones),
                new SqlParameter("@NumeroPersonas", numeroPersonas),
                alojamientoParam,
                new SqlParameter("@TipoTemporadaId", tipoTemporadaId),
                new SqlParameter("@FechaInicio", fechaInicio),
                new SqlParameter("@FechaFin", fechaFin),
                tarifaTotalParam);

            return (decimal)(tarifaTotalParam.Value ?? 0);
        }
        catch
        {
            var numeroNoches = (fechaFin - fechaInicio).Days;
            if (numeroNoches <= 0)
            {
                numeroNoches = 1;
            }

            var query = _context.Tarifas
                .Where(t =>
                    t.Activo &&
                    t.SitioId == sitioId &&
                    t.TipoTemporadaId == tipoTemporadaId &&
                    t.NumeroHabitaciones == numeroHabitaciones &&
                    numeroPersonas >= t.NumeroPersonasMin &&
                    numeroPersonas <= t.NumeroPersonasMax);

            if (alojamientoId.HasValue)
            {
                query = query.Where(t => t.AlojamientoId == null || t.AlojamientoId == alojamientoId.Value);
            }

            var tarifa = await query
                .OrderBy(t => t.EsTarifaEspecial)
                .ThenBy(t => t.PrecioBase)
                .FirstOrDefaultAsync();

            if (tarifa == null)
            {
                return 0;
            }

            var personasAdicionales = Math.Max(0, numeroPersonas - tarifa.NumeroPersonasMax);
            var precioPorNoche = tarifa.PrecioBase + (personasAdicionales * (tarifa.PrecioPersonaAdicional ?? 0));
            var tarifaTotal = precioPorNoche * numeroNoches;

            return tarifaTotal;
        }
    }

    public async Task<Reserva> CrearReservaAsync(Reserva reserva)
    {
        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync();
        return reserva;
    }

    public async Task<List<Reserva>> ObtenerReservasPorUsuarioAsync(string usuarioId)
    {
        return await _context.Reservas
            .Include(r => r.Sitio)
            .Include(r => r.Alojamiento)
            .Include(r => r.EstadoReserva)
            .Where(r => r.UsuarioId == usuarioId)
            .OrderByDescending(r => r.FechaReserva)
            .ToListAsync();
    }

    public async Task<Reserva?> ObtenerReservaPorIdAsync(int id)
    {
        return await _context.Reservas
            .Include(r => r.Sitio)
            .Include(r => r.Alojamiento)
            .Include(r => r.EstadoReserva)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Data;
using ReservasFondoXYZ.Data.Dtos;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Business.Services;

public class ReservaService : IReservaService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReservaService> _logger;

    public ReservaService(ApplicationDbContext context, ILogger<ReservaService> logger)
    {
        _context = context;
        _logger = logger;
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ejecutando stored procedure ObtenerHabitacionesDisponiblesPorFechas, usando fallback LINQ");
            
            var habitacionesOcupadas = await _context.ReservasHabitaciones
                .Where(rh =>
                    rh.Reserva != null &&
                    rh.Reserva.FechaInicio < fechaFin &&
                    rh.Reserva.FechaFin > fechaInicio &&
                    (rh.Reserva.EstadoReservaId == 1 || rh.Reserva.EstadoReservaId == 2))
                .Select(rh => rh.HabitacionId)
                .ToListAsync();

            var habitacionesDisponibles = await _context.Habitaciones
                .Include(h => h.Alojamiento!)
                .ThenInclude(a => a.Sitio!)
                .ThenInclude(s => s.TipoSitio)
                .Where(h =>
                    !habitacionesOcupadas.Contains(h.Id) &&
                    h.Activo &&
                    h.Alojamiento != null &&
                    h.Alojamiento.Activo &&
                    h.Alojamiento.Sitio != null &&
                    h.Alojamiento.Sitio.Activo)
                .Select(h => new HabitacionDisponibleDto
                {
                    Id = h.Id,
                    Numero = h.Numero,
                    AlojamientoId = h.AlojamientoId,
                    AlojamientoNombre = h.Alojamiento!.Nombre,
                    SitioId = h.Alojamiento.SitioId,
                    SitioNombre = h.Alojamiento.Sitio!.Nombre,
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ejecutando stored procedure ObtenerHabitacionesDisponiblesPorFechasYPersonas, usando fallback LINQ");
            
            var habitacionesOcupadas = await _context.ReservasHabitaciones
                .Where(rh =>
                    rh.Reserva != null &&
                    rh.Reserva.FechaInicio < fechaFin &&
                    rh.Reserva.FechaFin > fechaInicio &&
                    (rh.Reserva.EstadoReservaId == 1 || rh.Reserva.EstadoReservaId == 2))
                .Select(rh => rh.HabitacionId)
                .ToListAsync();

            var habitacionesDisponibles = await _context.Habitaciones
                .Include(h => h.Alojamiento!)
                .ThenInclude(a => a.Sitio!)
                .ThenInclude(s => s.TipoSitio)
                .Where(h =>
                    !habitacionesOcupadas.Contains(h.Id) &&
                    h.Activo &&
                    h.Alojamiento != null &&
                    h.Alojamiento.Activo &&
                    h.Alojamiento.Sitio != null &&
                    h.Alojamiento.Sitio.Activo &&
                    h.CapacidadMaxima >= numeroPersonas)
                .Select(h => new HabitacionDisponibleDto
                {
                    Id = h.Id,
                    Numero = h.Numero,
                    AlojamientoId = h.AlojamientoId,
                    AlojamientoNombre = h.Alojamiento!.Nombre,
                    SitioId = h.Alojamiento.SitioId,
                    SitioNombre = h.Alojamiento.Sitio!.Nombre,
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ejecutando stored procedure ObtenerTarifas, usando fallback LINQ");
            
            var query = _context.Tarifas
                .Include(t => t.Sitio)
                .Include(t => t.Alojamiento)
                .Include(t => t.TipoTemporada)
                .Where(t =>
                    t.Activo &&
                    t.SitioId == sitioId &&
                    t.TipoTemporadaId == tipoTemporadaId &&
                    numeroPersonas >= t.NumeroPersonasMin);

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
                    SitioNombre = t.Sitio!.Nombre,
                    TipoTemporadaId = t.TipoTemporadaId,
                    TipoTemporadaNombre = t.TipoTemporada!.Nombre,
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

            if (tarifaTotalParam.Value is null || tarifaTotalParam.Value is DBNull)
            {
                throw new InvalidOperationException("El procedimiento CalcularTarifaTotal no retornÃ³ el valor OUTPUT esperado.");
            }

            return Convert.ToDecimal(tarifaTotalParam.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ejecutando stored procedure CalcularTarifaTotal, usando fallback LINQ");
            
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
                    numeroPersonas >= t.NumeroPersonasMin);

            if (alojamientoId.HasValue)
            {
                query = query.Where(t => t.AlojamientoId == null || t.AlojamientoId == alojamientoId.Value);
            }

            var tarifa = await query
                .Where(t => numeroPersonas <= t.NumeroPersonasMax)
                .OrderBy(t => t.EsTarifaEspecial)
                .ThenBy(t => t.NumeroPersonasMax)
                .ThenBy(t => (double)t.PrecioBase)
                .FirstOrDefaultAsync();

            tarifa ??= await query
                .OrderByDescending(t => t.NumeroPersonasMax)
                .ThenBy(t => t.EsTarifaEspecial)
                .ThenBy(t => (double)t.PrecioBase)
                .FirstOrDefaultAsync();

            if (tarifa == null)
            {
                return 0;
            }

            var personasAdicionales = Math.Max(0, numeroPersonas - tarifa.NumeroPersonasMax);
            var precioPorNoche = tarifa.PrecioBase + (personasAdicionales * (tarifa.PrecioPersonaAdicional ?? 0));
            var tarifaTotal = precioPorNoche * numeroNoches * numeroHabitaciones;

            return tarifaTotal;
        }
    }

    public async Task<int> ObtenerTipoTemporadaPorFechaAsync(DateTime fecha)
    {
        try
        {
            var tipoTemporadaParam = new SqlParameter("@TipoTemporadaId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            var nombreTemporadaParam = new SqlParameter("@NombreTemporada", System.Data.SqlDbType.NVarChar, 100)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC ObtenerTemporadaPorFecha @Fecha, @TipoTemporadaId OUTPUT, @NombreTemporada OUTPUT",
                new SqlParameter("@Fecha", fecha),
                tipoTemporadaParam,
                nombreTemporadaParam);

            return (int)(tipoTemporadaParam.Value ?? 1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ejecutando stored procedure ObtenerTemporadaPorFecha, usando fallback LINQ");
            
            var temporada = await _context.Temporadas
                .Where(t => t.Activo && fecha >= t.FechaInicio && fecha <= t.FechaFin)
                .OrderBy(t => t.TipoTemporadaId)
                .FirstOrDefaultAsync();

            return temporada?.TipoTemporadaId ?? 1;
        }
    }

    public async Task<bool> HabitacionesDisponiblesAsync(IEnumerable<int> habitacionesIds, DateTime fechaInicio, DateTime fechaFin)
    {
        var ids = habitacionesIds.Distinct().ToList();
        if (!ids.Any() || fechaInicio >= fechaFin)
        {
            return false;
        }

        var habitacionesActivas = await _context.Habitaciones
            .Include(h => h.Alojamiento)
            .ThenInclude(a => a!.Sitio)
            .Where(h =>
                ids.Contains(h.Id) &&
                h.Activo &&
                h.Alojamiento != null &&
                h.Alojamiento.Activo &&
                h.Alojamiento.Sitio != null &&
                h.Alojamiento.Sitio.Activo)
            .Select(h => h.Id)
            .ToListAsync();

        if (habitacionesActivas.Count != ids.Count)
        {
            return false;
        }

        var habitacionesOcupadas = await _context.ReservasHabitaciones
            .Where(rh =>
                ids.Contains(rh.HabitacionId) &&
                rh.Reserva != null &&
                rh.Reserva.FechaInicio < fechaFin &&
                rh.Reserva.FechaFin > fechaInicio &&
                (rh.Reserva.EstadoReservaId == 1 || rh.Reserva.EstadoReservaId == 2))
            .AnyAsync();

        return !habitacionesOcupadas;
    }

    public async Task<Reserva> CrearReservaAsync(Reserva reserva, List<int>? habitacionesIds = null)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (habitacionesIds != null && habitacionesIds.Any())
            {
                var disponibles = await HabitacionesDisponiblesAsync(habitacionesIds, reserva.FechaInicio, reserva.FechaFin);
                if (!disponibles)
                {
                    throw new InvalidOperationException("Una o más habitaciones ya no están disponibles para las fechas seleccionadas.");
                }
            }

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            if (habitacionesIds != null && habitacionesIds.Any())
            {
                foreach (var habitacionId in habitacionesIds)
                {
                    var reservaHabitacion = new ReservaHabitacion
                    {
                        ReservaId = reserva.Id,
                        HabitacionId = habitacionId
                    };
                    _context.ReservasHabitaciones.Add(reservaHabitacion);
                }
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return reserva;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando reserva, realizando rollback");
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Reserva>> ObtenerReservasPorUsuarioAsync(string usuarioId)
    {
        return await _context.Reservas
            .Include(r => r.Sitio)
            .Include(r => r.Alojamiento)
            .Include(r => r.EstadoReserva)
            .Include(r => r.ReservaHabitaciones)
                .ThenInclude(rh => rh.Habitacion)
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
            .Include(r => r.ReservaHabitaciones)
                .ThenInclude(rh => rh.Habitacion)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}

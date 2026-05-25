using Microsoft.EntityFrameworkCore;
using ReservasFondoXYZ.Data.Data;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Business.Services;

public class AlojamientoService : IAlojamientoService
{
    private readonly ApplicationDbContext _context;

    public AlojamientoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Alojamiento>> ObtenerTodosAsync()
    {
        return await _context.Alojamientos
            .Include(a => a.Sitio)
            .Include(a => a.TipoAlojamiento)
            .Where(a => a.Activo)
            .ToListAsync();
    }

    public async Task<Alojamiento?> ObtenerPorIdAsync(int id)
    {
        return await _context.Alojamientos
            .Include(a => a.Sitio)
            .Include(a => a.TipoAlojamiento)
            .Include(a => a.Habitaciones)
            .FirstOrDefaultAsync(a => a.Id == id && a.Activo);
    }

    public async Task<List<Alojamiento>> ObtenerPorSitioAsync(int sitioId)
    {
        return await _context.Alojamientos
            .Include(a => a.TipoAlojamiento)
            .Include(a => a.Habitaciones)
            .Where(a => a.SitioId == sitioId && a.Activo)
            .ToListAsync();
    }

    public async Task<Alojamiento> CrearAsync(Alojamiento alojamiento)
    {
        _context.Alojamientos.Add(alojamiento);
        await _context.SaveChangesAsync();
        return alojamiento;
    }

    public async Task<Alojamiento?> ActualizarAsync(Alojamiento alojamiento)
    {
        var existing = await _context.Alojamientos.FindAsync(alojamiento.Id);
        if (existing == null)
            return null;

        existing.Nombre = alojamiento.Nombre;
        existing.Descripcion = alojamiento.Descripcion;
        existing.CapacidadMaxima = alojamiento.CapacidadMaxima;
        existing.NumeroHabitaciones = alojamiento.NumeroHabitaciones;
        existing.Activo = alojamiento.Activo;
        existing.TipoAlojamientoId = alojamiento.TipoAlojamientoId;
        existing.SitioId = alojamiento.SitioId;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var alojamiento = await _context.Alojamientos.FindAsync(id);
        if (alojamiento == null)
            return false;

        var tieneReservasActivas = await _context.Reservas
            .AnyAsync(r => r.AlojamientoId == id && 
                          r.EstadoReservaId != 3 && 
                          r.FechaFin >= DateTime.Today);

        if (tieneReservasActivas)
            throw new InvalidOperationException("No se puede desactivar el alojamiento porque tiene reservas futuras activas.");

        alojamiento.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TipoAlojamiento>> ObtenerTiposAlojamientoAsync()
    {
        return await _context.TiposAlojamiento.ToListAsync();
    }
}
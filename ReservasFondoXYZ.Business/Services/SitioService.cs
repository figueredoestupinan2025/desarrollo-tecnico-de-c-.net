using Microsoft.EntityFrameworkCore;
using ReservasFondoXYZ.Data.Data;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Business.Services;

public class SitioService : ISitioService
{
    private readonly ApplicationDbContext _context;

    public SitioService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Sitio>> ObtenerTodosAsync()
    {
        return await _context.Sitios
            .Include(s => s.TipoSitio)
            .Where(s => s.Activo)
            .ToListAsync();
    }

    public async Task<Sitio?> ObtenerPorIdAsync(int id)
    {
        return await _context.Sitios
            .Include(s => s.TipoSitio)
            .Include(s => s.Alojamientos).ThenInclude(a => a.TipoAlojamiento)
            .FirstOrDefaultAsync(s => s.Id == id && s.Activo);
    }

    public async Task<Sitio> CrearAsync(Sitio sitio)
    {
        _context.Sitios.Add(sitio);
        await _context.SaveChangesAsync();
        return sitio;
    }

    public async Task<Sitio?> ActualizarAsync(Sitio sitio)
    {
        var existing = await _context.Sitios.FindAsync(sitio.Id);
        if (existing == null)
            return null;

        existing.Nombre = sitio.Nombre;
        existing.Ubicacion = sitio.Ubicacion;
        existing.Descripcion = sitio.Descripcion;
        existing.CapacidadTotal = sitio.CapacidadTotal;
        existing.Activo = sitio.Activo;
        existing.TipoSitioId = sitio.TipoSitioId;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var sitio = await _context.Sitios.FindAsync(id);
        if (sitio == null)
            return false;

        var tieneReservasActivas = await _context.Reservas
            .AnyAsync(r => r.SitioId == id && 
                          r.EstadoReservaId != 3 && 
                          r.FechaFin >= DateTime.Today);

        if (tieneReservasActivas)
            throw new InvalidOperationException("No se puede desactivar el sitio porque tiene reservas futuras activas.");

        sitio.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Alojamiento>> ObtenerAlojamientosPorSitioAsync(int sitioId)
    {
        return await _context.Alojamientos
            .Include(a => a.TipoAlojamiento)
            .Include(a => a.Habitaciones)
            .Where(a => a.SitioId == sitioId && a.Activo)
            .ToListAsync();
    }

    public async Task<List<TipoTemporada>> ObtenerTiposTemporadaAsync()
    {
        return await _context.TiposTemporada.ToListAsync();
    }

    public async Task<List<TipoSitio>> ObtenerTiposSitioAsync()
    {
        return await _context.TiposSitio.ToListAsync();
    }
}

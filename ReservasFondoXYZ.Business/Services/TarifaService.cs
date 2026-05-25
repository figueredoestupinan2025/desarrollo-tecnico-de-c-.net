using Microsoft.EntityFrameworkCore;
using ReservasFondoXYZ.Data.Data;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Business.Services;

public class TarifaService : ITarifaService
{
    private readonly ApplicationDbContext _context;

    public TarifaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tarifa>> ObtenerTodosAsync()
    {
        return await _context.Tarifas
            .Include(t => t.Sitio)
            .Include(t => t.Alojamiento)
            .Include(t => t.TipoTemporada)
            .OrderBy(t => t.Sitio!.Nombre)
            .ThenBy(t => t.TipoTemporada!.Nombre)
            .ThenBy(t => t.NumeroHabitaciones)
            .ToListAsync();
    }

    public async Task<Tarifa?> ObtenerPorIdAsync(int id)
    {
        return await _context.Tarifas
            .Include(t => t.Sitio)
            .Include(t => t.Alojamiento)
            .Include(t => t.TipoTemporada)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tarifa> CrearAsync(Tarifa tarifa)
    {
        _context.Tarifas.Add(tarifa);
        await _context.SaveChangesAsync();
        return tarifa;
    }

    public async Task<Tarifa?> ActualizarAsync(Tarifa tarifa)
    {
        var existing = await _context.Tarifas.FindAsync(tarifa.Id);
        if (existing == null)
        {
            return null;
        }

        existing.SitioId = tarifa.SitioId;
        existing.AlojamientoId = tarifa.AlojamientoId;
        existing.TipoTemporadaId = tarifa.TipoTemporadaId;
        existing.NumeroPersonasMin = tarifa.NumeroPersonasMin;
        existing.NumeroPersonasMax = tarifa.NumeroPersonasMax;
        existing.NumeroHabitaciones = tarifa.NumeroHabitaciones;
        existing.PrecioBase = tarifa.PrecioBase;
        existing.PrecioPersonaAdicional = tarifa.PrecioPersonaAdicional;
        existing.EsTarifaEspecial = tarifa.EsTarifaEspecial;
        existing.Descripcion = tarifa.Descripcion;
        existing.Activo = tarifa.Activo;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var tarifa = await _context.Tarifas.FindAsync(id);
        if (tarifa == null)
        {
            return false;
        }

        tarifa.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}

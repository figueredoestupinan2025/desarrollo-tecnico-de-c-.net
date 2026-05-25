using Microsoft.EntityFrameworkCore;
using ReservasFondoXYZ.Data.Data;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Business.Services;

public class TemporadaService : ITemporadaService
{
    private readonly ApplicationDbContext _context;

    public TemporadaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Temporada>> ObtenerTodosAsync()
    {
        return await _context.Temporadas
            .Include(t => t.TipoTemporada)
            .OrderByDescending(t => t.FechaInicio)
            .ToListAsync();
    }

    public async Task<Temporada?> ObtenerPorIdAsync(int id)
    {
        return await _context.Temporadas
            .Include(t => t.TipoTemporada)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Temporada> CrearAsync(Temporada temporada)
    {
        _context.Temporadas.Add(temporada);
        await _context.SaveChangesAsync();
        return temporada;
    }

    public async Task<Temporada?> ActualizarAsync(Temporada temporada)
    {
        var existing = await _context.Temporadas.FindAsync(temporada.Id);
        if (existing == null)
        {
            return null;
        }

        existing.TipoTemporadaId = temporada.TipoTemporadaId;
        existing.Nombre = temporada.Nombre;
        existing.FechaInicio = temporada.FechaInicio;
        existing.FechaFin = temporada.FechaFin;
        existing.Descripcion = temporada.Descripcion;
        existing.Activo = temporada.Activo;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var temporada = await _context.Temporadas.FindAsync(id);
        if (temporada == null)
        {
            return false;
        }

        temporada.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}

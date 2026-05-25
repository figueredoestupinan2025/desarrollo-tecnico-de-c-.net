using Microsoft.EntityFrameworkCore;
using ReservasFondoXYZ.Data.Data;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Business.Services;

public class HabitacionService : IHabitacionService
{
    private readonly ApplicationDbContext _context;

    public HabitacionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Habitacion>> ObtenerTodosAsync()
    {
        return await _context.Habitaciones
            .Include(h => h.Alojamiento!).ThenInclude(a => a.Sitio)
            .Where(h => h.Activo)
            .ToListAsync();
    }

    public async Task<Habitacion?> ObtenerPorIdAsync(int id)
    {
        return await _context.Habitaciones
            .Include(h => h.Alojamiento!).ThenInclude(a => a.Sitio)
            .FirstOrDefaultAsync(h => h.Id == id && h.Activo);
    }

    public async Task<List<Habitacion>> ObtenerPorAlojamientoAsync(int alojamientoId)
    {
        return await _context.Habitaciones
            .Where(h => h.AlojamientoId == alojamientoId && h.Activo)
            .ToListAsync();
    }

    public async Task<Habitacion> CrearAsync(Habitacion habitacion)
    {
        _context.Habitaciones.Add(habitacion);
        await _context.SaveChangesAsync();
        return habitacion;
    }

    public async Task<Habitacion?> ActualizarAsync(Habitacion habitacion)
    {
        _context.Entry(habitacion).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return habitacion;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var habitacion = await _context.Habitaciones.FindAsync(id);
        if (habitacion == null)
            return false;

        var tieneReservasActivas = await _context.ReservasHabitaciones
            .Include(rh => rh.Reserva)
            .AnyAsync(rh => rh.HabitacionId == id && 
                          rh.Reserva != null &&
                          rh.Reserva.EstadoReservaId != 3 && 
                          rh.Reserva.FechaFin >= DateTime.Today);

        if (tieneReservasActivas)
            throw new InvalidOperationException("No se puede desactivar la habitación porque tiene reservas futuras activas.");

        habitacion.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}

using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Business.Services;

public interface IHabitacionService
{
    Task<List<Habitacion>> ObtenerTodosAsync();
    Task<Habitacion?> ObtenerPorIdAsync(int id);
    Task<List<Habitacion>> ObtenerPorAlojamientoAsync(int alojamientoId);
    Task<Habitacion> CrearAsync(Habitacion habitacion);
    Task<Habitacion?> ActualizarAsync(Habitacion habitacion);
    Task<bool> EliminarAsync(int id);
}
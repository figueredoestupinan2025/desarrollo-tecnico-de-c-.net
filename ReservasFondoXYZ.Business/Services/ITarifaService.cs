using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Business.Services;

public interface ITarifaService
{
    Task<List<Tarifa>> ObtenerTodosAsync();
    Task<Tarifa?> ObtenerPorIdAsync(int id);
    Task<Tarifa> CrearAsync(Tarifa tarifa);
    Task<Tarifa?> ActualizarAsync(Tarifa tarifa);
    Task<bool> EliminarAsync(int id);
}

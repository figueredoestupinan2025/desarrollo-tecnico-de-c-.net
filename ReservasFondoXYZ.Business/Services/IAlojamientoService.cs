using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Business.Services;

public interface IAlojamientoService
{
    Task<List<Alojamiento>> ObtenerTodosAsync();
    Task<Alojamiento?> ObtenerPorIdAsync(int id);
    Task<List<Alojamiento>> ObtenerPorSitioAsync(int sitioId);
    Task<Alojamiento> CrearAsync(Alojamiento alojamiento);
    Task<Alojamiento?> ActualizarAsync(Alojamiento alojamiento);
    Task<bool> EliminarAsync(int id);
    Task<List<TipoAlojamiento>> ObtenerTiposAlojamientoAsync();
}
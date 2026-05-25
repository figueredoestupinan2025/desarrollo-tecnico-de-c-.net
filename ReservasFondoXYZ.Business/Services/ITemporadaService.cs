using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Business.Services;

public interface ITemporadaService
{
    Task<List<Temporada>> ObtenerTodosAsync();
    Task<Temporada?> ObtenerPorIdAsync(int id);
    Task<Temporada> CrearAsync(Temporada temporada);
    Task<Temporada?> ActualizarAsync(Temporada temporada);
    Task<bool> EliminarAsync(int id);
}

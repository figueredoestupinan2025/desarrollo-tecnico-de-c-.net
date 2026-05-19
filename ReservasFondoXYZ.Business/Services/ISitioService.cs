using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Business.Services;

public interface ISitioService
{
    Task<List<Sitio>> ObtenerTodosAsync();
    Task<Sitio?> ObtenerPorIdAsync(int id);
    Task<Sitio> CrearAsync(Sitio sitio);
    Task<Sitio?> ActualizarAsync(Sitio sitio);
    Task<bool> EliminarAsync(int id);
    Task<List<Alojamiento>> ObtenerAlojamientosPorSitioAsync(int sitioId);
    Task<List<TipoTemporada>> ObtenerTiposTemporadaAsync();
    Task<List<TipoSitio>> ObtenerTiposSitioAsync();
}

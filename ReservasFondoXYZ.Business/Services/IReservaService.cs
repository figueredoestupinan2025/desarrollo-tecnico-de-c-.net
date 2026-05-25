using ReservasFondoXYZ.Data.Dtos;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Business.Services;

public interface IReservaService
{
    Task<List<HabitacionDisponibleDto>> ObtenerHabitacionesDisponiblesAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<List<HabitacionDisponibleDto>> ObtenerHabitacionesDisponiblesPorPersonasAsync(DateTime fechaInicio, DateTime fechaFin, int numeroPersonas);
    Task<List<TarifaDto>> ObtenerTarifasAsync(int sitioId, int tipoTemporadaId, int numeroPersonas, int? alojamientoId = null);
    Task<decimal> CalcularTarifaTotalAsync(int sitioId, int numeroHabitaciones, int numeroPersonas, int? alojamientoId, int tipoTemporadaId, DateTime fechaInicio, DateTime fechaFin);
    Task<int> ObtenerTipoTemporadaPorFechaAsync(DateTime fecha);
    Task<bool> HabitacionesDisponiblesAsync(IEnumerable<int> habitacionesIds, DateTime fechaInicio, DateTime fechaFin);
    Task<Reserva> CrearReservaAsync(Reserva reserva, List<int>? habitacionesIds = null);
    Task<List<Reserva>> ObtenerReservasPorUsuarioAsync(string usuarioId);
    Task<Reserva?> ObtenerReservaPorIdAsync(int id);
}

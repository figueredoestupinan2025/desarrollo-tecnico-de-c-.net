namespace ReservasFondoXYZ.Data.Dtos;

public class TarifaDto
{
    public int Id { get; set; }
    public int SitioId { get; set; }
    public string SitioNombre { get; set; } = string.Empty;
    public int? AlojamientoId { get; set; }
    public string? AlojamientoNombre { get; set; }
    public int TipoTemporadaId { get; set; }
    public string TipoTemporadaNombre { get; set; } = string.Empty;
    public int NumeroPersonasMin { get; set; }
    public int NumeroPersonasMax { get; set; }
    public int NumeroHabitaciones { get; set; }
    public decimal PrecioBase { get; set; }
    public decimal? PrecioPersonaAdicional { get; set; }
    public bool EsTarifaEspecial { get; set; }
    public string? Descripcion { get; set; }
}

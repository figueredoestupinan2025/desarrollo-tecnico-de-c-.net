namespace ReservasFondoXYZ.Data.Dtos;

public class HabitacionDisponibleDto
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int CapacidadMaxima { get; set; }
    public int AlojamientoId { get; set; }
    public string AlojamientoNombre { get; set; } = string.Empty;
    public int SitioId { get; set; }
    public string SitioNombre { get; set; } = string.Empty;
    public string TipoSitio { get; set; } = string.Empty;
}

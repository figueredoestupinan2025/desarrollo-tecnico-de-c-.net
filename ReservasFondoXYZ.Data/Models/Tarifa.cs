using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasFondoXYZ.Data.Models;

public class Tarifa
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Sitio")]
    public int SitioId { get; set; }

    public Sitio? Sitio { get; set; }

    [ForeignKey("Alojamiento")]
    public int? AlojamientoId { get; set; }

    public Alojamiento? Alojamiento { get; set; }

    [ForeignKey("TipoTemporada")]
    public int TipoTemporadaId { get; set; }

    public TipoTemporada? TipoTemporada { get; set; }

    [Required]
    public int NumeroPersonasMin { get; set; }

    [Required]
    public int NumeroPersonasMax { get; set; }

    public int NumeroHabitaciones { get; set; } = 1;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioBase { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PrecioPersonaAdicional { get; set; }

    public bool EsTarifaEspecial { get; set; } = false;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;
}

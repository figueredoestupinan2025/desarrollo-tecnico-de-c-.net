using System.ComponentModel.DataAnnotations;

namespace ReservasFondoXYZ.Web.ViewModels;

public class TarifaFormViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Sitio")]
    public int SitioId { get; set; }

    [Display(Name = "Alojamiento")]
    public int? AlojamientoId { get; set; }

    [Required]
    [Display(Name = "Tipo de temporada")]
    public int TipoTemporadaId { get; set; }

    [Required]
    [Range(1, 200)]
    [Display(Name = "Personas mínimo")]
    public int NumeroPersonasMin { get; set; } = 1;

    [Required]
    [Range(1, 200)]
    [Display(Name = "Personas máximo")]
    public int NumeroPersonasMax { get; set; } = 4;

    [Required]
    [Range(1, 50)]
    [Display(Name = "Habitaciones")]
    public int NumeroHabitaciones { get; set; } = 1;

    [Required]
    [Range(0, 999999999)]
    [Display(Name = "Precio base")]
    public decimal PrecioBase { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Precio persona adicional")]
    public decimal? PrecioPersonaAdicional { get; set; }

    [Display(Name = "Tarifa especial")]
    public bool EsTarifaEspecial { get; set; }

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;
}

public class TemporadaFormViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Tipo de temporada")]
    public int TipoTemporadaId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha fin")]
    public DateTime FechaFin { get; set; } = DateTime.Today.AddDays(1);

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;
}

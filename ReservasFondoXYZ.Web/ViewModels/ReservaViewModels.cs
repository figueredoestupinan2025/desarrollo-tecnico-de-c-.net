using System.ComponentModel.DataAnnotations;

namespace ReservasFondoXYZ.Web.ViewModels;

public class DisponibilidadViewModel
{
    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha Inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "La fecha de fin es requerida")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha Fin")]
    public DateTime FechaFin { get; set; } = DateTime.Today.AddDays(1);

    [Required(ErrorMessage = "El número de personas es requerido")]
    [Range(1, 20, ErrorMessage = "El número de personas debe estar entre 1 y 20")]
    [Display(Name = "Número de Personas")]
    public int NumeroPersonas { get; set; } = 1;

    [Display(Name = "Sitio (opcional)")]
    public int? SitioId { get; set; }
}

public class CrearReservaViewModel
{
    [Required]
    public int SitioId { get; set; }

    [Required]
    public int AlojamientoId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaFin { get; set; }

    [Required]
    [Range(1, 20)]
    public int NumeroPersonas { get; set; }

    [Required]
    public int NumeroHabitaciones { get; set; }
}

public class PagoReservaViewModel
{
    [Required]
    public int SitioId { get; set; }

    [Required]
    public int AlojamientoId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaFin { get; set; }

    [Required]
    [Range(1, 20)]
    public int NumeroPersonas { get; set; }

    [Required]
    public int NumeroHabitaciones { get; set; }

    [Required]
    public decimal TarifaTotal { get; set; }

    [Required(ErrorMessage = "El número de tarjeta es requerido")]
    [Display(Name = "Número de Tarjeta")]
    [StringLength(16, MinimumLength = 16, ErrorMessage = "El número de tarjeta debe tener 16 dígitos")]
    public string NumeroTarjeta { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del titular es requerido")]
    [Display(Name = "Nombre del Titular")]
    public string NombreTitular { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de expiración es requerida")]
    [Display(Name = "Fecha de Expiración (MM/AA)")]
    [RegularExpression(@"(0[1-9]|1[0-2])\/\d{2}", ErrorMessage = "Formato inválido. Use MM/AA")]
    public string FechaExpiracion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El CVV es requerido")]
    [Display(Name = "CVV")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "El CVV debe tener 3 dígitos")]
    public string CVV { get; set; } = string.Empty;
}

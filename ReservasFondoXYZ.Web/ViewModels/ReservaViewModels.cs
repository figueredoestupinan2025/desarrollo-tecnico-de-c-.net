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

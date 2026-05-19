using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasFondoXYZ.Data.Models;

public class Habitacion
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Alojamiento")]
    public int AlojamientoId { get; set; }

    public Alojamiento? Alojamiento { get; set; }

    [Required]
    [MaxLength(50)]
    public string Numero { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [Required]
    public int CapacidadMaxima { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<ReservaHabitacion> ReservaHabitaciones { get; set; } = new List<ReservaHabitacion>();
}

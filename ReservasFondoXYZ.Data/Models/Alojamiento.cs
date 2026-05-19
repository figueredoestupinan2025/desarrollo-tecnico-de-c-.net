using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasFondoXYZ.Data.Models;

public class Alojamiento
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Sitio")]
    public int SitioId { get; set; }

    public Sitio? Sitio { get; set; }

    [ForeignKey("TipoAlojamiento")]
    public int TipoAlojamientoId { get; set; }

    public TipoAlojamiento? TipoAlojamiento { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [Required]
    public int CapacidadMaxima { get; set; }

    public int NumeroHabitaciones { get; set; } = 1;

    public bool Activo { get; set; } = true;

    public ICollection<Habitacion> Habitaciones { get; set; } = new List<Habitacion>();
    public ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservasFondoXYZ.Data.Models;

public class EstadoReserva
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}

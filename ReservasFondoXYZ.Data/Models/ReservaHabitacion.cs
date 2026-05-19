using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasFondoXYZ.Data.Models;

public class ReservaHabitacion
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Reserva")]
    public int ReservaId { get; set; }

    public Reserva? Reserva { get; set; }

    [ForeignKey("Habitacion")]
    public int HabitacionId { get; set; }

    public Habitacion? Habitacion { get; set; }
}

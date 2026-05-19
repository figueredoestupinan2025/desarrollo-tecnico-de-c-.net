using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ReservasFondoXYZ.Data.Models;

public class Reserva
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Usuario")]
    public string UsuarioId { get; set; } = string.Empty;

    public ApplicationUser? Usuario { get; set; }

    [ForeignKey("Sitio")]
    public int SitioId { get; set; }

    public Sitio? Sitio { get; set; }

    [ForeignKey("Alojamiento")]
    public int AlojamientoId { get; set; }

    public Alojamiento? Alojamiento { get; set; }

    [ForeignKey("EstadoReserva")]
    public int EstadoReservaId { get; set; } = 1;

    public EstadoReserva? EstadoReserva { get; set; }

    [Required]
    public DateTime FechaInicio { get; set; }

    [Required]
    public DateTime FechaFin { get; set; }

    [Required]
    public int NumeroPersonas { get; set; }

    [Required]
    public int NumeroHabitaciones { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TarifaTotal { get; set; }

    public DateTime FechaReserva { get; set; } = DateTime.Now;

    public string? Observaciones { get; set; }

    public ICollection<ReservaHabitacion> ReservaHabitaciones { get; set; } = new List<ReservaHabitacion>();
}

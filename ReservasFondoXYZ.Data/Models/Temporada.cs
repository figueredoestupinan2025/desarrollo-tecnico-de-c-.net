using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasFondoXYZ.Data.Models;

public class Temporada
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("TipoTemporada")]
    public int TipoTemporadaId { get; set; }

    public TipoTemporada? TipoTemporada { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public DateTime FechaInicio { get; set; }

    [Required]
    public DateTime FechaFin { get; set; }

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;
}

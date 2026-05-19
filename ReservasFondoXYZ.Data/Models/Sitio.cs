using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasFondoXYZ.Data.Models;

public class Sitio
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [ForeignKey("TipoSitio")]
    public int TipoSitioId { get; set; }

    public TipoSitio? TipoSitio { get; set; }

    [MaxLength(200)]
    public string? Ubicacion { get; set; }

    public string? Descripcion { get; set; }

    [Required]
    public int CapacidadTotal { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Alojamiento> Alojamientos { get; set; } = new List<Alojamiento>();
    public ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}

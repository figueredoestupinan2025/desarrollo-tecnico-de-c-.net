using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservasFondoXYZ.Data.Models;

public class TipoTemporada
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Temporada> Temporadas { get; set; } = new List<Temporada>();
    public ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();
}

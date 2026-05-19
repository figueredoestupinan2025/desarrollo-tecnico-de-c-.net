using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservasFondoXYZ.Data.Models;

public class TipoAlojamiento
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Alojamiento> Alojamientos { get; set; } = new List<Alojamiento>();
}

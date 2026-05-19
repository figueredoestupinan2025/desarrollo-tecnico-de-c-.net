using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservasFondoXYZ.Data.Models;

public class TipoSitio
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Sitio> Sitios { get; set; } = new List<Sitio>();
}

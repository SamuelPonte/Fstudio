using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fstudio.Models.Entities;

public class ClienteFotografia
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [ForeignKey("ClienteId")]
    public Cliente? Cliente { get; set; }

    [Required]
    [Display(Name = "Fotografia")]
    public int FotografiaId { get; set; }

    [ForeignKey("FotografiaId")]
    public Fotografia? Fotografia { get; set; }

    [Display(Name = "Data de Adição")]
    public DateTime DataAdicao { get; set; } = DateTime.UtcNow;

    [Display(Name = "Descarregada")]
    public bool Descarregada { get; set; } = false;

    [Display(Name = "Data de Download")]
    public DateTime? DataDownload { get; set; }
}

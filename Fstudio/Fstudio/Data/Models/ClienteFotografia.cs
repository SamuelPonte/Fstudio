using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fstudio.Data.Models;

/// <summary>
/// Representa a associação entre um cliente e uma fotografia.
/// Esta classe funciona como tabela intermédia da relação muitos-para-muitos
/// entre clientes e fotografias, permitindo guardar informação adicional,
/// como a data de adição, se a fotografia foi descarregada e a data do download.
/// </summary>
public class ClienteFotografia
{
    /// <summary>
    /// PK e FK para o cliente associado a esta fotografia.
    /// </summary>
    [Required]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    /// <summary>
    /// Cliente associado a esta fotografia.
    /// </summary>
    [ForeignKey("ClienteId")]
    public Cliente? Cliente { get; set; }

    /// <summary>
    /// PK eFK para a fotografia associada a este cliente.
    /// </summary>
    [Required]
    [Display(Name = "Fotografia")]
    public int FotografiaId { get; set; }

    /// <summary>
    /// Fotografia associada a este cliente.
    /// </summary>
    [ForeignKey("FotografiaId")]
    public Fotografia? Fotografia { get; set; }

    /// <summary>
    /// Data em que a fotografia foi associada ao cliente.
    /// Este valor é definido automaticamente no momento da criação do registo.
    /// </summary>
    [Display(Name = "Data de Adição")]
    public DateTime DataAdicao { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indica se a fotografia já foi descarregada pelo cliente.
    /// </summary>
    [Display(Name = "Descarregada")]
    public bool Descarregada { get; set; } = false; // Valor padrão "false"

    /// <summary>
    /// Data em que a fotografia foi descarregada pelo cliente.
    /// Este campo só deve ser preenchido quando a fotografia for descarregada.
    /// </summary>
    [Display(Name = "Data de Download")]
    public DateTime? DataDownload { get; set; }
}

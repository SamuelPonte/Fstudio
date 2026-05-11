using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fstudio.Data.Models;

public class Fotografia
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(200, ErrorMessage = "O título não pode exceder 200 caracteres")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "A descrição não pode exceder 1000 caracteres")]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Display(Name = "Data da Sessão")]
    [DataType(DataType.Date)]
    public DateTime? DataSessao { get; set; }

    [Required(ErrorMessage = "A imagem é obrigatória")]
    [StringLength(500)]
    [Display(Name = "URL da Imagem")]
    public string ImagemUrl { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "URL da Miniatura")]
    public string? ThumbnailUrl { get; set; }

    [Display(Name = "Destaque")]
    public bool Destaque { get; set; } = false;

    [Display(Name = "Visível no Portfólio")]
    public bool VisivelPortfolio { get; set; } = true;

    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    // Relação N:1 com Categoria
    [Required(ErrorMessage = "A categoria é obrigatória")]
    [Display(Name = "Categoria")]
    public int CategoriaId { get; set; }

    [ForeignKey("CategoriaId")]
    public Categoria? Categoria { get; set; }

    // Relação N:N com Cliente via ClienteFotografia
    public ICollection<ClienteFotografia> ClienteFotografias { get; set; } = new List<ClienteFotografia>();
}

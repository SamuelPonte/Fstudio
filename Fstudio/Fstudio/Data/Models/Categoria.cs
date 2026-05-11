using System.ComponentModel.DataAnnotations;

namespace Fstudio.Data.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O slug é obrigatório")]
    [StringLength(100)]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "O slug deve conter apenas letras minúsculas, números e hífens")]
    public string Slug { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Display(Name = "Ordem de Exibição")]
    [Range(0, 1000, ErrorMessage = "A ordem deve estar entre 0 e 1000")]
    public int OrdemExibicao { get; set; } = 0;

    [Display(Name = "Activa")]
    public bool Activa { get; set; } = true;

    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    // Relação 1:N com Fotografia
    public ICollection<Fotografia> Fotografias { get; set; } = new List<Fotografia>();
}

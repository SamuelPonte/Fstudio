using System.ComponentModel.DataAnnotations;

namespace Fstudio.Models.ViewModels;

public class CategoriaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int OrdemExibicao { get; set; }
    public bool Activa { get; set; }
    public int TotalFotografias { get; set; }
}

public class CategoriaCreateDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O slug é obrigatório")]
    [StringLength(100)]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "O slug deve conter apenas letras minúsculas, números e hífens")]
    public string Slug { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descricao { get; set; }

    public int OrdemExibicao { get; set; }
    public bool Activa { get; set; } = true;
}

public class CategoriaUpdateDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O slug é obrigatório")]
    [StringLength(100)]
    public string Slug { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descricao { get; set; }

    public int OrdemExibicao { get; set; }
    public bool Activa { get; set; }
}

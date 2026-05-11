using System.ComponentModel.DataAnnotations;

namespace Fstudio.Models.ViewModels;

public class FotografiaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime? DataSessao { get; set; }
    public string ImagemUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public bool Destaque { get; set; }
    public bool VisivelPortfolio { get; set; }
    public int CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    public DateTime DataCriacao { get; set; }
}

public class FotografiaCreateDto
{
    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Descricao { get; set; }

    public DateTime? DataSessao { get; set; }

    [Required(ErrorMessage = "A imagem é obrigatória")]
    public string ImagemUrl { get; set; } = string.Empty;

    public string? ThumbnailUrl { get; set; }
    public bool Destaque { get; set; }
    public bool VisivelPortfolio { get; set; } = true;

    [Required(ErrorMessage = "A categoria é obrigatória")]
    public int CategoriaId { get; set; }
}

public class FotografiaUpdateDto
{
    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Descricao { get; set; }

    public DateTime? DataSessao { get; set; }
    public string? ImagemUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool Destaque { get; set; }
    public bool VisivelPortfolio { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória")]
    public int CategoriaId { get; set; }
}

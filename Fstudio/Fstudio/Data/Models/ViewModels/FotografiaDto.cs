using System.ComponentModel.DataAnnotations;

namespace Fstudio.Data.Models.ViewModels;

/// <summary>
/// DTO utilizado para devolver os dados de uma fotografia através da API.
/// Contém a informação necessária para listagens, detalhes e apresentação no portfólio,
/// incluindo dados da categoria associada.
/// </summary>
public class FotografiaDto
{
    /// <summary>
    /// Identificador único da fotografia.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Título da fotografia ou da sessão fotográfica.
    /// </summary>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição opcional da fotografia.
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Data em que a sessão fotográfica foi realizada.
    /// Este campo pode ser nulo quando a data não é relevante ou ainda não foi definida.
    /// </summary>
    public DateTime? DataSessao { get; set; }

    /// <summary>
    /// Caminho ou URL da imagem principal da fotografia.
    /// </summary>
    public string ImagemUrl { get; set; } = string.Empty;

    /// <summary>
    /// Caminho ou URL da miniatura da fotografia.
    /// Pode ser usado para melhorar o desempenho nas listagens.
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Indica se a fotografia deve aparecer como destaque.
    /// </summary>
    public bool Destaque { get; set; }

    /// <summary>
    /// Indica se a fotografia está visível no portfólio público.
    /// </summary>
    public bool VisivelPortfolio { get; set; }

    /// <summary>
    /// Identificador da categoria associada à fotografia.
    /// </summary>
    public int CategoriaId { get; set; }

    /// <summary>
    /// Nome da categoria associada à fotografia.
    /// Este campo é usado apenas para apresentação na API.
    /// </summary>
    public string? CategoriaNome { get; set; }

    /// <summary>
    /// Data em que a fotografia foi criada/registada na aplicação.
    /// </summary>
    public DateTime DataCriacao { get; set; }
}

/// <summary>
/// DTO usado para receber os dados necessários para criar uma nova fotografia através da API.
/// Inclui validações para garantir que os campos obrigatórios são enviados corretamente.
/// </summary>
public class FotografiaCreateDto
{
    /// <summary>
    /// Título da fotografia ou da sessão fotográfica.
    /// Campo obrigatório com limite máximo de 200 caracteres.
    /// </summary>
    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição opcional da fotografia.
    /// Pode conter detalhes sobre a sessão, o local ou o evento.
    /// </summary>
    [StringLength(1000)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Data da sessão fotográfica.
    /// Este campo é opcional.
    /// </summary>
    public DateTime? DataSessao { get; set; }

    /// <summary>
    /// Caminho ou URL da imagem principal.
    /// Campo obrigatório para que a fotografia possa ser apresentada.
    /// </summary>
    [Required(ErrorMessage = "A imagem é obrigatória")]
    public string ImagemUrl { get; set; } = string.Empty;

    /// <summary>
    /// Caminho ou URL da miniatura da fotografia.
    /// Se não for indicado, pode ser usado o mesmo caminho da imagem principal.
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Indica se a fotografia deve ser marcada como destaque.
    /// </summary>
    public bool Destaque { get; set; }

    /// <summary>
    /// Indica se a fotografia deve ficar visível no portfólio público.
    /// Por defeito, uma nova fotografia fica visível.
    /// </summary>
    public bool VisivelPortfolio { get; set; } = true;

    /// <summary>
    /// Identificador da categoria à qual a fotografia pertence.
    /// Campo obrigatório para garantir a relação 1-N entre categoria e fotografias.
    /// </summary>
    [Required(ErrorMessage = "A categoria é obrigatória")]
    public int CategoriaId { get; set; }
}

/// <summary>
/// DTO usado para receber os dados necessários para atualizar uma fotografia existente.
/// Permite alterar os dados principais da fotografia e a respetiva categoria.
/// </summary>
public class FotografiaUpdateDto
{
    /// <summary>
    /// Novo título da fotografia.
    /// Campo obrigatório com limite máximo de 200 caracteres.
    /// </summary>
    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Nova descrição da fotografia.
    /// Este campo é opcional.
    /// </summary>
    [StringLength(1000)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Nova data da sessão fotográfica.
    /// Este campo é opcional.
    /// </summary>
    public DateTime? DataSessao { get; set; }

    /// <summary>
    /// Novo caminho ou URL da imagem principal.
    /// Se não for enviado, a imagem atual pode ser mantida pelo controller.
    /// </summary>
    public string? ImagemUrl { get; set; }

    /// <summary>
    /// Novo caminho ou URL da miniatura.
    /// Se não for enviado, a miniatura atual pode ser mantida pelo controller.
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Define se a fotografia fica marcada como destaque após a atualização.
    /// </summary>
    public bool Destaque { get; set; }

    /// <summary>
    /// Define se a fotografia fica visível no portfólio público após a atualização.
    /// </summary>
    public bool VisivelPortfolio { get; set; }

    /// <summary>
    /// Identificador da categoria associada à fotografia.
    /// Campo obrigatório para manter a relação entre fotografia e categoria.
    /// </summary>
    [Required(ErrorMessage = "A categoria é obrigatória")]
    public int CategoriaId { get; set; }
}

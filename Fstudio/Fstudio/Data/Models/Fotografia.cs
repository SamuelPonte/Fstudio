using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fstudio.Data.Models;

/// <summary>
/// Representa uma fotografia registada no sistema.
/// A fotografia pode ser apresentada no portfólio público, destacada na página inicial
/// e associada a uma categoria e a vários clientes através da tabela ClienteFotografia.
/// </summary>
public class Fotografia
{
    /// <summary>
    /// PK
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Título da fotografia ou da sessão fotográfica.
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty; // Obrigatório

    /// <summary>
    /// Descrição opcional da fotografia
    /// </summary>
    [StringLength(1000, ErrorMessage = "A {0} não pode exceder {1} caracteres")]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    /// <summary>
    /// Data em que a sessão fotográfica foi realizada.
    /// </summary>
    [Display(Name = "Data da Sessão")]
    [DataType(DataType.Date)]
    public DateTime? DataSessao { get; set; } // Opcional

    /// <summary>
    /// Caminho ou URL da imagem principal da fotografia.
    /// </summary>
    [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
    [StringLength(500, ErrorMessage = "A {0} não pode exceder {1} caracteres")]
    [Display(Name = "URL da Imagem")]
    public string ImagemUrl { get; set; } = string.Empty; // Obrigatório

    /// <summary>
    /// Caminho ou URL da miniatura da fotografia.
    /// Esta imagem pode ser usada para melhorar o desempenho nas listagens e no portfólio.
    /// </summary>
    [StringLength(500, ErrorMessage = "A {0} não pode exceder {1} caracteres")]
    [Display(Name = "URL da Miniatura")]
    public string? ThumbnailUrl { get; set; } // Opcional

    /// <summary>
    /// Indica se a fotografia deve ser apresentada como destaque.
    /// </summary>
    [Display(Name = "Destaque")]
    public bool Destaque { get; set; } = false; // Valor padrão não destacado

    /// <summary>
    /// Indica se a fotografia está visível no portfólio público da aplicação.
    /// Se estiver a falso, a fotografia pode continuar guardada na base de dados,
    /// mas não será apresentada publicamente.
    /// </summary>
    [Display(Name = "Visível no Portfólio")]
    public bool VisivelPortfolio { get; set; } = true; // Valor padrão visivel no portfolio

    /// <summary>
    /// Data em que a fotografia foi criada/registada no sistema.
    /// Este valor é atribuído automaticamente no momento da criação do registo.
    /// </summary>
    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    /* ******************************************
     * Relacionamentos 1-N
     * ****************************************** */
    /// <summary>
    /// FK para a categoria associada a esta fotografia.
    /// </summary>
    [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
    [Display(Name = "Categoria")]
    public int CategoriaId { get; set; }

    /// <summary>
    /// Categoria associada à fotografia.
    /// </summary>
    [ForeignKey("CategoriaId")]
    public Categoria? Categoria { get; set; }

    /* ******************************************
     * Relacionamentos N-M
     * ****************************************** */
    /// <summary>
    /// Lista de associações entre esta fotografia e os clientes.
    /// Esta relação é representada através da tabela intermédia ClienteFotografia.
    /// </summary>
    public ICollection<ClienteFotografia> ClienteFotografias { get; set; } = new List<ClienteFotografia>();
}

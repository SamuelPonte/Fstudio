using System.ComponentModel.DataAnnotations;

namespace Fstudio.Data.Models;

/// <summary>
/// Representa uma categoria de fotografias no sistema.
/// As categorias permitem organizar as fotografias por tipo, tema ou serviço,
/// </summary>
public class Categoria
{
    /// <summary>
    /// PK
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Nome da categoria apresentado na aplicação.
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty; // Obrigatório

    /// <summary>
    /// Identificador textual usado em URLs amigáveis.
    /// Deve conter apenas letras minúsculas, números e hífens.
    /// Por exemplo: casamentos, batizados ou sessoes-fotograficas.
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres")]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "O {0} deve conter apenas letras minúsculas, números e hífens")]
    [Display(Name = "Slug")]
    public string Slug { get; set; } = string.Empty; // Obrigatório

    /// <summary>
    /// Descrição opcional da categoria.
    /// </summary>
    [StringLength(500, ErrorMessage = "A {0} não pode exceder {1} caracteres")]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    /// <summary>
    /// Define a ordem em que a categoria será apresentada no portfólio ou na área administrativa.
    /// Quanto menor for o valor, mais cedo a categoria poderá aparecer nas listagens.
    /// </summary>
    [Range(0, 1000, ErrorMessage = "A {0} deve estar entre {1} e {2}")]
    [Display(Name = "Ordem de Exibição")]
    public int OrdemExibicao { get; set; } = 0;

    /// <summary>
    /// Indica se a categoria está ativa.
    /// </summary>
    [Display(Name = "Activa")]
    public bool Activa { get; set; } = true; // Ativa por padrão

    /// <summary>
    /// Data em que a categoria foi criada no sistema.
    /// Este valor é definido automaticamente no momento da criação do registo.
    /// </summary>
    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;


    /* ******************************************
     * Relacionamentos 1-N
     * ****************************************** */
    /// <summary>
    /// Lista de fotografias associadas a esta categoria.
    /// </summary>
    public ICollection<Fotografia> Fotografias { get; set; } = new List<Fotografia>();
}

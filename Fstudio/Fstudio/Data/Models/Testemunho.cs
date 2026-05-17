using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fstudio.Data.Models;

/// <summary>
/// Representa um testemunho deixado por um cliente sobre os serviços de fotografia.
/// O testemunho pode incluir um texto descritivo, uma avaliação e um estado de aprovação,
/// permitindo que apenas testemunhos aprovados sejam apresentados publicamente.
/// </summary>
public class Testemunho
{
    /// <summary>
    /// PK
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Texto do testemunho deixado pelo cliente.
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres")]
    [Display(Name = "Testemunho")]
    public string Texto { get; set; } = string.Empty; // Obrigatório

    /// <summary>
    /// Avaliação atribuída pelo cliente ao serviço prestado.
    /// O valor deve estar compreendido entre 1 e 5.
    /// </summary>
    [Range(1, 5, ErrorMessage = "A {0} deve estar entre {1} e {2}")]
    [Display(Name = "Avaliação")]
    public int Avaliacao { get; set; } = 5; // Valor padrão 5

    /// <summary>
    /// Indica se o testemunho foi aprovado pelo administrador.
    /// Apenas testemunhos aprovados devem ser apresentados publicamente.
    /// </summary>
    [Display(Name = "Aprovado")]
    public bool Aprovado { get; set; } = false; // Não aprovado por padrão

    /// <summary>
    /// Data em que o testemunho foi criado no sistema.
    /// Este valor é definido automaticamente no momento da criação do registo.
    /// </summary>
    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data em que o testemunho foi aprovado pelo administrador.
    /// Este campo só deve ser preenchido quando o testemunho for aprovado.
    /// </summary>
    [Display(Name = "Data de Aprovação")]
    public DateTime? DataAprovacao { get; set; }

    /* ******************************************
     * Relacionamentos 1-N
     * ****************************************** */
    /// <summary>
    /// FK que identifica o cliente que escreveu o testemunho.
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    /// <summary>
    /// Cliente que escreveu o testemunho.
    /// </summary>
    [ForeignKey("ClienteId")]
    public Cliente? Cliente { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fstudio.Data.Models;

/// <summary>
/// Representa um cliente do sistema, contendo os seus dados pessoais,
/// informações sobre o evento e as fotografias/serviços associados.
/// </summary>
public class Cliente
{
    /// <summary>
    /// PK
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Nome do cliente
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty; // Obrigatório

    /// <summary>
    /// Endereço de email do cliente
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
    [EmailAddress(ErrorMessage = "O {0} é inválido")]
    [StringLength(256, ErrorMessage = "O {0} não pode exceder {1} caracteres")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty; // Obrigatório

    /// <summary>
    /// Numero de telemóvel do cliente
    /// </summary>
    [Phone(ErrorMessage = "O {0} é inválido")]
    [StringLength(20, ErrorMessage = "O {0} não pode exceder {1} caracteres")]
    [Display(Name = "Telefone")]
    public string? Telefone { get; set; } // Opcional

    /// <summary>
    /// Data prevista para o evento associado ao cliente.
    /// </summary>
    [Display(Name = "Data do Evento")]
    [DataType(DataType.Date)]
    public DateTime? DataEvento { get; set; } // Opcional

    /// <summary>
    /// Tipo de serviço pretendido pelo cliente.
    /// </summary>
    [StringLength(100, ErrorMessage = "O {0} não pode exceder {1} caracteres")]
    [Display(Name = "Tipo de Serviço")]
    public string? TipoServico { get; set; } // Opcional

    /// <summary>
    /// Estado do cliente (Ativo, Inativo, etc.)
    /// </summary>
    [Required (ErrorMessage = "O {0} é de preenchimento obrigatório")]
    [StringLength(50, ErrorMessage = "O {0} não pode exceder {1} caracteres")]
    [Display(Name = "Estado")]
    public EstadoCliente Estado { get; set; } = EstadoCliente.Ativo; // Valor padrão "Ativo"

    /// <summary>
    /// Notas adicionais sobre o cliente, o evento ou o serviço pretendido.
    /// </summary>
    [StringLength(2000, ErrorMessage = "As {0} não podem exceder {1} caracteres")]
    [Display(Name = "Notas")]
    public string? Notas { get; set; } // Opcional

    /// <summary>
    /// Data de criação do cliente no sistema, 
    /// definida automaticamente na criação do registo
    /// </summary>
    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;


    /* ******************************************
     * Relacionamentos 1-1
     * ****************************************** */
    /// <summary>
    /// FK para a tabela AspNetUsers, associando o cliente a um utilizador autenticado.
    /// </summary>
    [Display(Name = "Utilizador")]
    public string? UserId { get; set; }

    /// <summary>
    /// Utilizador autenticado associado ao cliente.
    /// </summary>
    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; }


    /* ******************************************
     * Relacionamentos N-N
     * ****************************************** */
    /// <summary>
    /// Lista de fotografias associadas ao cliente, 
    /// representando os serviços de fotografia reservados.
    /// </summary>
    public ICollection<ClienteFotografia> ClienteFotografias { get; set; } = new List<ClienteFotografia>();


    /* ******************************************
     * Relacionamentos 1-N
     * ****************************************** */
    /// <summary>
    /// Testemunhos deixados pelo cliente, 
    /// representando o feedback e avaliações dos serviços de fotografia.
    /// </summary>
    public ICollection<Testemunho> Testemunhos { get; set; } = new List<Testemunho>();
}

/// <summary>
/// Classe de enumeração para representar o estado do cliente
/// </summary>
public enum EstadoCliente
{
    Ativo,
    Inativo,
    Pendente
}
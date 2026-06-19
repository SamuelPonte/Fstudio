using System.ComponentModel.DataAnnotations;

namespace Fstudio.Data.Models.ViewModels;

/// <summary>
/// DTO utilizado para devolver os dados de uma mensagem de contacto através da API.
/// Permite apresentar ao administrador a informação enviada pelo visitante,
/// bem como o estado da mensagem na área administrativa.
/// </summary>
public class ContactoDto
{
    /// <summary>
    /// Identificador único da mensagem de contacto.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome da pessoa que enviou a mensagem.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Endereço de email da pessoa que enviou a mensagem.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Número de telefone indicado pelo visitante.
    /// Este campo é opcional.
    /// </summary>
    public string? Telefone { get; set; }

    /// <summary>
    /// Data prevista para o evento associado ao pedido de contacto.
    /// Este campo é opcional.
    /// </summary>
    public DateTime? DataEvento { get; set; }

    /// <summary>
    /// Tipo de serviço pretendido pelo visitante.
    /// Exemplo: casamento, sessão de noivado ou outro serviço fotográfico.
    /// </summary>
    public string? TipoServico { get; set; }

    /// <summary>
    /// Conteúdo da mensagem enviada pelo visitante.
    /// </summary>
    public string Mensagem { get; set; } = string.Empty;

    /// <summary>
    /// Indica se a mensagem já foi lida pelo administrador.
    /// </summary>
    public bool Lido { get; set; }

    /// <summary>
    /// Indica se a mensagem foi arquivada pelo administrador.
    /// </summary>
    public bool Arquivado { get; set; }

    /// <summary>
    /// Data e hora em que a mensagem foi enviada.
    /// </summary>
    public DateTime DataEnvio { get; set; }
}

/// <summary>
/// DTO usado para receber os dados enviados através do formulário de contacto.
/// Inclui validações para garantir que os campos obrigatórios são preenchidos
/// e que os dados respeitam os formatos esperados.
/// </summary>
public class ContactoCreateDto
{
    /// <summary>
    /// Nome da pessoa que pretende entrar em contacto.
    /// Campo obrigatório com limite máximo de 200 caracteres.
    /// </summary>
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(200)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email da pessoa que enviou o pedido.
    /// Campo obrigatório e validado como endereço de email.
    /// </summary>
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Número de telefone do visitante.
    /// Campo opcional, mas validado como telefone caso seja preenchido.
    /// </summary>
    [Phone(ErrorMessage = "Telefone inválido")]
    [StringLength(20)]
    public string? Telefone { get; set; }

    /// <summary>
    /// Data prevista para o evento indicado no pedido de contacto.
    /// Este campo é opcional.
    /// </summary>
    public DateTime? DataEvento { get; set; }

    /// <summary>
    /// Tipo de serviço pretendido.
    /// Campo opcional com limite máximo de 100 caracteres.
    /// </summary>
    [StringLength(100)]
    public string? TipoServico { get; set; }

    /// <summary>
    /// Mensagem escrita pelo visitante.
    /// Campo obrigatório com limite máximo de 5000 caracteres.
    /// </summary>
    [Required(ErrorMessage = "A mensagem é obrigatória")]
    [StringLength(5000)]
    public string Mensagem { get; set; } = string.Empty;
}

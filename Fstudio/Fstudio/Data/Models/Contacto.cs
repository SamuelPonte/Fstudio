using System.ComponentModel.DataAnnotations;

namespace Fstudio.Data.Models;

/// <summary>
/// Representa uma mensagem enviada através do formulário de contacto da aplicação.
/// Esta classe permite guardar pedidos de informação, pedidos de orçamento
/// ou mensagens enviadas por visitantes, clientes ou potenciais clientes.
/// </summary>
public class Contacto
{
    /// <summary>
    /// PK
    /// </summary>
    [Key]
    public int Id { get; set; }


    /// <summary>
    /// Nome da pessoa que enviou a mensagem.
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty; // Obrigatório

    /// <summary>
    /// Endereço de email da pessoa que enviou a mensagem.
    /// Este email pode ser usado pelo administrador para responder ao contacto.
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
    [EmailAddress(ErrorMessage = "O {0} não é válido")]
    [StringLength(256, ErrorMessage = "O {0} não pode exceder {1} caracteres")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty; // Obrigatório

    /// <summary>
    /// Número de telemóvel da pessoa que enviou a mensagem.
    /// </summary>
    [Phone(ErrorMessage = "O {0} não é válido")]
    [StringLength(20, ErrorMessage = "O {0} não pode exceder {1} caracteres")]
    [Display(Name = "Telefone")]
    public string? Telefone { get; set; } // Opcional

    /// <summary>
    /// Data prevista para o evento indicado no pedido de contacto.
    /// Este campo é opcional, pois nem todas as mensagens estão associadas a um evento.
    /// </summary>
    [Display(Name = "Data do Evento")]
    [DataType(DataType.Date)]
    public DateTime? DataEvento { get; set; }

    /// <summary>
    /// Tipo de serviço pretendido pela pessoa que enviou o contacto.
    /// </summary>
    [StringLength(100, ErrorMessage = "O {0} não pode exceder {1} caracteres")]
    [Display(Name = "Tipo de Serviço")]
    public string? TipoServico { get; set; } // Opcional

    /// <summary>
    /// Conteúdo da mensagem enviada através do formulário de contacto.
    /// </summary>
    [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "A {0} deve ter entre {2} e {1} caracteres")]
    [Display(Name = "Mensagem")]
    public string Mensagem { get; set; } = string.Empty; // Obrigatório

    /// <summary>
    /// Indica se a mensagem já foi lida pelo administrador.
    /// </summary>
    [Display(Name = "Lido")]
    public bool Lido { get; set; } = false; // Não lido por padrão

    /// <summary>
    /// Indica se a mensagem foi arquivada pelo administrador.
    /// </summary>
    [Display(Name = "Arquivado")]
    public bool Arquivado { get; set; } = false; // Não arquivada por padrão

    /// <summary>
    /// Data em que a mensagem foi enviada.
    /// Este valor é definido automaticamente no momento da criação do registo.
    /// </summary>
    [Display(Name = "Data de Envio")]
    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data em que a mensagem foi lida pelo administrador.
    /// Este campo só deve ser preenchido quando a mensagem for marcada como lida.
    /// </summary>
    [Display(Name = "Data de Leitura")]
    public DateTime? DataLeitura { get; set; }
}

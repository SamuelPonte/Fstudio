using System.ComponentModel.DataAnnotations;

namespace Fstudio.Data.Models;

public class Contacto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(200, ErrorMessage = "O nome não pode exceder 200 caracteres")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(256)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Telefone inválido")]
    [StringLength(20)]
    [Display(Name = "Telefone")]
    public string? Telefone { get; set; }

    [Display(Name = "Data do Evento")]
    [DataType(DataType.Date)]
    public DateTime? DataEvento { get; set; }

    [StringLength(100)]
    [Display(Name = "Tipo de Serviço")]
    public string? TipoServico { get; set; }

    [Required(ErrorMessage = "A mensagem é obrigatória")]
    [StringLength(5000, ErrorMessage = "A mensagem não pode exceder 5000 caracteres")]
    [Display(Name = "Mensagem")]
    public string Mensagem { get; set; } = string.Empty;

    [Display(Name = "Lido")]
    public bool Lido { get; set; } = false;

    [Display(Name = "Arquivado")]
    public bool Arquivado { get; set; } = false;

    [Display(Name = "Data de Envio")]
    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

    [Display(Name = "Data de Leitura")]
    public DateTime? DataLeitura { get; set; }
}

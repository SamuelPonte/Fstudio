using System.ComponentModel.DataAnnotations;

namespace Fstudio.Models.ViewModels;

public class ContactoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public DateTime? DataEvento { get; set; }
    public string? TipoServico { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public bool Lido { get; set; }
    public bool Arquivado { get; set; }
    public DateTime DataEnvio { get; set; }
}

public class ContactoCreateDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Telefone inválido")]
    [StringLength(20)]
    public string? Telefone { get; set; }

    public DateTime? DataEvento { get; set; }

    [StringLength(100)]
    public string? TipoServico { get; set; }

    [Required(ErrorMessage = "A mensagem é obrigatória")]
    [StringLength(5000)]
    public string Mensagem { get; set; } = string.Empty;
}

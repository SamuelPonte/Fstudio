using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fstudio.Models.Entities;

public class Cliente
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

    [Required]
    [StringLength(50)]
    [Display(Name = "Estado")]
    public string Estado { get; set; } = "Ativo";

    [StringLength(2000)]
    [Display(Name = "Notas")]
    public string? Notas { get; set; }

    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    // Relação 1:1 com ApplicationUser
    [Display(Name = "Utilizador")]
    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; }

    // Relação N:N com Fotografia via ClienteFotografia
    public ICollection<ClienteFotografia> ClienteFotografias { get; set; } = new List<ClienteFotografia>();

    // Relação 1:N com Testemunho
    public ICollection<Testemunho> Testemunhos { get; set; } = new List<Testemunho>();
}

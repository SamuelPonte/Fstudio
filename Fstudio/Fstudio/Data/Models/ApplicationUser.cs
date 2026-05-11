using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Fstudio.Data.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Nome Completo")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public Cliente? Cliente { get; set; }
}

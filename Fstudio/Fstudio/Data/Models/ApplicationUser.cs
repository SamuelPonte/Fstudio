using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Fstudio.Data.Models;

/// <summary>
/// Representa um utilizador autenticado da aplicação.
/// Esta classe estende a classe IdentityUser, permitindo adicionar
/// propriedades personalizadas ao sistema de autenticação do ASP.NET Core Identity.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Nome completo do utilizador autenticado.
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres")]
    [Display(Name = "Nome Completo")]
    public string NomeCompleto { get; set; } = string.Empty; // Obrigatório

    /// <summary>
    /// Data em que o utilizador foi criado no sistema.
    /// Este valor é definido automaticamente no momento da criação do registo.
    /// </summary>
    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;


    /* ******************************************
     * Relacionamento 1-1
     * ****************************************** */
    /// <summary>
    /// Cliente associado ao utilizador autenticado.
    /// Esta propriedade representa a relação entre a conta de utilizador
    /// e os dados específicos do cliente.
    /// </summary>
    public Cliente? Cliente { get; set; }
}

// ============================================================================
// Register.cshtml.cs
// Página de registo de novos utilizadores
// ============================================================================

using System.ComponentModel.DataAnnotations;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fstudio.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger)
    {
        _userManager = userManager;
        _userStore = userStore;
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo {1} caracteres")]
        [Display(Name = "Nome Completo")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A password é obrigatória")]
        [StringLength(100, ErrorMessage = "A password deve ter entre {2} e {1} caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Password")]
        [Compare("Password", ErrorMessage = "As passwords não coincidem")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                NomeCompleto = Input.NomeCompleto,
                EmailConfirmed = true, // Auto-confirmar para simplificar
                DataCriacao = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Novo utilizador criado com sucesso.");

                // Atribuir role Cliente por defeito
                await _userManager.AddToRoleAsync(user, "Cliente");

                // Fazer login automático
                await _signInManager.SignInAsync(user, isPersistent: false);

                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, TraduzirErro(error.Description));
            }
        }

        return Page();
    }

    // Traduzir erros comuns do Identity para português
    private static string TraduzirErro(string error)
    {
        return error switch
        {
            var e when e.Contains("is already taken") => "Este email já está registado.",
            var e when e.Contains("Password") && e.Contains("digit") => "A password deve conter pelo menos um número.",
            var e when e.Contains("Password") && e.Contains("lowercase") => "A password deve conter pelo menos uma letra minúscula.",
            var e when e.Contains("Password") && e.Contains("uppercase") => "A password deve conter pelo menos uma letra maiúscula.",
            var e when e.Contains("Password") && e.Contains("non alphanumeric") => "A password deve conter pelo menos um caractere especial.",
            var e when e.Contains("Password") && e.Contains("at least") => "A password deve ter pelo menos 6 caracteres.",
            _ => error
        };
    }
}

// ============================================================================
// ForgotPassword.cshtml.cs
// Página de recuperação de password
// Permite ao utilizador solicitar um email para repor a sua password
// ============================================================================

using System.ComponentModel.DataAnnotations;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fstudio.Areas.Identity.Pages.Account;

/// <summary>
/// Modelo da página de recuperação de password.
/// Recebe o email do utilizador, verifica se existe na base de dados
/// e envia um email com um link para repor a password.
/// </summary>
public class ForgotPasswordModel : PageModel
{
    // ── Serviços injetados via Dependency Injection ───────────────────────
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Construtor — recebe os serviços necessários via injeção de dependências.
    /// </summary>
    /// <param name="userManager">Gestor de utilizadores do ASP.NET Core Identity</param>
    public ForgotPasswordModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // ── Propriedades da página ────────────────────────────────────────────

    /// <summary>
    /// Dados submetidos no formulário (email do utilizador).
    /// Ligado ao formulário via [BindProperty].
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; } = new();

    /// <summary>
    /// Indica se o email de recuperação foi enviado com sucesso.
    /// Quando true, a página mostra uma mensagem de confirmação em vez do formulário.
    /// </summary>
    public bool EmailEnviado { get; set; } = false;

    /// <summary>
    /// Modelo dos dados do formulário de recuperação de password.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        /// Email do utilizador que pretende recuperar a password.
        /// Deve corresponder a um email registado na base de dados.
        /// </summary>
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "O email introduzido não é válido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — apresenta o formulário de recuperação de password.
    /// </summary>
    public IActionResult OnGet()
    {
        // Se o utilizador já está autenticado, redirecionar para a página inicial
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Index");

        return Page();
    }

    /// <summary>
    /// POST — processa o pedido de recuperação de password.
    /// Por segurança, mostra sempre a mesma mensagem de sucesso independentemente
    /// de o email existir ou não (evita enumeração de utilizadores).
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        // Validar os dados do formulário
        if (!ModelState.IsValid)
            return Page();

        // Verificar se o utilizador existe na base de dados
        var user = await _userManager.FindByEmailAsync(Input.Email);

        // Nota de segurança: mesmo que o utilizador não exista, mostramos a mesma
        // mensagem de sucesso — isto impede que atacantes descubram quais emails
        // estão registados na aplicação (enumeração de utilizadores)
        if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            // Utilizador não existe ou email não confirmado — mostrar sucesso na mesma
            EmailEnviado = true;
            return Page();
        }

        // Gerar token de reset de password (válido por tempo limitado)
        // Este token seria incluído no link enviado por email
        // Nota: o envio de email requer configuração de um serviço SMTP
        // (ex: SendGrid, MailKit) que não está implementado nesta versão
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // TODO: Integrar serviço de email para enviar o link de reset
        // O link seria: /Identity/Account/ResetPassword?token=...&email=...
        // Por ora, a funcionalidade de envio de email não está ativa.

        // Mostrar mensagem de sucesso
        EmailEnviado = true;
        return Page();
    }
}

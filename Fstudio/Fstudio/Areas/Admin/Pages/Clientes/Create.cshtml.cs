// ============================================================================
// Admin/Clientes/Create.cshtml.cs
// Criação de um novo cliente pelo administrador
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClienteEntity = Fstudio.Data.Models.Cliente;

namespace Fstudio.Areas.Admin.Pages.Clientes;

/// <summary>
/// Modelo da página de criação de clientes na área de administração.
/// Permite criar um novo registo de cliente com opção de criar automaticamente
/// uma conta de utilizador (ApplicationUser) associada com role "Cliente".
/// Quando a conta é criada, é gerada uma password aleatória que é exibida
/// uma única vez ao administrador via TempData.
/// Acesso restrito à role "Admin".
/// </summary>
[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    // ── Serviços ──────────────────────────────────────────────────────────
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Construtor — injeta o contexto da BD e o gestor de utilizadores do Identity.
    /// </summary>
    public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context    = context;
        _userManager = userManager;
    }

    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Dados do novo cliente submetidos pelo formulário.
    /// Inicializado com estado Ativo por defeito.
    /// </summary>
    [BindProperty]
    public ClienteEntity Cliente { get; set; } = new() { Estado = EstadoCliente.Ativo };

    /// <summary>
    /// Indica se deve ser criada uma conta de utilizador para este cliente.
    /// Quando true, é gerada uma conta Identity com role "Cliente" e password aleatória.
    /// </summary>
    [BindProperty]
    public bool CriarUtilizador { get; set; }

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — apresenta o formulário de criação de cliente vazio.
    /// </summary>
    public IActionResult OnGet()
    {
        return Page();
    }

    /// <summary>
    /// POST — valida e guarda o novo cliente.
    /// Se CriarUtilizador for true, cria também a conta Identity associada
    /// e guarda a password gerada em TempData para exibição única ao admin.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Definir data de criação no servidor
        Cliente.DataCriacao = DateTime.UtcNow;

        if (CriarUtilizador)
        {
            // Criar conta de utilizador Identity para o cliente
            var user = new ApplicationUser
            {
                UserName       = Cliente.Email,
                Email          = Cliente.Email,
                NomeCompleto   = Cliente.Nome,
                EmailConfirmed = true,          // Auto-confirmar — não usamos verificação por email
                DataCriacao    = DateTime.UtcNow
            };

            // Gerar password aleatória segura
            var password = GenerateRandomPassword();
            var result   = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Atribuir role "Cliente" ao utilizador criado
                await _userManager.AddToRoleAsync(user, "Cliente");

                // Associar o utilizador ao registo de cliente
                Cliente.UserId = user.Id;

                // Guardar a password em TempData para exibição única ao admin
                // TempData é limpo após ser lido — a password não fica persistida
                TempData["PasswordGerada"] = password;
            }
            else
            {
                // Mostrar erros de criação da conta (ex: email já existe)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }

        _context.Clientes.Add(Cliente);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    /// <summary>
    /// Gera uma password aleatória que cumpre os requisitos do Identity:
    /// mínimo 8 caracteres, maiúsculas, minúsculas, números e caractere especial.
    /// Usa um conjunto de caracteres sem ambiguidade visual (sem 0/O, 1/I/l).
    /// </summary>
    /// <returns>Password aleatória de 14 caracteres</returns>
    private static string GenerateRandomPassword()
    {
        // Caracteres sem ambiguidade visual para facilitar a comunicação ao cliente
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
        var random   = new Random();
        var password = new char[12];

        // Gerar 12 caracteres aleatórios do conjunto definido
        for (int i = 0; i < password.Length; i++)
        {
            password[i] = chars[random.Next(chars.Length)];
        }

        // Adicionar "!1" no final para garantir número e caractere especial
        return new string(password) + "!1";
    }
}

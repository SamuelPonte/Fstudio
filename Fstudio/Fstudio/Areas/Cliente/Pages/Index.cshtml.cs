// ============================================================================
// Cliente/Index.cshtml.cs
// Galeria privada do cliente — página principal da área do cliente
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Cliente.Pages;

/// <summary>
/// Modelo da página principal da área privada do cliente.
/// Apresenta a galeria de fotografias do evento do cliente autenticado.
///
/// Fluxo de acesso:
/// 1. Verificar que o utilizador autenticado existe
/// 2. Verificar que tem um registo de cliente associado
/// 3. Verificar que o cliente está em estado "Ativo" (não Pendente nem Inativo)
/// 4. Carregar as fotografias associadas ao cliente
///
/// Acesso restrito à role "Cliente".
/// </summary>
[Authorize(Roles = "Cliente")]
public class IndexModel : PageModel
{
    // ── Serviços ──────────────────────────────────────────────────────────
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Construtor — injeta o contexto da BD e o gestor de utilizadores.
    /// </summary>
    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context     = context;
        _userManager = userManager;
    }

    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Dados do cliente autenticado (nome, evento, estado, etc.).
    /// </summary>
    public Fstudio.Data.Models.Cliente? Cliente { get; set; }

    /// <summary>
    /// Fotografias associadas a este cliente, com os dados da fotografia incluídos.
    /// Ordenadas da mais recentemente adicionada à galeria.
    /// </summary>
    public List<ClienteFotografia> Fotografias { get; set; } = [];

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — verifica o estado do cliente e carrega as suas fotografias.
    /// Redireciona para a página inicial com aviso se o cliente não existir
    /// ou se a conta estiver em estado Pendente ou Inativo.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        // Obter o utilizador Identity autenticado
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            // Sessão expirou ou utilizador inválido — redirecionar para login
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        // Procurar o registo de cliente associado ao utilizador
        Cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        if (Cliente == null)
        {
            // O utilizador tem role "Cliente" mas sem registo na tabela Clientes
            // Pode acontecer se o admin criou a conta sem criar o registo de cliente
            TempData["Aviso"] = "A sua conta ainda não está associada a um cliente.";
            return RedirectToPage("/Index", new { area = "" });
        }

        // Verificar estado do cliente — só "Ativo" tem acesso à galeria
        if (Cliente.Estado == EstadoCliente.Pendente)
        {
            TempData["Aviso"] = "A sua conta ainda está pendente de aprovação pelo administrador.";
            return RedirectToPage("/Index", new { area = "" });
        }

        if (Cliente.Estado == EstadoCliente.Inativo)
        {
            TempData["Aviso"] = "A sua conta encontra-se inativa. Contacte o estúdio para mais informações.";
            return RedirectToPage("/Index", new { area = "" });
        }

        // Carregar fotografias da galeria privada com dados da foto e categoria
        Fotografias = await _context.ClienteFotografias
            .Include(cf => cf.Fotografia)
                .ThenInclude(f => f!.Categoria)  // ! porque Fotografia pode ser null em teoria
            .Where(cf => cf.ClienteId == Cliente.Id)
            .OrderByDescending(cf => cf.DataAdicao)  // Mais recentes primeiro
            .ToListAsync();

        return Page();
    }
}

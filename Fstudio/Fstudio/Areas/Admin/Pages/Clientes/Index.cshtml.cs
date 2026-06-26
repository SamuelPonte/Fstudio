// ============================================================================
// Admin/Clientes/Index.cshtml.cs
// Listagem de todos os clientes do estúdio
// ============================================================================

using Fstudio.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClienteEntity = Fstudio.Data.Models.Cliente;

namespace Fstudio.Areas.Admin.Pages.Clientes;

/// <summary>
/// Modelo da página de listagem de clientes na área de administração.
/// Apresenta todos os clientes ordenados do mais recente para o mais antigo,
/// com o número de fotografias associadas a cada um.
/// Acesso restrito à role "Admin".
/// </summary>
[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    // ── Serviços ──────────────────────────────────────────────────────────
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor — injeta o contexto da base de dados.
    /// </summary>
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Lista de clientes a apresentar na página.
    /// Inclui as relações ClienteFotografias para mostrar a contagem de fotos.
    /// O alias ClienteEntity evita conflito com a classe PageModel do ASP.NET.
    /// </summary>
    public List<ClienteEntity> Clientes { get; set; } = [];

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega todos os clientes com as respetivas fotografias,
    /// ordenados do mais recentemente criado para o mais antigo.
    /// </summary>
    public async Task OnGetAsync()
    {
        Clientes = await _context.Clientes
            .Include(c => c.ClienteFotografias)   // Para mostrar contagem de fotografias
            .OrderByDescending(c => c.DataCriacao) // Mais recentes primeiro
            .ToListAsync();
    }
}

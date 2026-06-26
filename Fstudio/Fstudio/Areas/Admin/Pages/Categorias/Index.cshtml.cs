// ============================================================================
// Admin/Categorias/Index.cshtml.cs
// Lista todas as categorias de fotografia para o administrador
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Categorias;

/// <summary>
/// Modelo da página de listagem de categorias na área de administração.
/// Apresenta todas as categorias ordenadas pela ordem de exibição definida,
/// incluindo o número de fotografias associadas a cada uma.
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
    /// Lista de categorias a apresentar na página.
    /// Inclui as fotografias associadas para permitir mostrar a contagem.
    /// </summary>
    public List<Categoria> Categorias { get; set; } = [];

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega todas as categorias com as respetivas fotografias,
    /// ordenadas pela ordem de exibição configurada pelo administrador.
    /// </summary>
    public async Task OnGetAsync()
    {
        Categorias = await _context.Categorias
            .Include(c => c.Fotografias)          // Necessário para mostrar o total de fotos
            .OrderBy(c => c.OrdemExibicao)        // Ordem definida pelo admin
            .ToListAsync();
    }
}

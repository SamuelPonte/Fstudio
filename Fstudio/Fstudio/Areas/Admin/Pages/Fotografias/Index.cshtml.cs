// ============================================================================
// Admin/Fotografias/Index.cshtml.cs
// Listagem de fotografias com filtro por categoria
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Fotografias;

/// <summary>
/// Modelo da página de listagem de fotografias na área de administração.
/// Apresenta todas as fotografias com suporte a filtro por categoria.
/// O filtro é mantido na URL via [BindProperty(SupportsGet = true)],
/// permitindo partilhar links filtrados.
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
    /// Lista de fotografias a apresentar, filtrada pela categoria selecionada.
    /// Inclui a categoria de cada fotografia para mostrar na tabela.
    /// </summary>
    public List<Fotografia> Fotografias { get; set; } = [];

    /// <summary>
    /// Lista de categorias para o dropdown de filtro.
    /// Construída com SelectList para integração com o tag helper asp-for.
    /// </summary>
    public SelectList Categorias { get; set; } = null!;

    /// <summary>
    /// ID da categoria selecionada no filtro.
    /// SupportsGet = true permite que o valor seja lido da query string (GET).
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public int? CategoriaId { get; set; }

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega as categorias para o dropdown e as fotografias filtradas.
    /// Se CategoriaId não for fornecido, apresenta todas as fotografias.
    /// Ordenadas da mais recente para a mais antiga.
    /// </summary>
    public async Task OnGetAsync()
    {
        // Preparar o dropdown de categorias para o filtro
        Categorias = new SelectList(
            await _context.Categorias.OrderBy(c => c.Nome).ToListAsync(),
            "Id", "Nome");

        // Construir query com filtro opcional de categoria
        var query = _context.Fotografias.Include(f => f.Categoria).AsQueryable();

        if (CategoriaId.HasValue)
        {
            // Aplicar filtro de categoria se selecionado
            query = query.Where(f => f.CategoriaId == CategoriaId.Value);
        }

        // Ordenar da mais recente para a mais antiga
        Fotografias = await query.OrderByDescending(f => f.DataCriacao).ToListAsync();
    }
}

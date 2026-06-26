// ============================================================================
// Pages/Portfolio/Index.cshtml.cs
// Galeria pública de fotografias do portfólio do Fstudio
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Pages.Portfolio;

/// <summary>
/// Modelo da página de portfólio público.
/// Apresenta a galeria de fotografias visíveis publicamente,
/// com suporte a filtro por categoria através do parâmetro "categoria" na URL.
///
/// Exemplos de URL:
/// - /Portfolio              → todas as fotografias do portfólio
/// - /Portfolio?categoria=casamentos → só fotografias da categoria com slug "casamentos"
///
/// As fotografias são ordenadas por: destaque primeiro, depois mais recentes.
/// </summary>
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
    /// Fotografias a apresentar na galeria, filtradas pela categoria selecionada.
    /// Inclui dados da categoria para apresentação.
    /// </summary>
    public List<Fotografia> Fotografias { get; set; } = [];

    /// <summary>
    /// Lista de todas as categorias ativas, para os botões de filtro.
    /// Ordenadas pela ordem de exibição configurada pelo admin.
    /// </summary>
    public List<Categoria> Categorias { get; set; } = [];

    /// <summary>
    /// Slug da categoria selecionada para filtro (ex: "casamentos").
    /// Vazio ou null significa "todas as categorias".
    /// SupportsGet = true permite ler o valor da query string.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? CategoriaSlug { get; set; }

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega as categorias e as fotografias, aplicando filtro se necessário.
    /// Fotografias ordenadas: destaques primeiro, depois por data decrescente.
    /// </summary>
    /// <param name="categoria">Slug da categoria para filtrar (opcional)</param>
    public async Task OnGetAsync(string? categoria)
    {
        // Guardar o slug atual para realçar o botão de filtro ativo na view
        CategoriaSlug = categoria;

        // Carregar todas as categorias ativas para os botões de filtro
        Categorias = await _context.Categorias
            .Where(c => c.Activa)
            .OrderBy(c => c.OrdemExibicao)
            .ToListAsync();

        // Query base: só fotografias visíveis no portfólio
        var query = _context.Fotografias
            .Include(f => f.Categoria)
            .Where(f => f.VisivelPortfolio);

        // Aplicar filtro de categoria se fornecido via query string
        if (!string.IsNullOrEmpty(categoria))
        {
            query = query.Where(f => f.Categoria != null && f.Categoria.Slug == categoria);
        }

        // Ordenar: destaques primeiro, depois por data decrescente
        Fotografias = await query
            .OrderByDescending(f => f.Destaque)
            .ThenByDescending(f => f.DataCriacao)
            .ToListAsync();
    }
}

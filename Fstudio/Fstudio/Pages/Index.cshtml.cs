// ============================================================================
// Pages/Index.cshtml.cs
// Página inicial (homepage) do site público do Fstudio
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Pages;

/// <summary>
/// Modelo da página inicial do site público.
/// Carrega os dados necessários para as secções dinâmicas da homepage:
/// - Fotografias em destaque para a secção de portfólio
/// - Testemunhos aprovados pelos clientes para a secção de testemunhos
///
/// As restantes secções da homepage (hero, sobre, contacto) são estáticas
/// e não necessitam de dados da base de dados.
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
    /// Até 6 fotografias marcadas como destaque e visíveis no portfólio.
    /// Apresentadas na secção de portfólio da homepage.
    /// </summary>
    public List<Fotografia> FotografiasDestaque { get; set; } = [];

    /// <summary>
    /// Até 3 testemunhos aprovados pelo administrador.
    /// Apresentados na secção de testemunhos da homepage.
    /// Inclui os dados do cliente para mostrar o nome do autor.
    /// </summary>
    public List<Testemunho> TestemunhosAprovados { get; set; } = [];

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega as fotografias em destaque e os testemunhos aprovados.
    /// Limita a 6 fotografias e 3 testemunhos para não sobrecarregar a página inicial.
    /// </summary>
    public async Task OnGetAsync()
    {
        // Fotografias em destaque e visíveis no portfólio — máximo 6 para a homepage
        FotografiasDestaque = await _context.Fotografias
            .Where(f => f.Destaque && f.VisivelPortfolio)
            .OrderByDescending(f => f.DataCriacao)
            .Take(6)
            .ToListAsync();

        // Testemunhos aprovados com dados do cliente (para o nome do autor)
        TestemunhosAprovados = await _context.Testemunhos
            .Include(t => t.Cliente)
            .Where(t => t.Aprovado)
            .OrderByDescending(t => t.DataAprovacao)  // Os mais recentemente aprovados primeiro
            .Take(3)
            .ToListAsync();
    }
}

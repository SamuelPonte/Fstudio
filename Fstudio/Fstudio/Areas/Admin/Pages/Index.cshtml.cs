// ============================================================================
// Admin/Index.cshtml.cs
// Dashboard principal da área de administração
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages;

/// <summary>
/// Modelo do dashboard principal da área de administração.
/// Apresenta uma visão geral do estado da aplicação com:
/// - Contadores de fotografias, clientes e notificações pendentes
/// - Últimas 5 mensagens de contacto recebidas
/// - Últimas 6 fotografias em destaque
///
/// É a primeira página que o administrador vê ao fazer login.
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

    // ── Propriedades (estatísticas e listas do dashboard) ─────────────────

    /// <summary>Total de fotografias registadas na base de dados.</summary>
    public int TotalFotografias { get; set; }

    /// <summary>Total de clientes registados.</summary>
    public int TotalClientes { get; set; }

    /// <summary>Número de mensagens de contacto ainda não lidas pelo admin.</summary>
    public int ContactosNaoLidos { get; set; }

    /// <summary>Número de testemunhos submetidos aguardando aprovação.</summary>
    public int TestemunhosPendentes { get; set; }

    /// <summary>
    /// Últimas 5 mensagens de contacto recebidas.
    /// Apresentadas no dashboard para acesso rápido às mensagens mais recentes.
    /// </summary>
    public List<Contacto> ContactosRecentes { get; set; } = [];

    /// <summary>
    /// Últimas 6 fotografias marcadas como destaque.
    /// Apresentadas no dashboard para visão rápida do portfólio em destaque.
    /// </summary>
    public List<Fotografia> FotografiasDestaque { get; set; } = [];

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega todas as estatísticas e listas do dashboard.
    /// Usa CountAsync para os contadores (mais eficiente que carregar todos os registos).
    /// </summary>
    public async Task OnGetAsync()
    {
        // Contadores — eficientes porque o SQL COUNT não traz dados desnecessários
        TotalFotografias     = await _context.Fotografias.CountAsync();
        TotalClientes        = await _context.Clientes.CountAsync();
        ContactosNaoLidos    = await _context.Contactos.CountAsync(c => !c.Lido);
        TestemunhosPendentes = await _context.Testemunhos.CountAsync(t => !t.Aprovado);

        // Últimas 5 mensagens de contacto para acesso rápido
        ContactosRecentes = await _context.Contactos
            .OrderByDescending(c => c.DataEnvio)
            .Take(5)
            .ToListAsync();

        // Últimas 6 fotografias em destaque para visão rápida do portfólio
        FotografiasDestaque = await _context.Fotografias
            .Where(f => f.Destaque)
            .OrderByDescending(f => f.DataCriacao)
            .Take(6)
            .ToListAsync();
    }
}

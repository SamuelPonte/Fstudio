// ============================================================================
// Admin/Testemunhos/Index.cshtml.cs
// Gestão de testemunhos submetidos por clientes
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Testemunhos;

/// <summary>
/// Modelo da página de gestão de testemunhos na área de administração.
/// Os testemunhos são submetidos pelos clientes e precisam de aprovação
/// do administrador antes de aparecerem publicamente no site.
///
/// Ações disponíveis:
/// - Aprovar: testemunho fica visível na página inicial do site público
/// - Rejeitar: reverte aprovação (não elimina, apenas oculta)
/// - Eliminar: remove permanentemente o testemunho
///
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
    /// Lista de todos os testemunhos, aprovados e pendentes.
    /// Inclui os dados do cliente para identificar o autor.
    /// </summary>
    public List<Testemunho> Testemunhos { get; set; } = [];

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega todos os testemunhos com os dados do cliente,
    /// ordenados do mais recente para o mais antigo.
    /// </summary>
    public async Task OnGetAsync()
    {
        Testemunhos = await _context.Testemunhos
            .Include(t => t.Cliente)          // Para mostrar o nome do cliente
            .OrderByDescending(t => t.DataCriacao)
            .ToListAsync();
    }

    /// <summary>
    /// POST — aprova um testemunho para exibição pública.
    /// Define Aprovado = true e regista a data/hora de aprovação.
    /// O testemunho passa a aparecer na página inicial do site.
    /// </summary>
    /// <param name="id">ID do testemunho a aprovar</param>
    public async Task<IActionResult> OnPostAprovarAsync(int id)
    {
        var testemunho = await _context.Testemunhos.FindAsync(id);
        if (testemunho != null)
        {
            testemunho.Aprovado       = true;
            testemunho.DataAprovacao  = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    /// <summary>
    /// POST — rejeita (ou reverte a aprovação de) um testemunho.
    /// O testemunho fica guardado mas deixa de aparecer no site público.
    /// </summary>
    /// <param name="id">ID do testemunho a rejeitar</param>
    public async Task<IActionResult> OnPostRejeitarAsync(int id)
    {
        var testemunho = await _context.Testemunhos.FindAsync(id);
        if (testemunho != null)
        {
            testemunho.Aprovado      = false;
            testemunho.DataAprovacao = null;  // Limpar data de aprovação
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    /// <summary>
    /// POST — elimina permanentemente um testemunho da base de dados.
    /// Esta ação não pode ser revertida.
    /// </summary>
    /// <param name="id">ID do testemunho a eliminar</param>
    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        var testemunho = await _context.Testemunhos.FindAsync(id);
        if (testemunho != null)
        {
            _context.Testemunhos.Remove(testemunho);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}

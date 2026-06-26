// ============================================================================
// Admin/Contactos/Index.cshtml.cs
// Listagem e gestão de mensagens de contacto recebidas
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Contactos;

/// <summary>
/// Modelo da página de listagem de contactos na área de administração.
/// Apresenta todas as mensagens de contacto recebidas, ordenadas das mais recentes.
/// Permite ao administrador arquivar mensagens diretamente da listagem.
/// Os contactos não lidos são destacados visualmente na view.
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
    /// Lista de mensagens de contacto a apresentar.
    /// Inclui todos os contactos, independentemente do estado de leitura ou arquivo.
    /// A view é responsável por diferenciar visualmente os não lidos.
    /// </summary>
    public List<Contacto> Contactos { get; set; } = [];

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega todos os contactos ordenados do mais recente para o mais antigo.
    /// </summary>
    public async Task OnGetAsync()
    {
        Contactos = await _context.Contactos
            .OrderByDescending(c => c.DataEnvio)
            .ToListAsync();
    }

    /// <summary>
    /// POST — arquiva um contacto específico diretamente da listagem.
    /// O contacto não é eliminado — apenas marcado como arquivado.
    /// Redireciona para a mesma página após arquivar.
    /// </summary>
    /// <param name="id">ID do contacto a arquivar</param>
    public async Task<IActionResult> OnPostArquivarAsync(int id)
    {
        var contacto = await _context.Contactos.FindAsync(id);
        if (contacto != null)
        {
            contacto.Arquivado = true;
            await _context.SaveChangesAsync();
        }

        // Redirecionar para a mesma página (recarregar listagem atualizada)
        return RedirectToPage();
    }
}

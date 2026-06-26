// ============================================================================
// Admin/Contactos/Details.cshtml.cs
// Detalhe de uma mensagem de contacto
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fstudio.Areas.Admin.Pages.Contactos;

/// <summary>
/// Modelo da página de detalhe de uma mensagem de contacto.
/// Ao aceder ao detalhe, a mensagem é automaticamente marcada como lida
/// e é registada a data/hora de leitura.
/// Permite também arquivar a mensagem a partir desta página.
/// Acesso restrito à role "Admin".
/// </summary>
[Authorize(Roles = "Admin")]
public class DetailsModel : PageModel
{
    // ── Serviços ──────────────────────────────────────────────────────────
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor — injeta o contexto da base de dados.
    /// </summary>
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Mensagem de contacto a apresentar em detalhe.
    /// </summary>
    public Contacto? Contacto { get; set; }

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega o contacto e marca-o automaticamente como lido.
    /// A marcação de lido acontece na primeira vez que o admin abre o detalhe.
    /// Retorna 404 se o contacto não existir.
    /// </summary>
    /// <param name="id">ID do contacto a visualizar</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        Contacto = await _context.Contactos.FindAsync(id);
        if (Contacto == null)
        {
            return NotFound();
        }

        // Marcar como lido automaticamente ao abrir o detalhe
        // Só altera se ainda não estiver marcado (evita gravações desnecessárias)
        if (!Contacto.Lido)
        {
            Contacto.Lido       = true;
            Contacto.DataLeitura = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return Page();
    }

    /// <summary>
    /// POST — arquiva o contacto e redireciona para a listagem.
    /// O contacto não é eliminado — apenas marcado como arquivado
    /// para não aparecer na vista principal da listagem.
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

        return RedirectToPage("Index");
    }
}

// ============================================================================
// Admin/Categorias/Delete.cshtml.cs
// Eliminação de uma categoria de fotografia
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Categorias;

/// <summary>
/// Modelo da página de eliminação de categorias na área de administração.
/// Apresenta uma página de confirmação antes de eliminar definitivamente.
/// Regra de negócio: não é possível eliminar uma categoria que tenha fotografias
/// associadas — o utilizador deve primeiro mover ou eliminar as fotografias.
/// Acesso restrito à role "Admin".
/// </summary>
[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    // ── Serviços ──────────────────────────────────────────────────────────
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor — injeta o contexto da base de dados.
    /// </summary>
    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Categoria a eliminar, apresentada na página de confirmação.
    /// Inclui as fotografias associadas para validar a regra de negócio.
    /// </summary>
    [BindProperty]
    public Categoria? Categoria { get; set; }

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega a categoria com as suas fotografias e apresenta a confirmação.
    /// Retorna 404 se a categoria não existir.
    /// </summary>
    /// <param name="id">ID da categoria a eliminar</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Incluir fotografias para mostrar o total na página de confirmação
        Categoria = await _context.Categorias
            .Include(c => c.Fotografias)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (Categoria == null)
        {
            return NotFound();
        }

        return Page();
    }

    /// <summary>
    /// POST — confirma e executa a eliminação da categoria.
    /// Redireciona para a listagem sem eliminar se a categoria tiver fotografias.
    /// Elimina o registo da base de dados se não tiver fotografias associadas.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (Categoria?.Id == null)
        {
            return NotFound();
        }

        // Recarregar da base de dados para garantir dados atuais (evitar race conditions)
        var categoria = await _context.Categorias
            .Include(c => c.Fotografias)
            .FirstOrDefaultAsync(c => c.Id == Categoria.Id);

        if (categoria == null)
        {
            return NotFound();
        }

        // Regra de negócio: não eliminar se tiver fotografias associadas
        // Redireciona silenciosamente — a view já avisa o utilizador desta restrição
        if (categoria.Fotografias.Count > 0)
        {
            return RedirectToPage("Index");
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}

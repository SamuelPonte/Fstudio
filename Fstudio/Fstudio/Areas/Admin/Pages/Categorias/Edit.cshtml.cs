// ============================================================================
// Admin/Categorias/Edit.cshtml.cs
// Edição de uma categoria de fotografia existente
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Categorias;

/// <summary>
/// Modelo da página de edição de categorias na área de administração.
/// Permite ao administrador alterar os dados de uma categoria existente.
/// Trata concorrência: se outro utilizador alterar o registo simultaneamente,
/// lança uma exceção em vez de sobrescrever silenciosamente.
/// Acesso restrito à role "Admin".
/// </summary>
[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    // ── Serviços ──────────────────────────────────────────────────────────
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor — injeta o contexto da base de dados.
    /// </summary>
    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Dados da categoria a editar, ligados ao formulário HTML.
    /// Inicializado com null! porque é sempre preenchido no OnGetAsync antes do POST.
    /// </summary>
    [BindProperty]
    public Categoria Categoria { get; set; } = null!;

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega a categoria pelo ID e apresenta o formulário preenchido.
    /// Retorna 404 se a categoria não existir.
    /// </summary>
    /// <param name="id">ID da categoria a editar</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
        {
            return NotFound();
        }

        Categoria = categoria;
        return Page();
    }

    /// <summary>
    /// POST — valida e guarda as alterações à categoria na base de dados.
    /// Usa Attach + EntityState.Modified para atualizar apenas os campos alterados.
    /// Trata DbUpdateConcurrencyException: verifica se o registo ainda existe
    /// antes de relançar a exceção.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Attach marca a entidade como modificada sem fazer um SELECT primeiro
        // Mais eficiente que FindAsync + alterar campos + SaveChanges
        _context.Attach(Categoria).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Verificar se o registo foi eliminado entretanto por outro utilizador
            if (!await _context.Categorias.AnyAsync(c => c.Id == Categoria.Id))
            {
                return NotFound();
            }
            throw; // Relançar se o registo existe — conflito de concorrência real
        }

        return RedirectToPage("Index");
    }
}

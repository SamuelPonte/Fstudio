// ============================================================================
// Admin/Fotografias/Delete.cshtml.cs
// Eliminação de uma fotografia e do ficheiro de imagem associado
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Fotografias;

/// <summary>
/// Modelo da página de eliminação de fotografias na área de administração.
/// Apresenta uma confirmação antes de eliminar definitivamente.
/// Ao eliminar, remove tanto o registo da base de dados como o ficheiro
/// de imagem físico do servidor (se existir), evitando ficheiros órfãos.
/// Acesso restrito à role "Admin".
/// </summary>
[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    // ── Serviços ──────────────────────────────────────────────────────────
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment; // Para obter o caminho físico do wwwroot

    /// <summary>
    /// Construtor — injeta o contexto da BD e o ambiente web (para caminhos de ficheiros).
    /// </summary>
    public DeleteModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context     = context;
        _environment = environment;
    }

    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Fotografia a eliminar, apresentada na página de confirmação.
    /// Inclui a categoria para mostrar informação completa ao administrador.
    /// </summary>
    [BindProperty]
    public Fotografia? Fotografia { get; set; }

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega a fotografia com a sua categoria e apresenta a confirmação.
    /// Retorna 404 se a fotografia não existir.
    /// </summary>
    /// <param name="id">ID da fotografia a eliminar</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Incluir categoria para mostrar na página de confirmação
        Fotografia = await _context.Fotografias
            .Include(f => f.Categoria)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (Fotografia == null)
        {
            return NotFound();
        }

        return Page();
    }

    /// <summary>
    /// POST — confirma e executa a eliminação da fotografia.
    /// Remove primeiro o ficheiro físico do servidor (para não deixar ficheiros órfãos),
    /// depois elimina o registo da base de dados.
    /// As relações ClienteFotografia são eliminadas em cascata pelo EF Core.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (Fotografia?.Id == null)
        {
            return NotFound();
        }

        var fotografia = await _context.Fotografias.FindAsync(Fotografia.Id);
        if (fotografia == null)
        {
            return NotFound();
        }

        // Eliminar o ficheiro físico da imagem do servidor
        // Importante: fazer antes de eliminar o registo para ter acesso ao URL
        if (!string.IsNullOrEmpty(fotografia.ImagemUrl))
        {
            // Converter URL relativo (/uploads/...) para caminho físico absoluto
            var imagePath = Path.Combine(_environment.WebRootPath, fotografia.ImagemUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        // Eliminar o registo da base de dados
        // As ClienteFotografias associadas são eliminadas em cascata
        _context.Fotografias.Remove(fotografia);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}

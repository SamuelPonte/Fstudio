// ============================================================================
// Admin/Categorias/Create.cshtml.cs
// Criação de uma nova categoria de fotografia
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fstudio.Areas.Admin.Pages.Categorias;

/// <summary>
/// Modelo da página de criação de categorias na área de administração.
/// Permite ao administrador criar uma nova categoria para organizar o portfólio.
/// Acesso restrito à role "Admin".
/// </summary>
[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    // ── Serviços ──────────────────────────────────────────────────────────
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor — injeta o contexto da base de dados.
    /// </summary>
    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Dados da nova categoria submetidos pelo formulário.
    /// Ligado ao formulário HTML via [BindProperty].
    /// </summary>
    [BindProperty]
    public Categoria Categoria { get; set; } = new();

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — apresenta o formulário de criação de categoria vazio.
    /// </summary>
    public IActionResult OnGet()
    {
        return Page();
    }

    /// <summary>
    /// POST — valida e guarda a nova categoria na base de dados.
    /// Define automaticamente a data de criação antes de guardar.
    /// Redireciona para a listagem após criação bem sucedida.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        // Validar as anotações de dados do modelo (Required, StringLength, etc.)
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Definir a data de criação automaticamente no servidor
        Categoria.DataCriacao = DateTime.UtcNow;

        _context.Categorias.Add(Categoria);
        await _context.SaveChangesAsync();

        // Redirecionar para a listagem após criação
        return RedirectToPage("Index");
    }
}

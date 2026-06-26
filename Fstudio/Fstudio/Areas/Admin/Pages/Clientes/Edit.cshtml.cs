// ============================================================================
// Admin/Clientes/Edit.cshtml.cs
// Edição dos dados de um cliente existente
// ============================================================================

using Fstudio.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClienteEntity = Fstudio.Data.Models.Cliente;

namespace Fstudio.Areas.Admin.Pages.Clientes;

/// <summary>
/// Modelo da página de edição de clientes na área de administração.
/// Permite ao administrador alterar dados do cliente (nome, email, telefone,
/// data do evento, tipo de serviço, estado, etc.).
/// Nota: a alteração do estado (Pendente → Ativo) ativa o acesso do cliente
/// à sua área privada de galeria.
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
    /// Dados do cliente a editar, ligados ao formulário HTML.
    /// </summary>
    [BindProperty]
    public ClienteEntity Cliente { get; set; } = null!;

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega o cliente pelo ID e apresenta o formulário preenchido.
    /// Retorna 404 se o cliente não existir.
    /// </summary>
    /// <param name="id">ID do cliente a editar</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
        {
            return NotFound();
        }

        Cliente = cliente;
        return Page();
    }

    /// <summary>
    /// POST — valida e guarda as alterações ao cliente na base de dados.
    /// Usa Attach + EntityState.Modified para atualizar eficientemente.
    /// Trata concorrência: verifica se o registo ainda existe se ocorrer conflito.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Attach marca todos os campos como modificados sem SELECT prévio
        _context.Attach(Cliente).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Verificar se o cliente ainda existe (pode ter sido eliminado)
            if (!await _context.Clientes.AnyAsync(c => c.Id == Cliente.Id))
            {
                return NotFound();
            }
            throw;
        }

        return RedirectToPage("Index");
    }
}

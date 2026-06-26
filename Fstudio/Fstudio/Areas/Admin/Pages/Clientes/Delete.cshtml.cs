// ============================================================================
// Admin/Clientes/Delete.cshtml.cs
// Eliminação de um cliente e da sua conta de utilizador associada
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClienteEntity = Fstudio.Data.Models.Cliente;

namespace Fstudio.Areas.Admin.Pages.Clientes;

/// <summary>
/// Modelo da página de eliminação de clientes na área de administração.
/// Apresenta uma confirmação antes de eliminar.
/// Ao eliminar um cliente, elimina também a conta de utilizador Identity associada
/// (se existir), garantindo que não ficam utilizadores órfãos na base de dados.
/// As relações ClienteFotografia são eliminadas em cascata pelo EF Core.
/// Acesso restrito à role "Admin".
/// </summary>
[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    // ── Serviços ──────────────────────────────────────────────────────────
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Construtor — injeta o contexto da BD e o gestor de utilizadores do Identity.
    /// </summary>
    public DeleteModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context     = context;
        _userManager = userManager;
    }

    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Cliente a eliminar, apresentado na página de confirmação.
    /// </summary>
    [BindProperty]
    public ClienteEntity? Cliente { get; set; }

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega o cliente com as suas fotografias e apresenta a confirmação.
    /// Retorna 404 se o cliente não existir.
    /// </summary>
    /// <param name="id">ID do cliente a eliminar</param>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        // Incluir fotografias para mostrar o impacto da eliminação na view
        Cliente = await _context.Clientes
            .Include(c => c.ClienteFotografias)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (Cliente == null)
        {
            return NotFound();
        }

        return Page();
    }

    /// <summary>
    /// POST — confirma e executa a eliminação do cliente e da sua conta Identity.
    /// Ordem: eliminar utilizador Identity → eliminar registo de cliente.
    /// As ClienteFotografias são eliminadas em cascata pelo EF Core.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (Cliente?.Id == null)
        {
            return NotFound();
        }

        // Recarregar da base de dados para dados atuais
        var cliente = await _context.Clientes
            .Include(c => c.ClienteFotografias)
            .FirstOrDefaultAsync(c => c.Id == Cliente.Id);

        if (cliente == null)
        {
            return NotFound();
        }

        // Eliminar a conta de utilizador Identity se existir
        // Importante: fazer antes de eliminar o cliente para evitar referências inválidas
        if (!string.IsNullOrEmpty(cliente.UserId))
        {
            var user = await _userManager.FindByIdAsync(cliente.UserId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        // Eliminar o cliente (as ClienteFotografias são eliminadas em cascata)
        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}

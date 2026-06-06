using Fstudio.Data;
using Fstudio.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClienteEntity = Fstudio.Models.Entities.Cliente;

namespace Fstudio.Areas.Admin.Pages.Clientes;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public ClienteEntity? Cliente { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Cliente = await _context.Clientes
            .Include(c => c.ClienteFotografias)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (Cliente == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Cliente?.Id == null)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes
            .Include(c => c.ClienteFotografias)
            .FirstOrDefaultAsync(c => c.Id == Cliente.Id);

        if (cliente == null)
        {
            return NotFound();
        }

        // Delete associated user if exists
        if (!string.IsNullOrEmpty(cliente.UserId))
        {
            var user = await _userManager.FindByIdAsync(cliente.UserId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}

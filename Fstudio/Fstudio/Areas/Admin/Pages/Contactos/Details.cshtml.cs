using Fstudio.Data;
using Fstudio.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fstudio.Areas.Admin.Pages.Contactos;

[Authorize(Roles = "Admin")]
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Contacto? Contacto { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Contacto = await _context.Contactos.FindAsync(id);
        if (Contacto == null)
        {
            return NotFound();
        }

        // Mark as read
        if (!Contacto.Lido)
        {
            Contacto.Lido = true;
            Contacto.DataLeitura = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return Page();
    }

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

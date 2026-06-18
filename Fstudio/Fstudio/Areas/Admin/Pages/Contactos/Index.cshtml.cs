using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Contactos;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Contacto> Contactos { get; set; } = [];

    public async Task OnGetAsync()
    {
        Contactos = await _context.Contactos
            .OrderByDescending(c => c.DataEnvio)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostArquivarAsync(int id)
    {
        var contacto = await _context.Contactos.FindAsync(id);
        if (contacto != null)
        {
            contacto.Arquivado = true;
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}

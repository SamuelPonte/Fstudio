using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Categorias;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Categoria? Categoria { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Categoria = await _context.Categorias
            .Include(c => c.Fotografias)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (Categoria == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Categoria?.Id == null)
        {
            return NotFound();
        }

        var categoria = await _context.Categorias
            .Include(c => c.Fotografias)
            .FirstOrDefaultAsync(c => c.Id == Categoria.Id);

        if (categoria == null)
        {
            return NotFound();
        }

        if (categoria.Fotografias.Count > 0)
        {
            return RedirectToPage("Index");
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}

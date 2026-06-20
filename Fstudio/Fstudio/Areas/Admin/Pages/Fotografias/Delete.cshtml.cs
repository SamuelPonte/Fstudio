using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Fotografias;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public DeleteModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [BindProperty]
    public Fotografia? Fotografia { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Fotografia = await _context.Fotografias
            .Include(f => f.Categoria)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (Fotografia == null)
        {
            return NotFound();
        }

        return Page();
    }

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

        // Remove o ficheiro físico da imagem antes de apagar o registo da base de dados.
        if (!string.IsNullOrEmpty(fotografia.ImagemUrl))
        {
            var imagePath = Path.Combine(_environment.WebRootPath, fotografia.ImagemUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        _context.Fotografias.Remove(fotografia);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}

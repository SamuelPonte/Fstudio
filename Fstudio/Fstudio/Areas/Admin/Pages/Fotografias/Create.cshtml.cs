using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Fotografias;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public CreateModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [BindProperty]
    public Fotografia Fotografia { get; set; } = new() { VisivelPortfolio = true };

    public SelectList Categorias { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync()
    {
        Categorias = new SelectList(
            await _context.Categorias.Where(c => c.Activa).OrderBy(c => c.Nome).ToListAsync(),
            "Id", "Nome");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(IFormFile? ImagemFile)
    {
        Categorias = new SelectList(
            await _context.Categorias.Where(c => c.Activa).OrderBy(c => c.Nome).ToListAsync(),
            "Id", "Nome");

        if (ImagemFile == null || ImagemFile.Length == 0)
        {
            ModelState.AddModelError("ImagemFile", "A imagem é obrigatória");
            return Page();
        }

        // Remove ImagemUrl validation since we'll set it from the file
        ModelState.Remove("Fotografia.ImagemUrl");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Save the image
        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "fotografias");
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImagemFile.FileName)}";
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await ImagemFile.CopyToAsync(stream);
        }

        Fotografia.ImagemUrl = $"/uploads/fotografias/{fileName}";
        Fotografia.ThumbnailUrl = Fotografia.ImagemUrl;
        Fotografia.DataCriacao = DateTime.UtcNow;

        _context.Fotografias.Add(Fotografia);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}

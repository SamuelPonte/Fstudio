using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Fotografias;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public EditModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [BindProperty]
    public Fotografia Fotografia { get; set; } = null!;

    public SelectList Categorias { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var fotografia = await _context.Fotografias.FindAsync(id);
        if (fotografia == null)
        {
            return NotFound();
        }

        Fotografia = fotografia;
        Categorias = new SelectList(
            await _context.Categorias.Where(c => c.Activa).OrderBy(c => c.Nome).ToListAsync(),
            "Id", "Nome");

        return Page();
    }

    private static readonly string[] _tiposPermitidos = ["image/jpeg", "image/png", "image/webp", "image/gif"];
    private const long _tamanhoMaximo = 10 * 1024 * 1024; // 10 MB

    public async Task<IActionResult> OnPostAsync(IFormFile? ImagemFile)
    {
        Categorias = new SelectList(
            await _context.Categorias.Where(c => c.Activa).OrderBy(c => c.Nome).ToListAsync(),
            "Id", "Nome");

        if (ImagemFile != null && ImagemFile.Length > 0)
        {
            if (!_tiposPermitidos.Contains(ImagemFile.ContentType.ToLower()))
            {
                ModelState.AddModelError("ImagemFile", "Formato não suportado. Use JPG, PNG, WebP ou GIF.");
                return Page();
            }

            if (ImagemFile.Length > _tamanhoMaximo)
            {
                ModelState.AddModelError("ImagemFile", "A imagem não pode exceder 10 MB.");
                return Page();
            }
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existing = await _context.Fotografias.FindAsync(Fotografia.Id);
        if (existing == null)
        {
            return NotFound();
        }

        // Handle new image upload
        if (ImagemFile != null && ImagemFile.Length > 0)
        {
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "fotografias");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImagemFile.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ImagemFile.CopyToAsync(stream);
            }

            // Delete old image if it exists
            if (!string.IsNullOrEmpty(existing.ImagemUrl))
            {
                var oldPath = Path.Combine(_environment.WebRootPath, existing.ImagemUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            existing.ImagemUrl = $"/uploads/fotografias/{fileName}";
            existing.ThumbnailUrl = existing.ImagemUrl;
        }

        existing.Titulo = Fotografia.Titulo;
        existing.Descricao = Fotografia.Descricao;
        existing.CategoriaId = Fotografia.CategoriaId;
        existing.DataSessao = Fotografia.DataSessao;
        existing.Destaque = Fotografia.Destaque;
        existing.VisivelPortfolio = Fotografia.VisivelPortfolio;

        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}

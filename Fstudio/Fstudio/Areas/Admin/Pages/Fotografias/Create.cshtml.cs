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

    // Configuração da validação do upload de imagens.
    // São aceites apenas os formatos usados no portfólio e é definido um limite máximo de 10 MB.
    private static readonly string[] _extensoesPermitidas = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] _tiposPermitidos = ["image/jpeg", "image/png", "image/webp"];
    private const long _tamanhoMaximo = 10 * 1024 * 1024; // 10 MB

    public async Task<IActionResult> OnPostAsync(IFormFile? ImagemFile)
    {
        Categorias = new SelectList(
            await _context.Categorias.Where(c => c.Activa).OrderBy(c => c.Nome).ToListAsync(),
            "Id", "Nome");

        // Valida se foi enviada uma imagem e se o ficheiro respeita as regras definidas.
        if (ImagemFile == null || ImagemFile.Length == 0)
        {
            ModelState.AddModelError("ImagemFile", "A imagem é obrigatória.");
            return Page();
        }

        var extensao = Path.GetExtension(ImagemFile.FileName).ToLowerInvariant();

        if (!_extensoesPermitidas.Contains(extensao))
        {
            ModelState.AddModelError("ImagemFile", "Só são permitidas imagens nos formatos JPG, JPEG, PNG ou WEBP.");
            return Page();
        }

        if (!_tiposPermitidos.Contains(ImagemFile.ContentType.ToLowerInvariant()))
        {
            ModelState.AddModelError("ImagemFile", "Formato não suportado. Use JPG, PNG ou WebP.");
            return Page();
        }

        if (ImagemFile.Length > _tamanhoMaximo)
        {
            ModelState.AddModelError("ImagemFile", "A imagem não pode exceder 10 MB.");
            return Page();
        }

        // Remove a validação de ImagemUrl, porque o caminho da imagem é definido automaticamente após o upload.
        ModelState.Remove("Fotografia.ImagemUrl");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Guarda a imagem na pasta wwwroot/uploads/fotografias com um nome único.
        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "fotografias");
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"{Guid.NewGuid()}{extensao}";
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

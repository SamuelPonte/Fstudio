using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Cliente.Pages;

[Authorize(Roles = "Cliente")]
public class DownloadModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _environment;

    public DownloadModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment environment)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
    }

    public bool SinglePhoto { get; set; }
    public int TotalFotos { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        if (cliente == null)
        {
            TempData["Aviso"] = "A sua conta ainda não está associada a um cliente.";
            return RedirectToPage("/Index", new { area = "" });
        }

        if (cliente.Estado == EstadoCliente.Pendente)
        {
            TempData["Aviso"] = "A sua conta ainda está pendente de aprovação pelo administrador.";
            return RedirectToPage("/Index", new { area = "" });
        }

        if (cliente.Estado == EstadoCliente.Inativo)
        {
            TempData["Aviso"] = "A sua conta encontra-se inativa.";
            return RedirectToPage("/Index", new { area = "" });
        }

        if (id.HasValue)
        {
            // Download single photo
            SinglePhoto = true;

            var cf = await _context.ClienteFotografias
                .Include(cf => cf.Fotografia)
                .FirstOrDefaultAsync(cf => cf.ClienteId == cliente.Id && cf.FotografiaId == id.Value);

            if (cf?.Fotografia == null)
            {
                return NotFound();
            }

            // Mark as downloaded
            cf.Descarregada = true;
            cf.DataDownload = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Return the file
            var filePath = Path.Combine(_environment.WebRootPath, cf.Fotografia.ImagemUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                var fileName = $"{cf.Fotografia.Titulo}{Path.GetExtension(filePath)}";
                return PhysicalFile(filePath, "image/jpeg", fileName);
            }

            return Page();
        }
        else
        {
            // Download all photos (show info page)
            TotalFotos = await _context.ClienteFotografias
                .CountAsync(cf => cf.ClienteId == cliente.Id);

            // Mark all as downloaded
            var fotos = await _context.ClienteFotografias
                .Where(cf => cf.ClienteId == cliente.Id && !cf.Descarregada)
                .ToListAsync();

            foreach (var foto in fotos)
            {
                foto.Descarregada = true;
                foto.DataDownload = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();

            return Page();
        }
    }
}

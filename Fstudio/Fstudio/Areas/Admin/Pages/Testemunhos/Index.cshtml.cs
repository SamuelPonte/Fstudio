using Fstudio.Data;
using Fstudio.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Testemunhos;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Testemunho> Testemunhos { get; set; } = [];

    public async Task OnGetAsync()
    {
        Testemunhos = await _context.Testemunhos
            .Include(t => t.Cliente)
            .OrderByDescending(t => t.DataCriacao)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAprovarAsync(int id)
    {
        var testemunho = await _context.Testemunhos.FindAsync(id);
        if (testemunho != null)
        {
            testemunho.Aprovado = true;
            testemunho.DataAprovacao = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejeitarAsync(int id)
    {
        var testemunho = await _context.Testemunhos.FindAsync(id);
        if (testemunho != null)
        {
            testemunho.Aprovado = false;
            testemunho.DataAprovacao = null;
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        var testemunho = await _context.Testemunhos.FindAsync(id);
        if (testemunho != null)
        {
            _context.Testemunhos.Remove(testemunho);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}

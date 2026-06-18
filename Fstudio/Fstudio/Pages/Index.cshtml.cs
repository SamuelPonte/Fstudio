using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Fotografia> FotografiasDestaque { get; set; } = [];
    public List<Testemunho> TestemunhosAprovados { get; set; } = [];

    public async Task OnGetAsync()
    {
        FotografiasDestaque = await _context.Fotografias
            .Where(f => f.Destaque && f.VisivelPortfolio)
            .OrderByDescending(f => f.DataCriacao)
            .Take(6)
            .ToListAsync();

        TestemunhosAprovados = await _context.Testemunhos
            .Include(t => t.Cliente)
            .Where(t => t.Aprovado)
            .OrderByDescending(t => t.DataAprovacao)
            .Take(3)
            .ToListAsync();
    }
}

using Fstudio.Data;
using Fstudio.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Pages.Portfolio;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Fotografia> Fotografias { get; set; } = [];
    public List<Categoria> Categorias { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? CategoriaSlug { get; set; }

    public async Task OnGetAsync(string? categoria)
    {
        CategoriaSlug = categoria;

        Categorias = await _context.Categorias
            .Where(c => c.Activa)
            .OrderBy(c => c.OrdemExibicao)
            .ToListAsync();

        var query = _context.Fotografias
            .Include(f => f.Categoria)
            .Where(f => f.VisivelPortfolio);

        if (!string.IsNullOrEmpty(categoria))
        {
            query = query.Where(f => f.Categoria != null && f.Categoria.Slug == categoria);
        }

        Fotografias = await query
            .OrderByDescending(f => f.Destaque)
            .ThenByDescending(f => f.DataCriacao)
            .ToListAsync();
    }
}

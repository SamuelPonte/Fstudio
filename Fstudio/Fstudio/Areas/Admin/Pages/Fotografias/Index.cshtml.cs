using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Fotografias;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Fotografia> Fotografias { get; set; } = [];
    public SelectList Categorias { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public int? CategoriaId { get; set; }

    public async Task OnGetAsync()
    {
        Categorias = new SelectList(await _context.Categorias.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");

        // Carrega as fotografias com a respetiva categoria, aplicando o filtro de categoria quando selecionado.
        var query = _context.Fotografias.Include(f => f.Categoria).AsQueryable();

        if (CategoriaId.HasValue)
        {
            query = query.Where(f => f.CategoriaId == CategoriaId.Value);
        }

        Fotografias = await query.OrderByDescending(f => f.DataCriacao).ToListAsync();
    }
}

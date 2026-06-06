using Fstudio.Data;
using Fstudio.Models.Entities;
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

        var query = _context.Fotografias.Include(f => f.Categoria).AsQueryable();

        if (CategoriaId.HasValue)
        {
            query = query.Where(f => f.CategoriaId == CategoriaId.Value);
        }

        Fotografias = await query.OrderByDescending(f => f.DataCriacao).ToListAsync();
    }
}

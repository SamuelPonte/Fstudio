using Fstudio.Data;
using Fstudio.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Categorias;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Categoria> Categorias { get; set; } = [];

    public async Task OnGetAsync()
    {
        Categorias = await _context.Categorias
            .Include(c => c.Fotografias)
            .OrderBy(c => c.OrdemExibicao)
            .ToListAsync();
    }
}

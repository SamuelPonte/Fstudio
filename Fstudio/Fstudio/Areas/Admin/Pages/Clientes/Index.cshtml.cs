using Fstudio.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClienteEntity = Fstudio.Models.Entities.Cliente;

namespace Fstudio.Areas.Admin.Pages.Clientes;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<ClienteEntity> Clientes { get; set; } = [];

    public async Task OnGetAsync()
    {
        Clientes = await _context.Clientes
            .Include(c => c.ClienteFotografias)
            .OrderByDescending(c => c.DataCriacao)
            .ToListAsync();
    }
}

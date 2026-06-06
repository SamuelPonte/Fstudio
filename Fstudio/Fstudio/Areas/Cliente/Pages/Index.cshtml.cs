using Fstudio.Data;
using Fstudio.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Cliente.Pages;

[Authorize(Roles = "Cliente")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Fstudio.Models.Entities.Cliente? Cliente { get; set; }
    public List<ClienteFotografia> Fotografias { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        Cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        if (Cliente == null)
        {
            return RedirectToPage("/Index");
        }

        Fotografias = await _context.ClienteFotografias
            .Include(cf => cf.Fotografia)
            .ThenInclude(f => f!.Categoria)
            .Where(cf => cf.ClienteId == Cliente.Id)
            .OrderByDescending(cf => cf.DataAdicao)
            .ToListAsync();

        return Page();
    }
}

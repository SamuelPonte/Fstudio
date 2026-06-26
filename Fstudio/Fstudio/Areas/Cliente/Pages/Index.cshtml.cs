using Fstudio.Data;
using Fstudio.Data.Models;
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

    public Fstudio.Data.Models.Cliente? Cliente { get; set; }
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
            TempData["Aviso"] = "A sua conta ainda não está associada a um cliente.";
            return RedirectToPage("/Index", new { area = "" });
        }

        if (Cliente.Estado == EstadoCliente.Pendente)
        {
            TempData["Aviso"] = "A sua conta ainda está pendente de aprovação pelo administrador.";
            return RedirectToPage("/Index", new { area = "" });
        }

        if (Cliente.Estado == EstadoCliente.Inativo)
        {
            TempData["Aviso"] = "A sua conta encontra-se inativa.";
            return RedirectToPage("/Index", new { area = "" });
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

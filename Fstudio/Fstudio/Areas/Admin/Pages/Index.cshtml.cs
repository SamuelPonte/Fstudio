using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int TotalFotografias { get; set; }
    public int TotalClientes { get; set; }
    public int ContactosNaoLidos { get; set; }
    public int TestemunhosPendentes { get; set; }
    public List<Contacto> ContactosRecentes { get; set; } = [];
    public List<Fotografia> FotografiasDestaque { get; set; } = [];

    public async Task OnGetAsync()
    {
        TotalFotografias = await _context.Fotografias.CountAsync();
        TotalClientes = await _context.Clientes.CountAsync();
        ContactosNaoLidos = await _context.Contactos.CountAsync(c => !c.Lido);
        TestemunhosPendentes = await _context.Testemunhos.CountAsync(t => !t.Aprovado);

        ContactosRecentes = await _context.Contactos
            .OrderByDescending(c => c.DataEnvio)
            .Take(5)
            .ToListAsync();

        FotografiasDestaque = await _context.Fotografias
            .Where(f => f.Destaque)
            .OrderByDescending(f => f.DataCriacao)
            .Take(6)
            .ToListAsync();
    }
}

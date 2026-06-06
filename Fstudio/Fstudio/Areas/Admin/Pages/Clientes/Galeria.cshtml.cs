using Fstudio.Data;
using Fstudio.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClienteEntity = Fstudio.Models.Entities.Cliente;

namespace Fstudio.Areas.Admin.Pages.Clientes;

[Authorize(Roles = "Admin")]
public class GaleriaModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public GaleriaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public ClienteEntity? Cliente { get; set; }
    public List<ClienteFotografia> FotografiasCliente { get; set; } = [];
    public List<Fotografia> FotografiasDisponiveis { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Cliente = await _context.Clientes.FindAsync(id);
        if (Cliente == null)
        {
            return NotFound();
        }

        await LoadDataAsync(id);
        return Page();
    }

    public async Task<IActionResult> OnPostAdicionarFotosAsync(int id, int[] fotografiaIds)
    {
        Cliente = await _context.Clientes.FindAsync(id);
        if (Cliente == null)
        {
            return NotFound();
        }

        var existingIds = await _context.ClienteFotografias
            .Where(cf => cf.ClienteId == id)
            .Select(cf => cf.FotografiaId)
            .ToListAsync();

        foreach (var fotoId in fotografiaIds.Where(fid => !existingIds.Contains(fid)))
        {
            _context.ClienteFotografias.Add(new ClienteFotografia
            {
                ClienteId = id,
                FotografiaId = fotoId,
                DataAdicao = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
        await LoadDataAsync(id);

        return Page();
    }

    public async Task<IActionResult> OnPostRemoverFotoAsync(int id, int clienteFotografiaId)
    {
        var cf = await _context.ClienteFotografias.FindAsync(clienteFotografiaId);
        if (cf != null)
        {
            _context.ClienteFotografias.Remove(cf);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    private async Task LoadDataAsync(int clienteId)
    {
        FotografiasCliente = await _context.ClienteFotografias
            .Include(cf => cf.Fotografia)
            .Where(cf => cf.ClienteId == clienteId)
            .OrderByDescending(cf => cf.DataAdicao)
            .ToListAsync();

        var idsNaGaleria = FotografiasCliente.Select(cf => cf.FotografiaId).ToList();

        FotografiasDisponiveis = await _context.Fotografias
            .Include(f => f.Categoria)
            .Where(f => !idsNaGaleria.Contains(f.Id))
            .OrderByDescending(f => f.DataCriacao)
            .ToListAsync();
    }
}

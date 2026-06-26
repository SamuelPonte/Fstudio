// ============================================================================
// Admin/Clientes/Galeria.cshtml.cs
// Gestão da galeria privada de um cliente
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClienteEntity = Fstudio.Data.Models.Cliente;

namespace Fstudio.Areas.Admin.Pages.Clientes;

/// <summary>
/// Modelo da página de gestão da galeria privada de um cliente.
/// Permite ao administrador:
/// - Ver as fotografias já associadas ao cliente
/// - Adicionar fotografias do portfólio à galeria do cliente
/// - Remover fotografias da galeria do cliente
///
/// A relação entre clientes e fotografias é feita através da tabela
/// ClienteFotografia (relação N:M). Esta página gere essa relação.
/// Acesso restrito à role "Admin".
/// </summary>
[Authorize(Roles = "Admin")]
public class GaleriaModel : PageModel
{
    // ── Serviços ──────────────────────────────────────────────────────────
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor — injeta o contexto da base de dados.
    /// </summary>
    public GaleriaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Cliente cuja galeria está a ser gerida.
    /// </summary>
    public ClienteEntity? Cliente { get; set; }

    /// <summary>
    /// Fotografias já associadas à galeria deste cliente,
    /// ordenadas da mais recentemente adicionada.
    /// </summary>
    public List<ClienteFotografia> FotografiasCliente { get; set; } = [];

    /// <summary>
    /// Fotografias do portfólio ainda não associadas a este cliente,
    /// disponíveis para adicionar à galeria.
    /// </summary>
    public List<Fotografia> FotografiasDisponiveis { get; set; } = [];

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — carrega o cliente e as listas de fotografias associadas e disponíveis.
    /// Retorna 404 se o cliente não existir.
    /// </summary>
    /// <param name="id">ID do cliente</param>
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

    /// <summary>
    /// POST — adiciona um conjunto de fotografias à galeria do cliente.
    /// Verifica quais as fotografias que já estão associadas para evitar duplicados.
    /// Após adicionar, recarrega os dados e volta à mesma página.
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <param name="fotografiaIds">Array de IDs das fotografias a adicionar</param>
    public async Task<IActionResult> OnPostAdicionarFotosAsync(int id, int[] fotografiaIds)
    {
        Cliente = await _context.Clientes.FindAsync(id);
        if (Cliente == null)
        {
            return NotFound();
        }

        // Obter IDs das fotografias já associadas para evitar duplicados
        var existingIds = await _context.ClienteFotografias
            .Where(cf => cf.ClienteId == id)
            .Select(cf => cf.FotografiaId)
            .ToListAsync();

        // Adicionar apenas as fotografias que ainda não estão na galeria
        foreach (var fotoId in fotografiaIds.Where(fid => !existingIds.Contains(fid)))
        {
            _context.ClienteFotografias.Add(new ClienteFotografia
            {
                ClienteId    = id,
                FotografiaId = fotoId,
                DataAdicao   = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        // Recarregar dados e retornar à página atualizada
        await LoadDataAsync(id);
        return Page();
    }

    /// <summary>
    /// POST — remove uma fotografia específica da galeria do cliente.
    /// A fotografia em si não é eliminada — apenas a associação ClienteFotografia.
    /// Redireciona para a mesma página após a remoção.
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <param name="fotografiaId">ID da fotografia a remover</param>
    public async Task<IActionResult> OnPostRemoverFotoAsync(int id, int fotografiaId)
    {
        // Procurar a relação N:M pela chave composta (ClienteId, FotografiaId)
        var cf = await _context.ClienteFotografias.FindAsync(id, fotografiaId);
        if (cf != null)
        {
            _context.ClienteFotografias.Remove(cf);
            await _context.SaveChangesAsync();
        }

        // Redirecionar para a mesma página com o mesmo cliente
        return RedirectToPage(new { id });
    }

    // ── Métodos Privados ──────────────────────────────────────────────────

    /// <summary>
    /// Carrega as fotografias do cliente e as fotografias disponíveis (não associadas).
    /// Usado em múltiplos handlers para evitar duplicação de código.
    /// </summary>
    /// <param name="clienteId">ID do cliente</param>
    private async Task LoadDataAsync(int clienteId)
    {
        // Fotografias já na galeria do cliente
        FotografiasCliente = await _context.ClienteFotografias
            .Include(cf => cf.Fotografia)
            .Where(cf => cf.ClienteId == clienteId)
            .OrderByDescending(cf => cf.DataAdicao)
            .ToListAsync();

        // IDs já associados para filtrar as disponíveis
        var idsNaGaleria = FotografiasCliente.Select(cf => cf.FotografiaId).ToList();

        // Fotografias do portfólio ainda não associadas a este cliente
        FotografiasDisponiveis = await _context.Fotografias
            .Include(f => f.Categoria)
            .Where(f => !idsNaGaleria.Contains(f.Id))
            .OrderByDescending(f => f.DataCriacao)
            .ToListAsync();
    }
}

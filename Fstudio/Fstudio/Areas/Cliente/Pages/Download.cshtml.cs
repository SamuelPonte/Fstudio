// ============================================================================
// Cliente/Download.cshtml.cs
// Download de fotografias da galeria privada do cliente
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Cliente.Pages;

/// <summary>
/// Modelo da página de download de fotografias da área do cliente.
/// Suporta dois modos de operação:
/// - Download individual: fornece um ficheiro de imagem específico (id obrigatório)
/// - Download geral: mostra uma página informativa sobre o conjunto de fotos
///   e marca todas como descarregadas (o download do ZIP não está implementado)
///
/// Em ambos os casos, verifica que o cliente está ativo e que a fotografia
/// lhe pertence (prevenindo acesso não autorizado a fotos de outros clientes).
/// Acesso restrito à role "Cliente".
/// </summary>
[Authorize(Roles = "Cliente")]
public class DownloadModel : PageModel
{
    // ── Serviços ──────────────────────────────────────────────────────────
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _environment; // Para acesso ao caminho físico das imagens

    /// <summary>
    /// Construtor — injeta contexto da BD, gestor de utilizadores e ambiente web.
    /// </summary>
    public DownloadModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment environment)
    {
        _context     = context;
        _userManager = userManager;
        _environment = environment;
    }

    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Indica se é um download de fotografia individual (true) ou geral (false).
    /// Usado pela view para apresentar conteúdo diferente.
    /// </summary>
    public bool SinglePhoto { get; set; }

    /// <summary>
    /// Total de fotografias disponíveis para download (modo geral).
    /// </summary>
    public int TotalFotos { get; set; }

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — processa o download conforme o parâmetro id:
    /// - Com id: devolve o ficheiro de imagem diretamente (PhysicalFile)
    /// - Sem id: apresenta página informativa e marca todas as fotos como descarregadas
    ///
    /// Verifica estado do cliente antes de permitir qualquer download.
    /// </summary>
    /// <param name="id">ID da fotografia a descarregar (null = download geral)</param>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        // Verificar que o utilizador existe
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        // Verificar que tem registo de cliente
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        if (cliente == null)
        {
            TempData["Aviso"] = "A sua conta ainda não está associada a um cliente.";
            return RedirectToPage("/Index", new { area = "" });
        }

        // Verificar estado — só clientes Ativos podem descarregar
        if (cliente.Estado == EstadoCliente.Pendente)
        {
            TempData["Aviso"] = "A sua conta ainda está pendente de aprovação pelo administrador.";
            return RedirectToPage("/Index", new { area = "" });
        }

        if (cliente.Estado == EstadoCliente.Inativo)
        {
            TempData["Aviso"] = "A sua conta encontra-se inativa.";
            return RedirectToPage("/Index", new { area = "" });
        }

        if (id.HasValue)
        {
            // ── Modo: Download de fotografia individual ───────────────────
            SinglePhoto = true;

            // Verificar que esta fotografia pertence ao cliente (segurança)
            var cf = await _context.ClienteFotografias
                .Include(cf => cf.Fotografia)
                .FirstOrDefaultAsync(cf => cf.ClienteId == cliente.Id && cf.FotografiaId == id.Value);

            if (cf?.Fotografia == null)
            {
                // 404 se a foto não existe ou não pertence ao cliente
                return NotFound();
            }

            // Marcar como descarregada e registar data/hora
            cf.Descarregada = true;
            cf.DataDownload = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Converter URL relativo em caminho físico absoluto
            var filePath = Path.Combine(_environment.WebRootPath, cf.Fotografia.ImagemUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                // Devolver o ficheiro com nome amigável baseado no título da fotografia
                var fileName = $"{cf.Fotografia.Titulo}{Path.GetExtension(filePath)}";
                return PhysicalFile(filePath, "image/jpeg", fileName);
            }

            // Ficheiro não encontrado no servidor — mostrar página sem download
            return Page();
        }
        else
        {
            // ── Modo: Informação de download geral ────────────────────────
            TotalFotos = await _context.ClienteFotografias
                .CountAsync(cf => cf.ClienteId == cliente.Id);

            // Marcar todas as fotografias não descarregadas como descarregadas
            var fotos = await _context.ClienteFotografias
                .Where(cf => cf.ClienteId == cliente.Id && !cf.Descarregada)
                .ToListAsync();

            foreach (var foto in fotos)
            {
                foto.Descarregada = true;
                foto.DataDownload = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();

            // Apresentar página informativa (o download em ZIP não está implementado)
            return Page();
        }
    }
}

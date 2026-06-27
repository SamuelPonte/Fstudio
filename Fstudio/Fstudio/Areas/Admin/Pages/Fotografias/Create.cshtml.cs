// ============================================================================
// Admin/Fotografias/Create.cshtml.cs
// Criação de fotografias com suporte a upload múltiplo simultâneo
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Areas.Admin.Pages.Fotografias;

/// <summary>
/// Modelo da página de criação de fotografias na área de administração.
/// Suporta upload de uma ou múltiplas imagens em simultâneo.
/// Quando são enviadas várias imagens, o título base é numerado automaticamente
/// (ex: "Casamento Silva 1", "Casamento Silva 2", ...).
/// A categoria, data de sessão e demais configurações aplicam-se a todas as fotos.
/// Acesso restrito à role "Admin".
/// </summary>
[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    // ── Serviços ──────────────────────────────────────────────────────────
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Construtor — injeta o contexto da BD e o ambiente web.
    /// </summary>
    public CreateModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context     = context;
        _environment = environment;
    }

    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Dados comuns a todas as fotografias submetidas:
    /// título base, descrição, categoria, data de sessão, destaque e visibilidade.
    /// </summary>
    [BindProperty]
    public Fotografia Fotografia { get; set; } = new() { VisivelPortfolio = true };

    /// <summary>
    /// Lista de categorias ativas para o dropdown de seleção.
    /// </summary>
    public SelectList Categorias { get; set; } = null!;

    // ── Validação ─────────────────────────────────────────────────────────

    /// <summary>Extensões de ficheiro permitidas no upload.</summary>
    private static readonly string[] _extensoesPermitidas = [".jpg", ".jpeg", ".png", ".webp"];

    /// <summary>Content-types permitidos no upload.</summary>
    private static readonly string[] _tiposPermitidos = ["image/jpeg", "image/png", "image/webp"];

    /// <summary>Tamanho máximo por ficheiro: 10 MB.</summary>
    private const long _tamanhoMaximo = 10 * 1024 * 1024;

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — apresenta o formulário de upload de fotografias.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        Categorias = new SelectList(
            await _context.Categorias.Where(c => c.Activa).OrderBy(c => c.Nome).ToListAsync(),
            "Id", "Nome");
        return Page();
    }

    /// <summary>
    /// POST — processa o upload de uma ou várias fotografias em simultâneo.
    ///
    /// Comportamento:
    /// - 1 ficheiro → título exatamente como preenchido
    /// - N ficheiros → título numerado: "Título Base 1", "Título Base 2", etc.
    ///
    /// Cada ficheiro é validado individualmente (tipo, tamanho).
    /// Se algum falhar a validação, nenhuma fotografia é guardada.
    /// </summary>
    /// <param name="ImagemFiles">Lista de ficheiros de imagem enviados pelo formulário</param>
    public async Task<IActionResult> OnPostAsync(IList<IFormFile>? ImagemFiles)
    {
        // Recarregar categorias para o caso de voltar à página com erros
        Categorias = new SelectList(
            await _context.Categorias.Where(c => c.Activa).OrderBy(c => c.Nome).ToListAsync(),
            "Id", "Nome");

        // Verificar que foi enviado pelo menos um ficheiro
        if (ImagemFiles == null || ImagemFiles.Count == 0)
        {
            ModelState.AddModelError("ImagemFiles", "É necessário selecionar pelo menos uma imagem.");
            return Page();
        }

        // Validar todos os ficheiros antes de guardar qualquer um
        // (falha rápida — evita uploads parciais)
        for (int i = 0; i < ImagemFiles.Count; i++)
        {
            var ficheiro = ImagemFiles[i];
            var prefixo  = ImagemFiles.Count > 1 ? $"Ficheiro {i + 1}: " : "";

            var extensao = Path.GetExtension(ficheiro.FileName).ToLowerInvariant();

            if (!_extensoesPermitidas.Contains(extensao))
            {
                ModelState.AddModelError("ImagemFiles",
                    $"{prefixo}Formato não suportado ({extensao}). Use JPG, PNG ou WebP.");
                return Page();
            }

            if (!_tiposPermitidos.Contains(ficheiro.ContentType.ToLowerInvariant()))
            {
                ModelState.AddModelError("ImagemFiles",
                    $"{prefixo}Tipo de ficheiro inválido. Use JPG, PNG ou WebP.");
                return Page();
            }

            if (ficheiro.Length > _tamanhoMaximo)
            {
                ModelState.AddModelError("ImagemFiles",
                    $"{prefixo}Tamanho ({ficheiro.Length / 1024 / 1024:F1} MB) excede o limite de 10 MB.");
                return Page();
            }
        }

        // Remover validação de ImagemUrl — é preenchida pelo upload, não pelo utilizador
        ModelState.Remove("Fotografia.ImagemUrl");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Garantir que a pasta de destino existe
        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "fotografias");
        Directory.CreateDirectory(uploadsPath);

        // Processar e guardar cada ficheiro
        var fotografias = new List<Fotografia>();
        bool multiplas  = ImagemFiles.Count > 1;

        for (int i = 0; i < ImagemFiles.Count; i++)
        {
            var ficheiro = ImagemFiles[i];
            var extensao = Path.GetExtension(ficheiro.FileName).ToLowerInvariant();

            // Guardar ficheiro com nome único (GUID) para evitar colisões
            var nomeUnico = $"{Guid.NewGuid()}{extensao}";
            var caminhoFisico = Path.Combine(uploadsPath, nomeUnico);

            await using (var stream = new FileStream(caminhoFisico, FileMode.Create))
            {
                await ficheiro.CopyToAsync(stream);
            }

            var urlImagem = $"/uploads/fotografias/{nomeUnico}";

            // Título: numerado automaticamente se forem múltiplas fotos
            var titulo = multiplas
                ? $"{Fotografia.Titulo} {i + 1}"
                : Fotografia.Titulo;

            fotografias.Add(new Fotografia
            {
                Titulo           = titulo,
                Descricao        = Fotografia.Descricao,
                CategoriaId      = Fotografia.CategoriaId,
                DataSessao       = Fotografia.DataSessao,
                Destaque         = Fotografia.Destaque,
                VisivelPortfolio = Fotografia.VisivelPortfolio,
                ImagemUrl        = urlImagem,
                ThumbnailUrl     = urlImagem,
                DataCriacao      = DateTime.UtcNow
            });
        }

        // Guardar todas as fotografias numa única operação de base de dados
        _context.Fotografias.AddRange(fotografias);
        await _context.SaveChangesAsync();

        // Mensagem de confirmação via TempData
        TempData["Sucesso"] = fotografias.Count == 1
            ? "Fotografia guardada com sucesso."
            : $"{fotografias.Count} fotografias guardadas com sucesso.";

        return RedirectToPage("Index");
    }
}

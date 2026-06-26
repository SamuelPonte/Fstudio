// ============================================================================
// FotografiasController.cs
// Controller da API REST para gestão de fotografias
// Endpoints disponíveis em: /api/fotografias
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Fstudio.Data.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Controllers.Api;

/// <summary>
/// Controller responsável pelos endpoints da API REST para fotografias.
/// As fotografias são o conteúdo principal da aplicação e podem ser associadas
/// a categorias e a clientes através da relação muitos-para-muitos ClienteFotografia.
///
/// Endpoints públicos (sem autenticação):
/// - GET /api/fotografias          → listar fotografias (com filtros opcionais)
/// - GET /api/fotografias/{id}     → obter fotografia por ID
///
/// Endpoints protegidos (requerem role "Admin"):
/// - POST   /api/fotografias       → criar nova fotografia
/// - PUT    /api/fotografias/{id}  → atualizar fotografia existente
/// - DELETE /api/fotografias/{id}  → eliminar fotografia
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class FotografiasController : ControllerBase
{
    // ── Serviços injetados via Dependency Injection ───────────────────────
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor — recebe o contexto da base de dados via injeção de dependências.
    /// </summary>
    /// <param name="context">Contexto EF Core para acesso à base de dados</param>
    public FotografiasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ── Endpoints de Leitura (públicos) ──────────────────────────────────

    /// <summary>
    /// GET /api/fotografias
    /// Retorna a lista de fotografias com suporte a múltiplos filtros opcionais.
    /// Ordenadas da mais recente para a mais antiga.
    ///
    /// Exemplos de uso:
    /// - GET /api/fotografias                        → todas as fotografias
    /// - GET /api/fotografias?categoriaId=1          → só da categoria 1
    /// - GET /api/fotografias?destaque=true          → só as fotografias em destaque
    /// - GET /api/fotografias?visivel=true&destaque=true → destaques visíveis no portfólio
    /// </summary>
    /// <param name="categoriaId">Filtrar por categoria (opcional)</param>
    /// <param name="destaque">Filtrar por destaque: true/false (opcional)</param>
    /// <param name="visivel">Filtrar por visibilidade no portfólio (opcional)</param>
    /// <returns>Lista de fotografias em formato JSON</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FotografiaDto>>> GetFotografias(
        [FromQuery] int? categoriaId,
        [FromQuery] bool? destaque,
        [FromQuery] bool? visivel)
    {
        // Construir query base com a categoria incluída (para devolver o nome)
        var query = _context.Fotografias
            .Include(f => f.Categoria)
            .AsQueryable();

        // Aplicar filtros opcionais — só adiciona a condição se o parâmetro foi fornecido
        if (categoriaId.HasValue)
            query = query.Where(f => f.CategoriaId == categoriaId.Value);

        if (destaque.HasValue)
            query = query.Where(f => f.Destaque == destaque.Value);

        if (visivel.HasValue)
            query = query.Where(f => f.VisivelPortfolio == visivel.Value);

        // Executar query, ordenar e projetar para DTO
        var fotografias = await query
            .OrderByDescending(f => f.DataCriacao)
            .Select(f => new FotografiaDto
            {
                Id               = f.Id,
                Titulo           = f.Titulo,
                Descricao        = f.Descricao,
                DataSessao       = f.DataSessao,
                ImagemUrl        = f.ImagemUrl,
                ThumbnailUrl     = f.ThumbnailUrl,
                Destaque         = f.Destaque,
                VisivelPortfolio = f.VisivelPortfolio,
                CategoriaId      = f.CategoriaId,
                CategoriaNome    = f.Categoria != null ? f.Categoria.Nome : null,
                DataCriacao      = f.DataCriacao
            })
            .ToListAsync();

        return Ok(fotografias);
    }

    /// <summary>
    /// GET /api/fotografias/{id}
    /// Retorna uma fotografia específica pelo seu ID numérico.
    /// Inclui o nome da categoria associada.
    /// </summary>
    /// <param name="id">ID da fotografia</param>
    /// <returns>Fotografia em formato JSON, ou 404 se não existir</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<FotografiaDto>> GetFotografia(int id)
    {
        // Carregar fotografia com a sua categoria (eager loading)
        var fotografia = await _context.Fotografias
            .Include(f => f.Categoria)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fotografia == null)
        {
            return NotFound(new { mensagem = $"Fotografia com ID {id} não encontrada." });
        }

        return Ok(new FotografiaDto
        {
            Id               = fotografia.Id,
            Titulo           = fotografia.Titulo,
            Descricao        = fotografia.Descricao,
            DataSessao       = fotografia.DataSessao,
            ImagemUrl        = fotografia.ImagemUrl,
            ThumbnailUrl     = fotografia.ThumbnailUrl,
            Destaque         = fotografia.Destaque,
            VisivelPortfolio = fotografia.VisivelPortfolio,
            CategoriaId      = fotografia.CategoriaId,
            CategoriaNome    = fotografia.Categoria?.Nome,
            DataCriacao      = fotografia.DataCriacao
        });
    }

    // ── Endpoints de Escrita (protegidos — apenas Admin) ─────────────────

    /// <summary>
    /// POST /api/fotografias
    /// Cria uma nova fotografia na base de dados.
    /// Requer autenticação com role "Admin".
    /// A categoria referenciada deve existir na base de dados.
    /// Nota: este endpoint recebe URLs de imagem. O upload de ficheiros
    /// é feito através da interface web (área de administração).
    /// </summary>
    /// <param name="dto">Dados da nova fotografia</param>
    /// <returns>201 Created com a fotografia criada</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<FotografiaDto>> CreateFotografia(FotografiaCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Verificar que a categoria existe antes de criar a fotografia
        var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
        if (categoria == null)
        {
            return BadRequest(new { mensagem = $"Categoria com ID {dto.CategoriaId} não encontrada." });
        }

        // Criar a entidade fotografia
        var fotografia = new Fotografia
        {
            Titulo           = dto.Titulo,
            Descricao        = dto.Descricao,
            DataSessao       = dto.DataSessao,
            ImagemUrl        = dto.ImagemUrl,
            ThumbnailUrl     = dto.ThumbnailUrl ?? dto.ImagemUrl,  // Se não houver thumbnail, usar a imagem principal
            Destaque         = dto.Destaque,
            VisivelPortfolio = dto.VisivelPortfolio,
            CategoriaId      = dto.CategoriaId,
            DataCriacao      = DateTime.UtcNow
        };

        _context.Fotografias.Add(fotografia);
        await _context.SaveChangesAsync();

        // Retornar 201 Created com o URL do novo recurso no header Location
        return CreatedAtAction(nameof(GetFotografia), new { id = fotografia.Id }, new FotografiaDto
        {
            Id               = fotografia.Id,
            Titulo           = fotografia.Titulo,
            Descricao        = fotografia.Descricao,
            DataSessao       = fotografia.DataSessao,
            ImagemUrl        = fotografia.ImagemUrl,
            ThumbnailUrl     = fotografia.ThumbnailUrl,
            Destaque         = fotografia.Destaque,
            VisivelPortfolio = fotografia.VisivelPortfolio,
            CategoriaId      = fotografia.CategoriaId,
            CategoriaNome    = categoria.Nome,
            DataCriacao      = fotografia.DataCriacao
        });
    }

    /// <summary>
    /// PUT /api/fotografias/{id}
    /// Atualiza uma fotografia existente.
    /// Requer autenticação com role "Admin".
    /// Os campos ImagemUrl e ThumbnailUrl só são atualizados se forem fornecidos.
    /// </summary>
    /// <param name="id">ID da fotografia a atualizar</param>
    /// <param name="dto">Novos dados da fotografia</param>
    /// <returns>204 No Content se bem sucedido</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFotografia(int id, FotografiaUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Verificar se a fotografia existe
        var fotografia = await _context.Fotografias.FindAsync(id);
        if (fotografia == null)
        {
            return NotFound(new { mensagem = $"Fotografia com ID {id} não encontrada." });
        }

        // Verificar se a categoria existe
        var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
        if (categoria == null)
        {
            return BadRequest(new { mensagem = $"Categoria com ID {dto.CategoriaId} não encontrada." });
        }

        // Atualizar campos — as URLs de imagem só são atualizadas se fornecidas
        // (permite atualizar metadados sem alterar a imagem)
        fotografia.Titulo           = dto.Titulo;
        fotografia.Descricao        = dto.Descricao;
        fotografia.DataSessao       = dto.DataSessao;
        fotografia.Destaque         = dto.Destaque;
        fotografia.VisivelPortfolio = dto.VisivelPortfolio;
        fotografia.CategoriaId      = dto.CategoriaId;

        if (!string.IsNullOrEmpty(dto.ImagemUrl))
            fotografia.ImagemUrl = dto.ImagemUrl;

        if (!string.IsNullOrEmpty(dto.ThumbnailUrl))
            fotografia.ThumbnailUrl = dto.ThumbnailUrl;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// DELETE /api/fotografias/{id}
    /// Elimina uma fotografia da base de dados.
    /// Requer autenticação com role "Admin".
    /// Nota: o ficheiro de imagem no servidor não é eliminado automaticamente —
    /// apenas o registo na base de dados é removido.
    /// </summary>
    /// <param name="id">ID da fotografia a eliminar</param>
    /// <returns>204 No Content se bem sucedido, 404 se não encontrada</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFotografia(int id)
    {
        var fotografia = await _context.Fotografias.FindAsync(id);
        if (fotografia == null)
        {
            return NotFound(new { mensagem = $"Fotografia com ID {id} não encontrada." });
        }

        // Remove o registo da base de dados
        // Nota: as relações ClienteFotografia associadas são eliminadas em cascata
        // conforme configurado no ApplicationDbContext
        _context.Fotografias.Remove(fotografia);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

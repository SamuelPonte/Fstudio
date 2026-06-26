// ============================================================================
// CategoriasController.cs
// Controller da API REST para gestão de categorias de fotografia
// Endpoints disponíveis em: /api/categorias
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Models;
using Fstudio.Data.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Controllers.Api;

/// <summary>
/// Controller responsável pelos endpoints da API REST para categorias.
/// As categorias organizam o portfólio de fotografias (ex: Casamentos, Noivados).
///
/// Endpoints públicos (sem autenticação):
/// - GET /api/categorias          → listar todas as categorias
/// - GET /api/categorias/{id}     → obter categoria por ID
/// - GET /api/categorias/slug/{slug} → obter categoria por slug URL
///
/// Endpoints protegidos (requerem role "Admin"):
/// - POST   /api/categorias       → criar nova categoria
/// - PUT    /api/categorias/{id}  → atualizar categoria existente
/// - DELETE /api/categorias/{id}  → eliminar categoria
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    // ── Serviços injetados via Dependency Injection ───────────────────────
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor — recebe o contexto da base de dados via injeção de dependências.
    /// </summary>
    /// <param name="context">Contexto EF Core para acesso à base de dados</param>
    public CategoriasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ── Endpoints de Leitura (públicos) ──────────────────────────────────

    /// <summary>
    /// GET /api/categorias
    /// Retorna a lista de todas as categorias, opcionalmente filtrada por estado.
    /// Inclui o total de fotografias em cada categoria.
    /// </summary>
    /// <param name="activa">Filtro opcional: true = só ativas, false = só inativas</param>
    /// <returns>Lista de categorias em formato JSON</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias([FromQuery] bool? activa)
    {
        // Construir a query base com include das fotografias (para contar)
        var query = _context.Categorias.Include(c => c.Fotografias).AsQueryable();

        // Aplicar filtro de estado se fornecido
        if (activa.HasValue)
        {
            query = query.Where(c => c.Activa == activa.Value);
        }

        // Executar query e projetar para DTO (evita expor dados internos do modelo)
        var categorias = await query
            .OrderBy(c => c.OrdemExibicao)  // Ordenar pela ordem definida pelo admin
            .Select(c => new CategoriaDto
            {
                Id             = c.Id,
                Nome           = c.Nome,
                Slug           = c.Slug,
                Descricao      = c.Descricao,
                OrdemExibicao  = c.OrdemExibicao,
                Activa         = c.Activa,
                TotalFotografias = c.Fotografias.Count  // Contagem eficiente via EF
            })
            .ToListAsync();

        return Ok(categorias);
    }

    /// <summary>
    /// GET /api/categorias/{id}
    /// Retorna uma categoria específica pelo seu ID numérico.
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <returns>Categoria em formato JSON, ou 404 se não existir</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
    {
        // Carregar categoria com fotografias (para contar o total)
        var categoria = await _context.Categorias
            .Include(c => c.Fotografias)
            .FirstOrDefaultAsync(c => c.Id == id);

        // Retornar 404 se não encontrada
        if (categoria == null)
        {
            return NotFound(new { mensagem = $"Categoria com ID {id} não encontrada." });
        }

        // Projetar para DTO e retornar
        return Ok(new CategoriaDto
        {
            Id               = categoria.Id,
            Nome             = categoria.Nome,
            Slug             = categoria.Slug,
            Descricao        = categoria.Descricao,
            OrdemExibicao    = categoria.OrdemExibicao,
            Activa           = categoria.Activa,
            TotalFotografias = categoria.Fotografias.Count
        });
    }

    /// <summary>
    /// GET /api/categorias/slug/{slug}
    /// Retorna uma categoria pelo seu slug (URL amigável).
    /// Útil para páginas de portfólio filtradas por categoria.
    /// Exemplo: GET /api/categorias/slug/casamentos
    /// </summary>
    /// <param name="slug">Slug da categoria (ex: "casamentos", "noivados")</param>
    /// <returns>Categoria em formato JSON, ou 404 se não existir</returns>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<CategoriaDto>> GetCategoriaBySlug(string slug)
    {
        // Pesquisar por slug (case-sensitive conforme está na BD)
        var categoria = await _context.Categorias
            .Include(c => c.Fotografias)
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (categoria == null)
        {
            return NotFound(new { mensagem = $"Categoria com slug '{slug}' não encontrada." });
        }

        return Ok(new CategoriaDto
        {
            Id               = categoria.Id,
            Nome             = categoria.Nome,
            Slug             = categoria.Slug,
            Descricao        = categoria.Descricao,
            OrdemExibicao    = categoria.OrdemExibicao,
            Activa           = categoria.Activa,
            TotalFotografias = categoria.Fotografias.Count
        });
    }

    // ── Endpoints de Escrita (protegidos — apenas Admin) ─────────────────

    /// <summary>
    /// POST /api/categorias
    /// Cria uma nova categoria de fotografia.
    /// Requer autenticação com role "Admin".
    /// O slug deve ser único na base de dados.
    /// </summary>
    /// <param name="dto">Dados da nova categoria (nome, slug, descrição, etc.)</param>
    /// <returns>201 Created com a categoria criada, ou 400 se os dados forem inválidos</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoriaDto>> CreateCategoria(CategoriaCreateDto dto)
    {
        // Validar o modelo (anotações de validação do DTO)
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Verificar unicidade do slug — dois slugs iguais causariam URLs duplicados
        if (await _context.Categorias.AnyAsync(c => c.Slug == dto.Slug))
        {
            return BadRequest(new { mensagem = "Já existe uma categoria com este slug." });
        }

        // Criar entidade e guardar na base de dados
        var categoria = new Categoria
        {
            Nome          = dto.Nome,
            Slug          = dto.Slug,
            Descricao     = dto.Descricao,
            OrdemExibicao = dto.OrdemExibicao,
            Activa        = dto.Activa,
            DataCriacao   = DateTime.UtcNow
        };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        // Retornar 201 Created com o URL do novo recurso no header Location
        return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, new CategoriaDto
        {
            Id               = categoria.Id,
            Nome             = categoria.Nome,
            Slug             = categoria.Slug,
            Descricao        = categoria.Descricao,
            OrdemExibicao    = categoria.OrdemExibicao,
            Activa           = categoria.Activa,
            TotalFotografias = 0  // Nova categoria começa sem fotografias
        });
    }

    /// <summary>
    /// PUT /api/categorias/{id}
    /// Atualiza uma categoria existente.
    /// Requer autenticação com role "Admin".
    /// </summary>
    /// <param name="id">ID da categoria a atualizar</param>
    /// <param name="dto">Novos dados da categoria</param>
    /// <returns>204 No Content se bem sucedido, 404 se não encontrada, 400 se inválido</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCategoria(int id, CategoriaUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Verificar se a categoria existe
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
        {
            return NotFound(new { mensagem = $"Categoria com ID {id} não encontrada." });
        }

        // Verificar unicidade do slug (excluindo a própria categoria da verificação)
        if (await _context.Categorias.AnyAsync(c => c.Slug == dto.Slug && c.Id != id))
        {
            return BadRequest(new { mensagem = "Já existe outra categoria com este slug." });
        }

        // Atualizar os campos da categoria
        categoria.Nome          = dto.Nome;
        categoria.Slug          = dto.Slug;
        categoria.Descricao     = dto.Descricao;
        categoria.OrdemExibicao = dto.OrdemExibicao;
        categoria.Activa        = dto.Activa;

        await _context.SaveChangesAsync();

        // 204 No Content — padrão REST para atualização bem sucedida sem retorno de dados
        return NoContent();
    }

    /// <summary>
    /// DELETE /api/categorias/{id}
    /// Elimina uma categoria da base de dados.
    /// Requer autenticação com role "Admin".
    /// Não é possível eliminar categorias que tenham fotografias associadas.
    /// </summary>
    /// <param name="id">ID da categoria a eliminar</param>
    /// <returns>204 No Content se bem sucedido, 404 se não encontrada, 400 se tiver fotografias</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategoria(int id)
    {
        // Carregar a categoria com as suas fotografias para verificar dependências
        var categoria = await _context.Categorias
            .Include(c => c.Fotografias)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria == null)
        {
            return NotFound(new { mensagem = $"Categoria com ID {id} não encontrada." });
        }

        // Regra de negócio: não permitir eliminação se existirem fotografias associadas
        // Isto evita fotografias órfãs (sem categoria) na base de dados
        if (categoria.Fotografias.Count > 0)
        {
            return BadRequest(new { mensagem = $"Não é possível eliminar a categoria '{categoria.Nome}' porque tem {categoria.Fotografias.Count} fotografia(s) associada(s). Mova ou elimine as fotografias primeiro." });
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

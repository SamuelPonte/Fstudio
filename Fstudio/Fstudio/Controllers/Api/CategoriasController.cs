using Fstudio.Data;
using Fstudio.Models.Entities;
using Fstudio.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/categorias
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias([FromQuery] bool? activa)
    {
        var query = _context.Categorias.Include(c => c.Fotografias).AsQueryable();

        if (activa.HasValue)
        {
            query = query.Where(c => c.Activa == activa.Value);
        }

        var categorias = await query
            .OrderBy(c => c.OrdemExibicao)
            .Select(c => new CategoriaDto
            {
                Id = c.Id,
                Nome = c.Nome,
                Slug = c.Slug,
                Descricao = c.Descricao,
                OrdemExibicao = c.OrdemExibicao,
                Activa = c.Activa,
                TotalFotografias = c.Fotografias.Count
            })
            .ToListAsync();

        return Ok(categorias);
    }

    // GET: api/categorias/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
    {
        var categoria = await _context.Categorias
            .Include(c => c.Fotografias)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria == null)
        {
            return NotFound();
        }

        return Ok(new CategoriaDto
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            Slug = categoria.Slug,
            Descricao = categoria.Descricao,
            OrdemExibicao = categoria.OrdemExibicao,
            Activa = categoria.Activa,
            TotalFotografias = categoria.Fotografias.Count
        });
    }

    // GET: api/categorias/slug/casamentos
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<CategoriaDto>> GetCategoriaBySlug(string slug)
    {
        var categoria = await _context.Categorias
            .Include(c => c.Fotografias)
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (categoria == null)
        {
            return NotFound();
        }

        return Ok(new CategoriaDto
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            Slug = categoria.Slug,
            Descricao = categoria.Descricao,
            OrdemExibicao = categoria.OrdemExibicao,
            Activa = categoria.Activa,
            TotalFotografias = categoria.Fotografias.Count
        });
    }

    // POST: api/categorias
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoriaDto>> CreateCategoria(CategoriaCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await _context.Categorias.AnyAsync(c => c.Slug == dto.Slug))
        {
            return BadRequest("Já existe uma categoria com este slug");
        }

        var categoria = new Categoria
        {
            Nome = dto.Nome,
            Slug = dto.Slug,
            Descricao = dto.Descricao,
            OrdemExibicao = dto.OrdemExibicao,
            Activa = dto.Activa,
            DataCriacao = DateTime.UtcNow
        };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, new CategoriaDto
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            Slug = categoria.Slug,
            Descricao = categoria.Descricao,
            OrdemExibicao = categoria.OrdemExibicao,
            Activa = categoria.Activa,
            TotalFotografias = 0
        });
    }

    // PUT: api/categorias/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCategoria(int id, CategoriaUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
        {
            return NotFound();
        }

        if (await _context.Categorias.AnyAsync(c => c.Slug == dto.Slug && c.Id != id))
        {
            return BadRequest("Já existe outra categoria com este slug");
        }

        categoria.Nome = dto.Nome;
        categoria.Slug = dto.Slug;
        categoria.Descricao = dto.Descricao;
        categoria.OrdemExibicao = dto.OrdemExibicao;
        categoria.Activa = dto.Activa;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/categorias/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategoria(int id)
    {
        var categoria = await _context.Categorias
            .Include(c => c.Fotografias)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria == null)
        {
            return NotFound();
        }

        if (categoria.Fotografias.Count > 0)
        {
            return BadRequest("Não é possível eliminar uma categoria com fotografias associadas");
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

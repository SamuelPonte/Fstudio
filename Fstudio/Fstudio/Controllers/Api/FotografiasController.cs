using Fstudio.Data;
using Fstudio.Data.Models;
using Fstudio.Data.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class FotografiasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FotografiasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/fotografias
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FotografiaDto>>> GetFotografias(
        [FromQuery] int? categoriaId,
        [FromQuery] bool? destaque,
        [FromQuery] bool? visivel)
    {
        var query = _context.Fotografias
            .Include(f => f.Categoria)
            .AsQueryable();

        if (categoriaId.HasValue)
        {
            query = query.Where(f => f.CategoriaId == categoriaId.Value);
        }

        if (destaque.HasValue)
        {
            query = query.Where(f => f.Destaque == destaque.Value);
        }

        if (visivel.HasValue)
        {
            query = query.Where(f => f.VisivelPortfolio == visivel.Value);
        }

        var fotografias = await query
            .OrderByDescending(f => f.DataCriacao)
            .Select(f => new FotografiaDto
            {
                Id = f.Id,
                Titulo = f.Titulo,
                Descricao = f.Descricao,
                DataSessao = f.DataSessao,
                ImagemUrl = f.ImagemUrl,
                ThumbnailUrl = f.ThumbnailUrl,
                Destaque = f.Destaque,
                VisivelPortfolio = f.VisivelPortfolio,
                CategoriaId = f.CategoriaId,
                CategoriaNome = f.Categoria != null ? f.Categoria.Nome : null,
                DataCriacao = f.DataCriacao
            })
            .ToListAsync();

        return Ok(fotografias);
    }

    // GET: api/fotografias/5
    [HttpGet("{id}")]
    public async Task<ActionResult<FotografiaDto>> GetFotografia(int id)
    {
        var fotografia = await _context.Fotografias
            .Include(f => f.Categoria)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fotografia == null)
        {
            return NotFound();
        }

        return Ok(new FotografiaDto
        {
            Id = fotografia.Id,
            Titulo = fotografia.Titulo,
            Descricao = fotografia.Descricao,
            DataSessao = fotografia.DataSessao,
            ImagemUrl = fotografia.ImagemUrl,
            ThumbnailUrl = fotografia.ThumbnailUrl,
            Destaque = fotografia.Destaque,
            VisivelPortfolio = fotografia.VisivelPortfolio,
            CategoriaId = fotografia.CategoriaId,
            CategoriaNome = fotografia.Categoria?.Nome,
            DataCriacao = fotografia.DataCriacao
        });
    }

    // POST: api/fotografias
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<FotografiaDto>> CreateFotografia(FotografiaCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
        if (categoria == null)
        {
            return BadRequest("Categoria não encontrada");
        }

        var fotografia = new Fotografia
        {
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            DataSessao = dto.DataSessao,
            ImagemUrl = dto.ImagemUrl,
            ThumbnailUrl = dto.ThumbnailUrl ?? dto.ImagemUrl,
            Destaque = dto.Destaque,
            VisivelPortfolio = dto.VisivelPortfolio,
            CategoriaId = dto.CategoriaId,
            DataCriacao = DateTime.UtcNow
        };

        _context.Fotografias.Add(fotografia);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFotografia), new { id = fotografia.Id }, new FotografiaDto
        {
            Id = fotografia.Id,
            Titulo = fotografia.Titulo,
            Descricao = fotografia.Descricao,
            DataSessao = fotografia.DataSessao,
            ImagemUrl = fotografia.ImagemUrl,
            ThumbnailUrl = fotografia.ThumbnailUrl,
            Destaque = fotografia.Destaque,
            VisivelPortfolio = fotografia.VisivelPortfolio,
            CategoriaId = fotografia.CategoriaId,
            CategoriaNome = categoria.Nome,
            DataCriacao = fotografia.DataCriacao
        });
    }

    // PUT: api/fotografias/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFotografia(int id, FotografiaUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var fotografia = await _context.Fotografias.FindAsync(id);
        if (fotografia == null)
        {
            return NotFound();
        }

        var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
        if (categoria == null)
        {
            return BadRequest("Categoria não encontrada");
        }

        fotografia.Titulo = dto.Titulo;
        fotografia.Descricao = dto.Descricao;
        fotografia.DataSessao = dto.DataSessao;
        if (!string.IsNullOrEmpty(dto.ImagemUrl))
        {
            fotografia.ImagemUrl = dto.ImagemUrl;
        }
        if (!string.IsNullOrEmpty(dto.ThumbnailUrl))
        {
            fotografia.ThumbnailUrl = dto.ThumbnailUrl;
        }
        fotografia.Destaque = dto.Destaque;
        fotografia.VisivelPortfolio = dto.VisivelPortfolio;
        fotografia.CategoriaId = dto.CategoriaId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/fotografias/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFotografia(int id)
    {
        var fotografia = await _context.Fotografias.FindAsync(id);
        if (fotografia == null)
        {
            return NotFound();
        }

        _context.Fotografias.Remove(fotografia);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

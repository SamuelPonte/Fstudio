using Fstudio.Data;
using Fstudio.Hubs;
using Fstudio.Models.Entities;
using Fstudio.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class ContactosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificacaoHub> _hubContext;

    public ContactosController(ApplicationDbContext context, IHubContext<NotificacaoHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    // GET: api/contactos
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ContactoDto>>> GetContactos(
        [FromQuery] bool? lido,
        [FromQuery] bool? arquivado)
    {
        var query = _context.Contactos.AsQueryable();

        if (lido.HasValue)
        {
            query = query.Where(c => c.Lido == lido.Value);
        }

        if (arquivado.HasValue)
        {
            query = query.Where(c => c.Arquivado == arquivado.Value);
        }

        var contactos = await query
            .OrderByDescending(c => c.DataEnvio)
            .Select(c => new ContactoDto
            {
                Id = c.Id,
                Nome = c.Nome,
                Email = c.Email,
                Telefone = c.Telefone,
                DataEvento = c.DataEvento,
                TipoServico = c.TipoServico,
                Mensagem = c.Mensagem,
                Lido = c.Lido,
                Arquivado = c.Arquivado,
                DataEnvio = c.DataEnvio
            })
            .ToListAsync();

        return Ok(contactos);
    }

    // GET: api/contactos/5
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ContactoDto>> GetContacto(int id)
    {
        var contacto = await _context.Contactos.FindAsync(id);

        if (contacto == null)
        {
            return NotFound();
        }

        // Mark as read
        if (!contacto.Lido)
        {
            contacto.Lido = true;
            contacto.DataLeitura = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return Ok(new ContactoDto
        {
            Id = contacto.Id,
            Nome = contacto.Nome,
            Email = contacto.Email,
            Telefone = contacto.Telefone,
            DataEvento = contacto.DataEvento,
            TipoServico = contacto.TipoServico,
            Mensagem = contacto.Mensagem,
            Lido = contacto.Lido,
            Arquivado = contacto.Arquivado,
            DataEnvio = contacto.DataEnvio
        });
    }

    // POST: api/contactos
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ContactoDto>> CreateContacto(ContactoCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var contacto = new Contacto
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            DataEvento = dto.DataEvento,
            TipoServico = dto.TipoServico,
            Mensagem = dto.Mensagem,
            DataEnvio = DateTime.UtcNow
        };

        _context.Contactos.Add(contacto);
        await _context.SaveChangesAsync();

        // Send SignalR notification
        await _hubContext.Clients.All.SendAsync("NovoContacto", contacto.Nome, contacto.Email, contacto.Mensagem);

        return CreatedAtAction(nameof(GetContacto), new { id = contacto.Id }, new ContactoDto
        {
            Id = contacto.Id,
            Nome = contacto.Nome,
            Email = contacto.Email,
            Telefone = contacto.Telefone,
            DataEvento = contacto.DataEvento,
            TipoServico = contacto.TipoServico,
            Mensagem = contacto.Mensagem,
            Lido = contacto.Lido,
            Arquivado = contacto.Arquivado,
            DataEnvio = contacto.DataEnvio
        });
    }

    // PUT: api/contactos/5/marcar-lido
    [HttpPut("{id}/marcar-lido")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarcarLido(int id)
    {
        var contacto = await _context.Contactos.FindAsync(id);
        if (contacto == null)
        {
            return NotFound();
        }

        contacto.Lido = true;
        contacto.DataLeitura = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/contactos/5/arquivar
    [HttpPut("{id}/arquivar")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Arquivar(int id)
    {
        var contacto = await _context.Contactos.FindAsync(id);
        if (contacto == null)
        {
            return NotFound();
        }

        contacto.Arquivado = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/contactos/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteContacto(int id)
    {
        var contacto = await _context.Contactos.FindAsync(id);
        if (contacto == null)
        {
            return NotFound();
        }

        _context.Contactos.Remove(contacto);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/contactos/nao-lidos/count
    [HttpGet("nao-lidos/count")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> GetNaoLidosCount()
    {
        var count = await _context.Contactos.CountAsync(c => !c.Lido);
        return Ok(count);
    }
}

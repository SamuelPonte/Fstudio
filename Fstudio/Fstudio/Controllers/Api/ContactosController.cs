// ============================================================================
// ContactosController.cs
// Controller da API REST para gestão de mensagens de contacto
// Endpoints disponíveis em: /api/contactos
// ============================================================================

using Fstudio.Data;
using Fstudio.Hubs;
using Fstudio.Data.Models;
using Fstudio.Data.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Controllers.Api;

/// <summary>
/// Controller responsável pelos endpoints da API REST para contactos.
/// Os contactos são mensagens enviadas por visitantes através do formulário
/// de contacto do site público.
///
/// Este controller integra com o SignalR (NotificacaoHub) para enviar
/// notificações em tempo real aos administradores quando há novos contactos.
///
/// Endpoints públicos:
/// - POST /api/contactos                    → submeter nova mensagem de contacto
///
/// Endpoints protegidos (requerem role "Admin"):
/// - GET  /api/contactos                    → listar todos os contactos
/// - GET  /api/contactos/{id}               → obter contacto por ID (marca como lido)
/// - PUT  /api/contactos/{id}/marcar-lido   → marcar contacto como lido
/// - PUT  /api/contactos/{id}/arquivar      → arquivar contacto
/// - GET  /api/contactos/nao-lidos/count    → contagem de mensagens não lidas
/// - DELETE /api/contactos/{id}             → eliminar contacto
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ContactosController : ControllerBase
{
    // ── Serviços injetados via Dependency Injection ───────────────────────
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificacaoHub> _hubContext;  // Para notificações SignalR

    /// <summary>
    /// Construtor — recebe os serviços necessários via injeção de dependências.
    /// </summary>
    /// <param name="context">Contexto EF Core para acesso à base de dados</param>
    /// <param name="hubContext">Contexto do Hub SignalR para enviar notificações em tempo real</param>
    public ContactosController(ApplicationDbContext context, IHubContext<NotificacaoHub> hubContext)
    {
        _context    = context;
        _hubContext = hubContext;
    }

    // ── Endpoints de Leitura (protegidos — apenas Admin) ─────────────────

    /// <summary>
    /// GET /api/contactos
    /// Retorna a lista de mensagens de contacto recebidas.
    /// Suporta filtros por estado de leitura e arquivo.
    /// Requer autenticação com role "Admin".
    ///
    /// Exemplos:
    /// - GET /api/contactos               → todos os contactos
    /// - GET /api/contactos?lido=false    → só os não lidos
    /// - GET /api/contactos?arquivado=false → só os não arquivados
    /// </summary>
    /// <param name="lido">Filtrar por estado de leitura (opcional)</param>
    /// <param name="arquivado">Filtrar por estado de arquivo (opcional)</param>
    /// <returns>Lista de contactos ordenados do mais recente para o mais antigo</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ContactoDto>>> GetContactos(
        [FromQuery] bool? lido,
        [FromQuery] bool? arquivado)
    {
        var query = _context.Contactos.AsQueryable();

        // Aplicar filtros opcionais
        if (lido.HasValue)
            query = query.Where(c => c.Lido == lido.Value);

        if (arquivado.HasValue)
            query = query.Where(c => c.Arquivado == arquivado.Value);

        // Ordenar do mais recente para o mais antigo e projetar para DTO
        var contactos = await query
            .OrderByDescending(c => c.DataEnvio)
            .Select(c => new ContactoDto
            {
                Id          = c.Id,
                Nome        = c.Nome,
                Email       = c.Email,
                Telefone    = c.Telefone,
                DataEvento  = c.DataEvento,
                TipoServico = c.TipoServico,
                Mensagem    = c.Mensagem,
                Lido        = c.Lido,
                Arquivado   = c.Arquivado,
                DataEnvio   = c.DataEnvio
            })
            .ToListAsync();

        return Ok(contactos);
    }

    /// <summary>
    /// GET /api/contactos/{id}
    /// Retorna um contacto específico pelo seu ID e marca-o automaticamente como lido.
    /// Requer autenticação com role "Admin".
    /// </summary>
    /// <param name="id">ID do contacto</param>
    /// <returns>Contacto em formato JSON, ou 404 se não existir</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ContactoDto>> GetContacto(int id)
    {
        var contacto = await _context.Contactos.FindAsync(id);

        if (contacto == null)
        {
            return NotFound(new { mensagem = $"Contacto com ID {id} não encontrado." });
        }

        // Marcar automaticamente como lido quando o admin acede ao detalhe
        if (!contacto.Lido)
        {
            contacto.Lido       = true;
            contacto.DataLeitura = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return Ok(new ContactoDto
        {
            Id          = contacto.Id,
            Nome        = contacto.Nome,
            Email       = contacto.Email,
            Telefone    = contacto.Telefone,
            DataEvento  = contacto.DataEvento,
            TipoServico = contacto.TipoServico,
            Mensagem    = contacto.Mensagem,
            Lido        = contacto.Lido,
            Arquivado   = contacto.Arquivado,
            DataEnvio   = contacto.DataEnvio
        });
    }

    // ── Endpoint Público ──────────────────────────────────────────────────

    /// <summary>
    /// POST /api/contactos
    /// Cria uma nova mensagem de contacto submetida por um visitante.
    /// Este é o único endpoint público deste controller (não requer autenticação).
    ///
    /// Após guardar na base de dados, envia automaticamente uma notificação
    /// SignalR para todos os administradores ligados ao painel.
    /// </summary>
    /// <param name="dto">Dados do formulário de contacto</param>
    /// <returns>201 Created com o contacto criado</returns>
    [HttpPost]
    [AllowAnonymous]  // Qualquer visitante pode submeter um contacto
    public async Task<ActionResult<ContactoDto>> CreateContacto(ContactoCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Criar entidade e definir data de envio com hora atual
        var contacto = new Contacto
        {
            Nome        = dto.Nome,
            Email       = dto.Email,
            Telefone    = dto.Telefone,
            DataEvento  = dto.DataEvento,
            TipoServico = dto.TipoServico,
            Mensagem    = dto.Mensagem,
            DataEnvio   = DateTime.UtcNow
        };

        _context.Contactos.Add(contacto);
        await _context.SaveChangesAsync();

        // Notificar administradores em tempo real via SignalR
        // O evento "NovoContacto" é escutado pelo JavaScript no _AdminLayout
        await _hubContext.Clients.All.SendAsync("NovoContacto",
            contacto.Nome,
            contacto.Email,
            contacto.Mensagem);

        return CreatedAtAction(nameof(GetContacto), new { id = contacto.Id }, new ContactoDto
        {
            Id          = contacto.Id,
            Nome        = contacto.Nome,
            Email       = contacto.Email,
            Telefone    = contacto.Telefone,
            DataEvento  = contacto.DataEvento,
            TipoServico = contacto.TipoServico,
            Mensagem    = contacto.Mensagem,
            Lido        = contacto.Lido,
            Arquivado   = contacto.Arquivado,
            DataEnvio   = contacto.DataEnvio
        });
    }

    // ── Endpoints de Gestão (protegidos — apenas Admin) ──────────────────

    /// <summary>
    /// PUT /api/contactos/{id}/marcar-lido
    /// Marca um contacto como lido e regista a data/hora de leitura.
    /// Requer autenticação com role "Admin".
    /// </summary>
    /// <param name="id">ID do contacto a marcar como lido</param>
    /// <returns>204 No Content se bem sucedido</returns>
    [HttpPut("{id}/marcar-lido")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarcarLido(int id)
    {
        var contacto = await _context.Contactos.FindAsync(id);
        if (contacto == null)
        {
            return NotFound(new { mensagem = $"Contacto com ID {id} não encontrado." });
        }

        // Atualizar estado e registar data/hora de leitura
        contacto.Lido        = true;
        contacto.DataLeitura = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// PUT /api/contactos/{id}/arquivar
    /// Arquiva um contacto para que não apareça na vista principal.
    /// Os contactos arquivados continuam na base de dados mas podem ser
    /// filtrados na listagem. Requer autenticação com role "Admin".
    /// </summary>
    /// <param name="id">ID do contacto a arquivar</param>
    /// <returns>204 No Content se bem sucedido</returns>
    [HttpPut("{id}/arquivar")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Arquivar(int id)
    {
        var contacto = await _context.Contactos.FindAsync(id);
        if (contacto == null)
        {
            return NotFound(new { mensagem = $"Contacto com ID {id} não encontrado." });
        }

        contacto.Arquivado = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// DELETE /api/contactos/{id}
    /// Elimina permanentemente um contacto da base de dados.
    /// Requer autenticação com role "Admin".
    /// </summary>
    /// <param name="id">ID do contacto a eliminar</param>
    /// <returns>204 No Content se bem sucedido</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteContacto(int id)
    {
        var contacto = await _context.Contactos.FindAsync(id);
        if (contacto == null)
        {
            return NotFound(new { mensagem = $"Contacto com ID {id} não encontrado." });
        }

        _context.Contactos.Remove(contacto);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// GET /api/contactos/nao-lidos/count
    /// Retorna o número de mensagens de contacto ainda não lidas.
    /// Usado pelo painel de administração para mostrar o badge de notificações.
    /// Requer autenticação com role "Admin".
    /// </summary>
    /// <returns>Número inteiro com a contagem de mensagens não lidas</returns>
    [HttpGet("nao-lidos/count")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> GetNaoLidosCount()
    {
        // Contar eficientemente sem carregar os registos completos
        var count = await _context.Contactos.CountAsync(c => !c.Lido);
        return Ok(count);
    }
}

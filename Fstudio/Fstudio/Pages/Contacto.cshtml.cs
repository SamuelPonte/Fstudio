// ============================================================================
// Pages/Contacto.cshtml.cs
// Formulário de contacto público do Fstudio
// ============================================================================

using Fstudio.Data;
using Fstudio.Hubs;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace Fstudio.Pages;

/// <summary>
/// Modelo da página de contacto do site público.
/// Permite a qualquer visitante enviar uma mensagem ao estúdio.
///
/// Após submissão do formulário:
/// 1. A mensagem é guardada na base de dados (tabela Contactos)
/// 2. Uma notificação SignalR é enviada em tempo real a todos os admins ligados
/// 3. É apresentada uma mensagem de sucesso ao visitante
///
/// Não requer autenticação — qualquer visitante pode contactar o estúdio.
/// </summary>
public class ContactoModel : PageModel
{
    // ── Serviços ──────────────────────────────────────────────────────────
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificacaoHub> _hubContext; // Para notificações SignalR

    /// <summary>
    /// Construtor — injeta o contexto da BD e o contexto do hub SignalR.
    /// </summary>
    public ContactoModel(ApplicationDbContext context, IHubContext<NotificacaoHub> hubContext)
    {
        _context    = context;
        _hubContext = hubContext;
    }

    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Dados do formulário de contacto submetidos pelo visitante.
    /// Ligado ao formulário HTML via [BindProperty].
    /// </summary>
    [BindProperty]
    public Contacto Contacto { get; set; } = new();

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — apresenta o formulário de contacto vazio.
    /// </summary>
    public void OnGet()
    {
        // Formulário estático — não necessita de dados da base de dados
    }

    /// <summary>
    /// POST — valida e guarda a mensagem de contacto.
    /// Após guardar, envia notificação SignalR ao administrador e
    /// apresenta mensagem de sucesso ao visitante via TempData.
    /// Redireciona para a mesma página (PRG pattern — evita resubmissão ao atualizar).
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Definir a data de envio no servidor (não confiar no cliente)
        Contacto.DataEnvio = DateTime.UtcNow;

        _context.Contactos.Add(Contacto);
        await _context.SaveChangesAsync();

        // Notificar administradores em tempo real via SignalR
        // O evento "NovoContacto" é escutado pelo JavaScript no _AdminLayout
        await _hubContext.Clients.All.SendAsync(
            "NovoContacto",
            Contacto.Nome,
            Contacto.Email,
            Contacto.Mensagem);

        // Mensagem de sucesso apresentada após redirecionamento (PRG pattern)
        TempData["Sucesso"] = "Mensagem enviada com sucesso! Entraremos em contacto brevemente.";

        // Redirecionar para a mesma página para evitar resubmissão ao atualizar
        return RedirectToPage();
    }
}

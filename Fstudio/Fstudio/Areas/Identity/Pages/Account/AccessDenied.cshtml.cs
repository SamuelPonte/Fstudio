// ============================================================================
// Identity/Account/AccessDenied.cshtml.cs
// Página de acesso negado (403 Forbidden)
// ============================================================================

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fstudio.Areas.Identity.Pages.Account;

/// <summary>
/// Modelo da página de acesso negado.
/// Apresentada quando um utilizador autenticado tenta aceder a um recurso
/// para o qual não tem permissão (ex: um Cliente a tentar aceder à área Admin).
/// O ASP.NET Core Identity redireciona automaticamente para esta página
/// quando a autorização falha e o utilizador já está autenticado.
/// O caminho desta página é configurado em Program.cs:
/// options.AccessDeniedPath = "/Identity/Account/AccessDenied"
/// </summary>
public class AccessDeniedModel : PageModel
{
    /// <summary>
    /// GET — apresenta a página de acesso negado.
    /// Não necessita de lógica adicional — apenas apresenta a mensagem de erro.
    /// </summary>
    public void OnGet()
    {
        // Sem lógica adicional necessária — a view apresenta a mensagem de erro
    }
}

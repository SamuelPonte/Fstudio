// ============================================================================
// Pages/StatusCode.cshtml.cs
// Página de erros HTTP personalizada (404, 500, etc.)
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fstudio.Pages;

/// <summary>
/// Modelo da página de erro HTTP personalizada.
/// Apresenta mensagens de erro amigáveis para diferentes códigos HTTP.
/// Configurada em Program.cs para interceptar erros em produção:
/// app.UseStatusCodePagesWithReExecute("/StatusCode/{0}")
///
/// O {0} é substituído pelo código HTTP real (ex: 404, 500, 403).
/// A view apresenta mensagens diferentes consoante o código de erro.
/// </summary>
public class StatusCodeModel : PageModel
{
    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Código de erro HTTP recebido (ex: 404, 500, 403).
    /// Passado via rota: /StatusCode/404
    /// </summary>
    public int ErrorCode { get; set; }

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — recebe o código de erro e disponibiliza-o para a view.
    /// </summary>
    /// <param name="code">Código de estado HTTP (ex: 404)</param>
    public void OnGet(int code)
    {
        // Guardar o código para a view apresentar a mensagem adequada
        ErrorCode = code;
    }
}

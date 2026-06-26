// ============================================================================
// Pages/Error.cshtml.cs
// Página de erro genérica para exceções não tratadas
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Fstudio.Pages;

/// <summary>
/// Modelo da página de erro genérica da aplicação.
/// Apresentada em desenvolvimento quando ocorre uma exceção não tratada.
/// Em produção, os erros HTTP são tratados pela página StatusCode.cshtml
/// via app.UseStatusCodePagesWithReExecute("/StatusCode/{0}").
///
/// Configurações especiais:
/// - ResponseCache(Duration = 0): garante que a página de erro nunca é cacheada
/// - IgnoreAntiforgeryToken: permite acesso mesmo se o token CSRF falhar
/// </summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    // ── Propriedades ──────────────────────────────────────────────────────

    /// <summary>
    /// Identificador único do pedido HTTP que causou o erro.
    /// Útil para correlacionar o erro com os logs do servidor.
    /// Obtido do Activity atual ou do TraceIdentifier do HttpContext.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Indica se o RequestId deve ser apresentado na página.
    /// Só apresentado se não estiver vazio.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    // ── Handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET — captura o ID do pedido para apresentação na página de erro.
    /// Prefere o ID da Activity de diagnóstico (mais preciso),
    /// com fallback para o TraceIdentifier do HTTP context.
    /// </summary>
    public void OnGet()
    {
        // Activity.Current?.Id é mais preciso quando há distributed tracing ativo
        // HttpContext.TraceIdentifier é o fallback standard do ASP.NET Core
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}

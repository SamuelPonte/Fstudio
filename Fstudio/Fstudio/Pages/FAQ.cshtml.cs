// ============================================================================
// Pages/FAQ.cshtml.cs
// Página de Perguntas Frequentes (FAQ)
// ============================================================================

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fstudio.Pages;

/// <summary>
/// Modelo da página de Perguntas Frequentes (FAQ).
/// Página estática — o conteúdo está definido diretamente no ficheiro .cshtml
/// e não necessita de dados da base de dados.
/// Acessível a todos os visitantes sem autenticação.
/// </summary>
public class FAQModel : PageModel
{
    /// <summary>
    /// GET — apresenta a página de FAQ.
    /// Sem lógica adicional — conteúdo estático definido na view.
    /// </summary>
    public void OnGet()
    {
    }
}

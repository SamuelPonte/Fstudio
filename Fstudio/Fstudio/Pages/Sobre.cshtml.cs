// ============================================================================
// Pages/Sobre.cshtml.cs
// Página "Sobre" — informação sobre o estúdio Fstudio
// ============================================================================

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fstudio.Pages;

/// <summary>
/// Modelo da página "Sobre" do site público.
/// Apresenta informação sobre o estúdio: história, equipa, valores e abordagem.
/// Página estática — todo o conteúdo está definido no ficheiro .cshtml
/// e não necessita de dados da base de dados.
/// Acessível a todos os visitantes sem autenticação.
/// </summary>
public class SobreModel : PageModel
{
    /// <summary>
    /// GET — apresenta a página "Sobre".
    /// Sem lógica adicional — conteúdo estático definido na view.
    /// </summary>
    public void OnGet()
    {
    }
}

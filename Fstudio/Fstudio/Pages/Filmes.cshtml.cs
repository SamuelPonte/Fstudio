// ============================================================================
// Pages/Filmes.cshtml.cs
// Página de filmes e vídeos de casamentos do Fstudio
// ============================================================================

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fstudio.Pages;

/// <summary>
/// Modelo da página de filmes do site público.
/// Apresenta os vídeos de casamentos e sessões do estúdio.
/// Página estática — o conteúdo (vídeos incorporados) está definido na view .cshtml
/// e não necessita de dados da base de dados nesta versão.
/// Acessível a todos os visitantes sem autenticação.
/// </summary>
public class FilmesModel : PageModel
{
    /// <summary>
    /// GET — apresenta a página de filmes.
    /// Sem lógica adicional — conteúdo estático definido na view.
    /// </summary>
    public void OnGet()
    {
    }
}

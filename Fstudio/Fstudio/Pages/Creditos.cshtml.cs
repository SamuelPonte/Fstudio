// ============================================================================
// Pages/Creditos.cshtml.cs
// Página de Créditos — requisito académico do trabalho prático
// ============================================================================

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fstudio.Pages;

/// <summary>
/// Modelo da página de Créditos do site.
/// Página obrigatória para o trabalho académico — contém:
/// - Nome do curso, disciplina e ano letivo
/// - Número e nome dos autores do projeto
/// - Bibliotecas, frameworks e código de terceiros utilizados (com origem)
/// - Credenciais dos utilizadores criados inicialmente (login e password)
///
/// Acessível a partir da página inicial e do rodapé do site.
/// Acessível a todos os visitantes sem autenticação.
/// </summary>
public class CreditosModel : PageModel
{
    /// <summary>
    /// GET — apresenta a página de Créditos.
    /// Sem lógica adicional — conteúdo estático definido na view.
    /// </summary>
    public void OnGet()
    {
    }
}

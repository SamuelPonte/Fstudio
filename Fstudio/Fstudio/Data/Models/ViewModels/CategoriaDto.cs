using System.ComponentModel.DataAnnotations;

namespace Fstudio.Data.Models.ViewModels;

/// <summary>
/// DTO utilizado para devolver os dados de uma categoria através da API.
/// Contém apenas a informação necessária para apresentar ou consultar categorias,
/// evitando expor diretamente a entidade da base de dados.
/// </summary>
public class CategoriaDto
{
    /// <summary>
    /// Identificador único da categoria.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome da categoria apresentado ao utilizador.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Identificador textual usado em URLs amigáveis.
    /// Exemplo: casamentos, batizados ou sessoes-fotograficas.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Descrição opcional da categoria.
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Ordem em que a categoria deve aparecer nas listagens.
    /// Quanto menor for o valor, mais cedo poderá ser apresentada.
    /// </summary>
    public int OrdemExibicao { get; set; }

    /// <summary>
    /// Indica se a categoria está ativa e pode ser apresentada na aplicação.
    /// </summary>
    public bool Activa { get; set; }

    /// <summary>
    /// Número total de fotografias associadas à categoria.
    /// Este valor é usado apenas para apresentação e estatísticas.
    /// </summary>
    public int TotalFotografias { get; set; }
}

/// <summary>
/// DTO usado para receber os dados necessários para criar uma nova categoria.
/// Inclui validações para garantir que os dados enviados pela API são válidos
/// antes de serem guardados na base de dados.
/// </summary>
public class CategoriaCreateDto
{
    /// <summary>
    /// Nome da nova categoria.
    /// Campo obrigatório com limite máximo de 100 caracteres.
    /// </summary>
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Slug da categoria, usado para criar URLs amigáveis.
    /// Deve conter apenas letras minúsculas, números e hífens.
    /// </summary>
    [Required(ErrorMessage = "O slug é obrigatório")]
    [StringLength(100)]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "O slug deve conter apenas letras minúsculas, números e hífens")]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Descrição opcional da categoria.
    /// Pode ser usada para explicar o tipo de fotografias incluídas nesta categoria.
    /// </summary>
    [StringLength(500)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Ordem de apresentação da categoria nas listagens.
    /// </summary>
    public int OrdemExibicao { get; set; }

    /// <summary>
    /// Indica se a categoria deve ficar ativa após a sua criação.
    /// Por defeito, uma nova categoria é criada como ativa.
    /// </summary>
    public bool Activa { get; set; } = true;
}

/// <summary>
/// DTO usado para receber os dados necessários para atualizar uma categoria existente.
/// É semelhante ao DTO de criação, mas aplica-se a uma categoria que já existe na base de dados.
/// </summary>
public class CategoriaUpdateDto
{
    /// <summary>
    /// Novo nome da categoria.
    /// Campo obrigatório com limite máximo de 100 caracteres.
    /// </summary>
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Novo slug da categoria.
    /// Deve continuar a respeitar o formato usado nas URLs amigáveis.
    /// </summary>
    [Required(ErrorMessage = "O slug é obrigatório")]
    [StringLength(100)]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Nova descrição da categoria.
    /// Este campo é opcional.
    /// </summary>
    [StringLength(500)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Nova ordem de apresentação da categoria.
    /// </summary>
    public int OrdemExibicao { get; set; }

    /// <summary>
    /// Define se a categoria fica ativa ou inativa após a atualização.
    /// </summary>
    public bool Activa { get; set; }
}

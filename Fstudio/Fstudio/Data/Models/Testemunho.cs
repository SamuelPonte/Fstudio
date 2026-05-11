using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fstudio.Data.Models;

public class Testemunho
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O texto é obrigatório")]
    [StringLength(2000, ErrorMessage = "O testemunho não pode exceder 2000 caracteres")]
    [Display(Name = "Testemunho")]
    public string Texto { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "A avaliação deve estar entre 1 e 5")]
    [Display(Name = "Avaliação")]
    public int Avaliacao { get; set; } = 5;

    [Display(Name = "Aprovado")]
    public bool Aprovado { get; set; } = false;

    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    [Display(Name = "Data de Aprovação")]
    public DateTime? DataAprovacao { get; set; }

    // Relação N:1 com Cliente
    [Required(ErrorMessage = "O cliente é obrigatório")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [ForeignKey("ClienteId")]
    public Cliente? Cliente { get; set; }
}

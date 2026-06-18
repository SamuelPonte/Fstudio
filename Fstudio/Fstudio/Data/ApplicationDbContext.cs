using Fstudio.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Data;

/// <summary>
/// Contexto principal da base de dados da aplicação.
/// Esta classe herda de IdentityDbContext<ApplicationUser>, permitindo integrar
/// as tabelas da aplicação com o sistema de autenticação e autorização do ASP.NET Core Identity.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Construtor do contexto da base de dados.
    /// Recebe as opções de configuração do Entity Framework Core,
    /// como a string de ligação definida no ficheiro appsettings.json.
    /// </summary>
    /// <param name="options">Opções de configuração do contexto da base de dados.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /* ******************************************
     * Tabelas da aplicação
     * ****************************************** */

    /// <summary>
    /// Tabela responsável por armazenar as categorias das fotografias.
    /// </summary>
    public DbSet<Categoria> Categorias => Set<Categoria>();

    /// <summary>
    /// Tabela responsável por armazenar as fotografias registadas no sistema.
    /// </summary>
    public DbSet<Fotografia> Fotografias => Set<Fotografia>();

    /// <summary>
    /// Tabela responsável por armazenar os dados dos clientes.
    /// </summary>
    public DbSet<Cliente> Clientes => Set<Cliente>();

    /// <summary>
    /// Tabela intermédia responsável por representar a relação muitos-para-muitos
    /// entre clientes e fotografias.
    /// </summary>
    public DbSet<ClienteFotografia> ClienteFotografias => Set<ClienteFotografia>();

    /// <summary>
    /// Tabela responsável por armazenar os testemunhos deixados pelos clientes.
    /// </summary>
    public DbSet<Testemunho> Testemunhos => Set<Testemunho>();

    /// <summary>
    /// Tabela responsável por armazenar as mensagens enviadas através
    /// do formulário de contacto da aplicação.
    /// </summary>
    public DbSet<Contacto> Contactos => Set<Contacto>();

    /// <summary>
    /// Configura o modelo da base de dados através da Fluent API.
    /// Neste método são definidos relacionamentos, chaves estrangeiras,
    /// comportamentos de eliminação e índices.
    /// </summary>
    /// <param name="builder">Objeto usado para configurar o modelo da base de dados.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Mantém todas as configurações internas do ASP.NET Core Identity.
        // Esta chamada é obrigatória quando se herda de IdentityDbContext.
        base.OnModelCreating(builder);

        /*
         * Configuração das chaves primárias do ASP.NET Core Identity.
         * O SQL Server não permite usar nvarchar(max) como chave primária ou índice.
         * Por isso, os identificadores das tabelas Identity devem ter tamanho limitado.
         */
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.Id)
                .HasMaxLength(450);
        });

        builder.Entity<IdentityRole>(entity =>
        {
            entity.Property(e => e.Id)
                .HasMaxLength(450);
        });

        /* ******************************************
         * Categoria
         * ****************************************** */
        builder.Entity<Categoria>(entity =>
        {
            // Garante que o slug de cada categoria é único.
            // Isto permite usar URLs amigáveis sem duplicações.
            entity.HasIndex(e => e.Slug).IsUnique();

            // Índice para melhorar pesquisas ou ordenações pelo nome da categoria.
            entity.HasIndex(e => e.Nome);
        });


        /* ******************************************
         * Fotografia
         * Relação N:1 com Categoria
         * ****************************************** */
        builder.Entity<Fotografia>(entity =>
        {
            // Uma fotografia pertence a uma categoria.
            // Uma categoria pode conter várias fotografias.
            entity.HasOne(f => f.Categoria)
                .WithMany(c => c.Fotografias)
                .HasForeignKey(f => f.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
            // O DeleteBehavior.Restrict impede que uma categoria seja eliminada
            // enquanto existirem fotografias associadas a essa categoria.

            // Índice para filtrar rapidamente fotografias em destaque.
            entity.HasIndex(e => e.Destaque);

            // Índice para filtrar fotografias visíveis no portfólio público.
            entity.HasIndex(e => e.VisivelPortfolio);

            // Índice para melhorar a listagem de fotografias por categoria.
            entity.HasIndex(e => e.CategoriaId);
        });


        /* ******************************************
         * Cliente
         * Relação 1:1 com ApplicationUser
         * ****************************************** */
        builder.Entity<Cliente>(entity =>
        {
            // Um cliente pode estar associado a um utilizador autenticado.
            // Um utilizador autenticado pode ter um registo de cliente associado.
            entity.HasOne(c => c.User)
                .WithOne(u => u.Cliente)
                .HasForeignKey<Cliente>(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            // DeleteBehavior.SetNull significa que, se o utilizador for eliminado,
            // o cliente continua na base de dados, mas o campo UserId passa a null.
            // Por isso, UserId deve ser opcional no modelo Cliente.

            // Índice para melhorar pesquisas por email.
            entity.HasIndex(e => e.Email);

            // Índice para melhorar filtros por estado do cliente.
            entity.HasIndex(e => e.Estado);
        });


        /* ******************************************
         * ClienteFotografia
         * Tabela de junção M:N entre Cliente e Fotografia
         * ****************************************** */
        builder.Entity<ClienteFotografia>(entity =>
        {
            // Um cliente pode ter várias fotografias associadas.
            // Cada registo ClienteFotografia pertence obrigatoriamente a um cliente.
            entity.HasOne(cf => cf.Cliente)
                .WithMany(c => c.ClienteFotografias)
                .HasForeignKey(cf => cf.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Uma fotografia pode estar associada a vários clientes.
            // Cada registo ClienteFotografia pertence obrigatoriamente a uma fotografia.
            entity.HasOne(cf => cf.Fotografia)
                .WithMany(f => f.ClienteFotografias)
                .HasForeignKey(cf => cf.FotografiaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Chave primária composta pelos campos ClienteId e FotografiaId.
            entity.HasKey(e => new { e.ClienteId, e.FotografiaId });
        });


        /* ******************************************
         * Testemunho
         * Relação N:1 com Cliente
         * ****************************************** */
        builder.Entity<Testemunho>(entity =>
        {
            // Um cliente pode deixar vários testemunhos.
            // Cada testemunho pertence obrigatoriamente a um cliente.
            entity.HasOne(t => t.Cliente)
                .WithMany(c => c.Testemunhos)
                .HasForeignKey(t => t.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
            // Se um cliente for eliminado, os seus testemunhos também são eliminados.

            // Índice para filtrar rapidamente testemunhos aprovados ou pendentes.
            entity.HasIndex(e => e.Aprovado);
        });


        /* ******************************************
         * Contacto
         * ****************************************** */
        builder.Entity<Contacto>(entity =>
        {
            // Índice para filtrar mensagens lidas e não lidas.
            entity.HasIndex(e => e.Lido);

            // Índice para filtrar mensagens arquivadas e não arquivadas.
            entity.HasIndex(e => e.Arquivado);

            // Índice para ordenar ou pesquisar mensagens pela data de envio.
            entity.HasIndex(e => e.DataEnvio);
        });
    }
}

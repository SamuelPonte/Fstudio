using Fstudio.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Fotografia> Fotografias => Set<Fotografia>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<ClienteFotografia> ClienteFotografias => Set<ClienteFotografia>();
    public DbSet<Testemunho> Testemunhos => Set<Testemunho>();
    public DbSet<Contacto> Contactos => Set<Contacto>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Categoria
        builder.Entity<Categoria>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.Nome);
        });

        // Fotografia - Relação N:1 com Categoria
        builder.Entity<Fotografia>(entity =>
        {
            entity.HasOne(f => f.Categoria)
                .WithMany(c => c.Fotografias)
                .HasForeignKey(f => f.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Destaque);
            entity.HasIndex(e => e.VisivelPortfolio);
            entity.HasIndex(e => e.CategoriaId);
        });

        // Cliente - Relação 1:1 com ApplicationUser
        builder.Entity<Cliente>(entity =>
        {
            entity.HasOne(c => c.User)
                .WithOne(u => u.Cliente)
                .HasForeignKey<Cliente>(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.Estado);
        });

        // ClienteFotografia - Tabela de junção M:N
        builder.Entity<ClienteFotografia>(entity =>
        {
            entity.HasOne(cf => cf.Cliente)
                .WithMany(c => c.ClienteFotografias)
                .HasForeignKey(cf => cf.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cf => cf.Fotografia)
                .WithMany(f => f.ClienteFotografias)
                .HasForeignKey(cf => cf.FotografiaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ClienteId, e.FotografiaId }).IsUnique();
        });

        // Testemunho - Relação N:1 com Cliente
        builder.Entity<Testemunho>(entity =>
        {
            entity.HasOne(t => t.Cliente)
                .WithMany(c => c.Testemunhos)
                .HasForeignKey(t => t.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Aprovado);
        });

        // Contacto
        builder.Entity<Contacto>(entity =>
        {
            entity.HasIndex(e => e.Lido);
            entity.HasIndex(e => e.Arquivado);
            entity.HasIndex(e => e.DataEnvio);
        });
    }
}

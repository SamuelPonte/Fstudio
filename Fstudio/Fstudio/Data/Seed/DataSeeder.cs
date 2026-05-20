// ============================================================================
// DataSeeder.cs
// Classe responsável por popular a base de dados com dados iniciais (seed data)
// Executada automaticamente no arranque da aplicação
// ============================================================================

using Fstudio.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fstudio.Data.Seed;

/// <summary>
/// Classe estática que contém métodos para inicializar a base de dados
/// com dados essenciais para o funcionamento da aplicação.
/// Inclui: roles de utilizador, utilizadores de teste e categorias de fotografia.
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Método principal que orquestra todo o processo de seeding.
    /// Deve ser chamado no arranque da aplicação (Program.cs).
    /// </summary>
    /// <param name="serviceProvider">Fornecedor de serviços da aplicação (DI Container)</param>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        // Criar um scope para resolver os serviços necessários
        // O scope garante que os serviços são descartados corretamente após uso
        using var scope = serviceProvider.CreateScope();

        // Obter instâncias dos serviços necessários através de Dependency Injection
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Aplicar migrações pendentes à base de dados
        // Isto cria as tabelas se não existirem
        await context.Database.MigrateAsync();

        // Executar os métodos de seeding por ordem
        // A ordem é importante: primeiro roles, depois users (que precisam das roles)
        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager, context);
        await SeedCategoriasAsync(context);
    }

    /// <summary>
    /// Cria as roles (papéis/funções) de utilizador na base de dados.
    /// Roles disponíveis:
    /// - Admin: Acesso completo ao backoffice e gestão da aplicação
    /// - Cliente: Acesso à área privada para ver fotografias do seu evento
    /// </summary>
    /// <param name="roleManager">Gestor de roles do ASP.NET Core Identity</param>
    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        // Array com os nomes das roles a criar
        string[] roles = ["Admin", "Cliente"];

        // Iterar sobre cada role e criar se não existir
        foreach (var role in roles)
        {
            // Verificar se a role já existe para evitar duplicados
            if (!await roleManager.RoleExistsAsync(role))
            {
                // Criar nova role na base de dados
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    /// <summary>
    /// Cria utilizadores de teste/demonstração na base de dados.
    /// Utilizadores criados:
    /// - Admin: admin@fstudio.info / Admin123! (acesso ao backoffice)
    /// - Cliente Demo: cliente@demo.com / Cliente123! (acesso à área cliente)
    /// </summary>
    /// <param name="userManager">Gestor de utilizadores do ASP.NET Core Identity</param>
    /// <param name="context">Contexto da base de dados</param>
    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        // ========================================
        // CRIAÇÃO DO UTILIZADOR ADMINISTRADOR
        // ========================================
        var adminEmail = "admin@fstudio.info";

        // Verificar se o admin já existe (evitar duplicados)
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            // Criar objeto do utilizador admin com todos os dados necessários
            var admin = new ApplicationUser
            {
                UserName = adminEmail,           // Nome de utilizador (usado para login)
                Email = adminEmail,               // Email do utilizador
                NomeCompleto = "Administrador Fstudio",  // Nome completo para exibição
                EmailConfirmed = true,            // Marcar email como confirmado (bypass verificação)
                DataCriacao = DateTime.UtcNow     // Data de criação do registo
            };

            // Criar o utilizador com a password especificada
            // O UserManager trata do hashing da password automaticamente
            var result = await userManager.CreateAsync(admin, "Admin123!");

            if (result.Succeeded)
            {
                // Associar o utilizador à role "Admin"
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // ========================================
        // CRIAÇÃO DO UTILIZADOR CLIENTE DE DEMONSTRAÇÃO
        // ========================================
        var clienteEmail = "cliente@demo.com";

        // Verificar se o cliente demo já existe
        if (await userManager.FindByEmailAsync(clienteEmail) == null)
        {
            // Criar objeto do utilizador cliente
            var clienteUser = new ApplicationUser
            {
                UserName = clienteEmail,
                Email = clienteEmail,
                NomeCompleto = "Cliente Demonstração",
                EmailConfirmed = true,
                DataCriacao = DateTime.UtcNow
            };

            // Criar o utilizador na base de dados
            var result = await userManager.CreateAsync(clienteUser, "Cliente123!");

            if (result.Succeeded)
            {
                // Associar à role "Cliente"
                await userManager.AddToRoleAsync(clienteUser, "Cliente");

                // Criar também um registo na tabela Clientes
                // Este registo contém informações adicionais do cliente (evento, contacto, etc.)
                var cliente = new Cliente
                {
                    Nome = "Cliente Demonstração",
                    Email = clienteEmail,
                    Telefone = "+351 912 345 678",
                    DataEvento = DateTime.UtcNow.AddMonths(3),  // Evento daqui a 3 meses
                    TipoServico = "Casamento",
                    Estado = "Ativo",
                    UserId = clienteUser.Id,  // Associar ao ApplicationUser criado
                    DataCriacao = DateTime.UtcNow
                };

                // Adicionar cliente ao contexto e guardar na base de dados
                context.Clientes.Add(cliente);
                await context.SaveChangesAsync();
            }
        }
    }

    /// <summary>
    /// Cria as categorias de fotografia iniciais na base de dados.
    /// Estas categorias são usadas para organizar o portfólio de fotografias.
    /// </summary>
    /// <param name="context">Contexto da base de dados</param>
    private static async Task SeedCategoriasAsync(ApplicationDbContext context)
    {
        // Verificar se já existem categorias (evitar duplicados)
        // Se já existirem, não fazer nada
        if (await context.Categorias.AnyAsync()) return;

        // Lista de categorias a criar
        // Cada categoria tem: Nome, Slug (URL amigável), Descrição, Ordem e Estado
        var categorias = new List<Categoria>
        {
            new()
            {
                Nome = "Casamentos",
                Slug = "casamentos",                    // URL: /portfolio/casamentos
                Descricao = "Fotografias de momentos únicos em casamentos",
                OrdemExibicao = 1,                      // Primeira categoria a aparecer
                Activa = true                           // Visível no site
            },
            new()
            {
                Nome = "Pré-Casamento",
                Slug = "pre-casamento",
                Descricao = "Sessões fotográficas antes do grande dia",
                OrdemExibicao = 2,
                Activa = true
            },
            new()
            {
                Nome = "Noivados",
                Slug = "noivados",
                Descricao = "Fotografias de pedidos de casamento e celebrações de noivado",
                OrdemExibicao = 3,
                Activa = true
            },
            new()
            {
                Nome = "Trash the Dress",
                Slug = "trash-the-dress",
                Descricao = "Sessões fotográficas pós-casamento criativas",
                OrdemExibicao = 4,
                Activa = true
            },
            new()
            {
                Nome = "Detalhes",
                Slug = "detalhes",
                Descricao = "Decoração, convites, anéis e outros detalhes especiais",
                OrdemExibicao = 5,
                Activa = true
            }
        };

        // Adicionar todas as categorias ao contexto
        context.Categorias.AddRange(categorias);

        // Guardar alterações na base de dados
        await context.SaveChangesAsync();
    }
}

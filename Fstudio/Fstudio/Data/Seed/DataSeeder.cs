// ============================================================================
// DataSeeder.cs
// Classe responsável por popular a base de dados com dados iniciais (seed data)
// Executada automaticamente no arranque da aplicação
// ============================================================================

using Fstudio.Data.Models;
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
        await SeedFotografiasAsync(context);
        await SeedTestemunhosAsync(context);
        await SeedContactosAsync(context);
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
                    Estado = EstadoCliente.Ativo,
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

    /// <summary>
    /// Cria fotografias de demonstração associadas às categorias existentes.
    /// </summary>
    private static async Task SeedFotografiasAsync(ApplicationDbContext context)
    {
        if (await context.Fotografias.AnyAsync()) return;

        // Obter IDs das categorias criadas
        var categorias = await context.Categorias.ToListAsync();
        if (categorias.Count == 0) return;
    
        int idCasamentos    = categorias.FirstOrDefault(c => c.Slug == "casamentos")?.Id    ?? categorias[0].Id;
        int idPreCasamento  = categorias.FirstOrDefault(c => c.Slug == "pre-casamento")?.Id ?? categorias[0].Id;
        int idNoivados      = categorias.FirstOrDefault(c => c.Slug == "noivados")?.Id      ?? categorias[0].Id;
        int idDetalhes      = categorias.FirstOrDefault(c => c.Slug == "detalhes")?.Id      ?? categorias[0].Id;

        // URLs de imagens de demonstração (Unsplash — uso livre)
        // Imagens de demonstração guardadas localmente em wwwroot/uploads/fotografias/demo
        var fotografias = new List<Fotografia>
{
    new()
    {
        Titulo = "Ana & Miguel — Quinta de Monserrate",
        CategoriaId = idCasamentos,
        ImagemUrl = "/uploads/fotografias/demo/casamento-01.jpg",
        ThumbnailUrl = "/uploads/fotografias/demo/casamento-01.jpg",
        VisivelPortfolio = true,
        Destaque = true,
        DataSessao = new DateTime(2025, 6, 14),
        DataCriacao = DateTime.UtcNow
    },
    new()
    {
        Titulo = "Sofia & João — Palácio de Queluz",
        CategoriaId = idCasamentos,
        ImagemUrl = "/uploads/fotografias/demo/casamento-02.jpg",
        ThumbnailUrl = "/uploads/fotografias/demo/casamento-02.jpg",
        VisivelPortfolio = true,
        Destaque = true,
        DataSessao = new DateTime(2025, 9, 6),
        DataCriacao = DateTime.UtcNow
    },
    new()
    {
        Titulo = "Inês & Rui — Sessão Pré-Casamento",
        CategoriaId = idPreCasamento,
        ImagemUrl = "/uploads/fotografias/demo/pre-casamento-01.jpg",
        ThumbnailUrl = "/uploads/fotografias/demo/pre-casamento-01.jpg",
        VisivelPortfolio = true,
        Destaque = false,
        DataSessao = new DateTime(2025, 4, 20),
        DataCriacao = DateTime.UtcNow
    },
    new()
    {
        Titulo = "Mariana & Tiago — Noivado na Praia",
        CategoriaId = idNoivados,
        ImagemUrl = "/uploads/fotografias/demo/noivado-01.jpg",
        ThumbnailUrl = "/uploads/fotografias/demo/noivado-01.jpg",
        VisivelPortfolio = true,
        Destaque = false,
        DataSessao = new DateTime(2025, 2, 14),
        DataCriacao = DateTime.UtcNow
    },
    new()
    {
        Titulo = "Detalhes — Anéis e Convites",
        CategoriaId = idDetalhes,
        ImagemUrl = "/uploads/fotografias/demo/detalhes-01.jpg",
        ThumbnailUrl = "/uploads/fotografias/demo/detalhes-01.jpg",
        VisivelPortfolio = true,
        Destaque = false,
        DataSessao = new DateTime(2025, 6, 14),
        DataCriacao = DateTime.UtcNow
    },
    new()
    {
        Titulo = "Catarina & Paulo — Jardim do Palácio",
        CategoriaId = idCasamentos,
        ImagemUrl = "/uploads/fotografias/demo/casamento-03.jpg",
        ThumbnailUrl = "/uploads/fotografias/demo/casamento-03.jpg",
        VisivelPortfolio = true,
        Destaque = false,
        DataSessao = new DateTime(2025, 7, 19),
        DataCriacao = DateTime.UtcNow
    }
};

        context.Fotografias.AddRange(fotografias);
        await context.SaveChangesAsync();

        // Associar 2 fotografias ao cliente de demonstração
        var cliente = await context.Clientes.FirstOrDefaultAsync(c => c.Email == "cliente@demo.com");
        if (cliente != null)
        {
            var fotos = await context.Fotografias.Take(2).ToListAsync();
            foreach (var foto in fotos)
            {
                context.ClienteFotografias.Add(new ClienteFotografia
                {
                    ClienteId    = cliente.Id,
                    FotografiaId = foto.Id,
                    DataAdicao   = DateTime.UtcNow
                });
            }
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Cria testemunhos de demonstração aprovados para apresentar no site.
    /// </summary>
    private static async Task SeedTestemunhosAsync(ApplicationDbContext context)
    {
        if (await context.Testemunhos.AnyAsync()) return;

        var cliente = await context.Clientes.FirstOrDefaultAsync();
        if (cliente == null) return;

        var testemunhos = new List<Testemunho>
        {
            new() { Texto = "O Fstudio capturou cada momento do nosso casamento de forma absolutamente mágica. As fotografias ficaram simplesmente perfeitas e vão ficar na nossa memória para sempre.", Avaliacao = 5, Aprovado = true, DataAprovacao = DateTime.UtcNow, ClienteId = cliente.Id, DataCriacao = DateTime.UtcNow },
            new() { Texto = "Profissionalismo e talento incrível. Desde a sessão de pré-casamento até ao grande dia, a equipa do Fstudio esteve sempre presente e com uma energia contagiante.", Avaliacao = 5, Aprovado = true, DataAprovacao = DateTime.UtcNow, ClienteId = cliente.Id, DataCriacao = DateTime.UtcNow },
            new() { Texto = "As nossas fotografias de noivado superaram todas as expectativas. O resultado foi cinematográfico e natural ao mesmo tempo. Recomendamos de olhos fechados!", Avaliacao = 5, Aprovado = true, DataAprovacao = DateTime.UtcNow, ClienteId = cliente.Id, DataCriacao = DateTime.UtcNow },
        };

        context.Testemunhos.AddRange(testemunhos);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Cria contactos de demonstração para o painel de administração.
    /// </summary>
    private static async Task SeedContactosAsync(ApplicationDbContext context)
    {
        if (await context.Contactos.AnyAsync()) return;

        var contactos = new List<Contacto>
        {
            new() { Nome = "Beatriz Santos",   Email = "beatriz.santos@email.com",   Telefone = "+351 916 123 456", DataEvento = new DateTime(2026, 9, 12), TipoServico = "Casamento",        Mensagem = "Bom dia! Estamos a planear o nosso casamento para setembro de 2026 e ficámos encantados com o vosso trabalho. Podiam enviar-nos informação sobre os pacotes disponíveis?",  Lido = true,  DataEnvio = DateTime.UtcNow.AddDays(-5) },
            new() { Nome = "Ricardo Ferreira", Email = "ricardo.ferreira@email.com", Telefone = "+351 962 789 012", DataEvento = new DateTime(2026, 6, 20), TipoServico = "Pré-Casamento",    Mensagem = "Olá! Queríamos fazer uma sessão de pré-casamento em Lisboa no próximo mês. Têm disponibilidade? Somos fãs do vosso trabalho!",                                              Lido = false, DataEnvio = DateTime.UtcNow.AddDays(-2) },
            new() { Nome = "Marta Oliveira",   Email = "marta.oliveira@email.com",   Telefone = null,               DataEvento = null,                      TipoServico = "Trash the Dress",  Mensagem = "Casámos em maio e gostaríamos muito de fazer uma sessão Trash the Dress antes do verão acabar. É possível orçamentar?",                                                       Lido = false, DataEnvio = DateTime.UtcNow.AddDays(-1) },
        };

        context.Contactos.AddRange(contactos);
        await context.SaveChangesAsync();
    }
}

// ============================================================================
// Program.cs
// Ponto de entrada da aplicação ASP.NET Core
// Configura todos os serviços, middleware e pipeline de request/response
// ============================================================================

using Fstudio.Data;
using Fstudio.Data.Seed;
using Fstudio.Hubs;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

// ============================================================================
// CRIAÇÃO DO BUILDER
// O WebApplicationBuilder é usado para configurar serviços e definições
// ============================================================================
var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CONFIGURAÇÃO DA BASE DE DADOS
// ============================================================================

// Obter a connection string do ficheiro appsettings.json
// Se não existir, lança uma exceção (fail-fast pattern)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Registar o DbContext no container de Dependency Injection
// UseSqlite - utiliza SQLite como base de dados (ficheiro local)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Adicionar página de exceções para desenvolvimento (mostra erros de BD)
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ============================================================================
// CONFIGURAÇÃO DO IDENTITY (AUTENTICAÇÃO E AUTORIZAÇÃO)
// ============================================================================

// Configurar ASP.NET Core Identity com suporte para roles
// ApplicationUser - classe customizada que estende IdentityUser
// IdentityRole - classe padrão para gestão de roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configurações de Sign In
    options.SignIn.RequireConfirmedAccount = false;  // Não exigir confirmação de email
    options.SignIn.RequireConfirmedEmail = false;     // Não exigir email confirmado

    // Configurações de Password (requisitos de segurança)
    options.Password.RequireDigit = true;            // Exigir pelo menos 1 número
    options.Password.RequireLowercase = true;        // Exigir letra minúscula
    options.Password.RequireUppercase = true;        // Exigir letra maiúscula
    options.Password.RequireNonAlphanumeric = true;  // Exigir caractere especial (!@#$...)
    options.Password.RequiredLength = 8;             // Mínimo 8 caracteres
})
.AddEntityFrameworkStores<ApplicationDbContext>()   // Usar EF Core para armazenar dados
.AddDefaultTokenProviders()                          // Tokens para reset password, etc.
.AddDefaultUI();                                     // UI padrão do Identity (Login, Register, etc.)

// Configurar caminhos dos cookies de autenticação
// Define para onde redirecionar em cada situação
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";           // Página de login
    options.LogoutPath = "/Identity/Account/Logout";         // Página de logout
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";  // Acesso negado
});

// ============================================================================
// CONFIGURAÇÃO DE RAZOR PAGES E ÁREAS
// ============================================================================

// Adicionar suporte a Razor Pages com autorização por área
builder.Services.AddRazorPages(options =>
{
    // Proteger toda a área Admin - só acessível por utilizadores com role "Admin"
    options.Conventions.AuthorizeAreaFolder("Admin", "/", "Admin");

    // Proteger toda a área Cliente - só acessível por utilizadores com role "Cliente"
    options.Conventions.AuthorizeAreaFolder("Cliente", "/", "Cliente");
});

// ============================================================================
// CONFIGURAÇÃO DE API REST
// ============================================================================

// Adicionar suporte a Controllers (para endpoints API)
builder.Services.AddControllers();

// Adicionar suporte ao Swagger/OpenAPI para documentar e testar a API
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Fstudio API",
        Version = "v1",
        Description = "API para gestão de categorias, fotografias, contactos e dados da aplicação Fstudio."
    });
});

// ============================================================================
// CONFIGURAÇÃO DE SIGNALR (COMUNICAÇÃO EM TEMPO REAL)
// ============================================================================

// Adicionar SignalR para notificações em tempo real
// Usado para notificar admins quando há novos contactos
builder.Services.AddSignalR();

// ============================================================================
// POLÍTICAS DE AUTORIZAÇÃO
// ============================================================================

// Definir políticas nomeadas para autorização
// Podem ser usadas em controllers/pages com [Authorize(Policy = "Admin")]
builder.Services.AddAuthorization(options =>
{
    // Política "Admin" - requer role Admin
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));

    // Política "Cliente" - requer role Cliente
    options.AddPolicy("Cliente", policy => policy.RequireRole("Cliente"));
});

// ============================================================================
// CONSTRUÇÃO DA APLICAÇÃO
// ============================================================================
var app = builder.Build();

// ============================================================================
// SEED DA BASE DE DADOS
// ============================================================================

// Popular a base de dados com dados iniciais (roles, users, categorias)
// Executado uma vez no arranque da aplicação
await DataSeeder.SeedAsync(app.Services);

// ============================================================================
// CONFIGURAÇÃO DO PIPELINE HTTP
// ============================================================================

// Configurações específicas por ambiente (Development vs Production)
if (app.Environment.IsDevelopment())
{
    // Em desenvolvimento: mostrar página de erros de migrações
    app.UseMigrationsEndPoint();
}
else
{
    // Em produção: usar handler de erros genérico
    app.UseExceptionHandler("/Error");

    // Usar páginas de erro personalizadas (404, 500, etc.)
    app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");

    // Ativar HSTS (HTTP Strict Transport Security)
    app.UseHsts();
}

// Ativar Swagger para documentação e testes da API
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fstudio API v1");
    c.RoutePrefix = "swagger";
});

// Redirecionar HTTP para HTTPS
app.UseHttpsRedirection();

// Ativar routing (necessário para mapear endpoints)
app.UseRouting();

// Middleware de autenticação (identificar o utilizador)
// IMPORTANTE: Deve vir antes de UseAuthorization
app.UseAuthentication();

// Middleware de autorização (verificar permissões)
app.UseAuthorization();

// ============================================================================
// MAPEAMENTO DE ENDPOINTS
// ============================================================================

// Servir ficheiros estáticos (CSS, JS, imagens de wwwroot)
app.MapStaticAssets();

// Mapear Razor Pages
app.MapRazorPages()
   .WithStaticAssets();

// Mapear Controllers da API REST
// Endpoints disponíveis: /api/fotografias, /api/categorias, /api/contactos
app.MapControllers();

// Mapear Hub SignalR para notificações em tempo real
// Os clientes ligam-se a: /hubs/notificacao
app.MapHub<NotificacaoHub>("/hubs/notificacao");

// ============================================================================
// INICIAR A APLICAÇÃO
// ============================================================================

app.Run();
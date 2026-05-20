using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



/* ******************************************
 * Configuração da base de dados
 * ****************************************** */

// Obtém a string de ligação à base de dados a partir do ficheiro appsettings.json.
// Caso a string "DefaultConnection" não exista, é lançada uma exceção.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Regista o ApplicationDbContext na aplicação e configura o Entity Framework Core
// para usar SQL Server como sistema de gestão de base de dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Adiciona uma página de erro detalhada para problemas relacionados com a base de dados.
// Esta funcionalidade é útil durante o desenvolvimento.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();





/* ******************************************
 * Configuração do ASP.NET Core Identity
 * ****************************************** */

// Configura o sistema de autenticação da aplicação usando a classe personalizada ApplicationUser.
// Esta classe estende IdentityUser e permite guardar dados adicionais do utilizador,
// como o nome completo e a data de criação.
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Obriga os utilizadores a confirmarem a conta antes de conseguirem iniciar sessão.
    // Normalmente, esta confirmação é feita através de email.
    options.SignIn.RequireConfirmedAccount = true;
})
    // Adiciona suporte a roles/perfis de utilizador, como "Administrador" e "Cliente".
    .AddRoles<IdentityRole>()

    // Indica que o Identity deve guardar os seus dados usando o ApplicationDbContext.
    .AddEntityFrameworkStores<ApplicationDbContext>();





/* ******************************************
 * Configuração das Razor Pages
 * ****************************************** */

// Adiciona suporte a Razor Pages, permitindo criar páginas dinâmicas
// para a interface web da aplicação.
builder.Services.AddRazorPages();


/* ******************************************
 * Construção da aplicação
 * ****************************************** */

var app = builder.Build();





/* ******************************************
 * Configuração do pipeline HTTP
 * ****************************************** */

// Se a aplicação estiver em ambiente de desenvolvimento,
// ativa a página de migrations para facilitar a gestão da base de dados.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // Em ambiente de produção, redireciona erros para uma página genérica.
    app.UseExceptionHandler("/Error");

    // Ativa HSTS, aumentando a segurança ao forçar o uso de HTTPS.
    app.UseHsts();
}

// Redireciona automaticamente pedidos HTTP para HTTPS.
app.UseHttpsRedirection();

// Ativa o sistema de routing da aplicação.
app.UseRouting();

// Ativa o sistema de autorização.
// Deve ser usado para proteger páginas ou funcionalidades com [Authorize].
app.UseAuthorization();






/* ******************************************
 * Configuração dos ficheiros estáticos e rotas
 * ****************************************** */

// Permite servir ficheiros estáticos, como CSS, JavaScript e imagens.
app.MapStaticAssets();

// Mapeia as Razor Pages da aplicação e associa os ficheiros estáticos necessários.
app.MapRazorPages()
   .WithStaticAssets();






/* ******************************************
 * Execução da aplicação
 * ****************************************** */

// Inicia a aplicação web.
app.Run();
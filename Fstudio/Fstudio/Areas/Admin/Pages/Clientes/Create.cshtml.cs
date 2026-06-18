using Fstudio.Data;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClienteEntity = Fstudio.Data.Models.Cliente;

namespace Fstudio.Areas.Admin.Pages.Clientes;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public ClienteEntity Cliente { get; set; } = new() { Estado = EstadoCliente.Ativo };

    [BindProperty]
    public bool CriarUtilizador { get; set; }    


    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Cliente.DataCriacao = DateTime.UtcNow;

        if (CriarUtilizador)
        {
            var user = new ApplicationUser
            {
                UserName = Cliente.Email,
                Email = Cliente.Email,
                NomeCompleto = Cliente.Nome,
                EmailConfirmed = true,
                DataCriacao = DateTime.UtcNow
            };

            var password = GenerateRandomPassword();
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Cliente");
                Cliente.UserId = user.Id;

                TempData["PasswordGerada"] = password;
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }

        _context.Clientes.Add(Cliente);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private static string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
        var random = new Random();
        var password = new char[12];

        for (int i = 0; i < password.Length; i++)
        {
            password[i] = chars[random.Next(chars.Length)];
        }

        return new string(password) + "!1";
    }
}

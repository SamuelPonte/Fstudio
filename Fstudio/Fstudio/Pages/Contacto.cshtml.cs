using Fstudio.Data;
using Fstudio.Hubs;
using Fstudio.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace Fstudio.Pages;

public class ContactoModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificacaoHub> _hubContext;

    public ContactoModel(ApplicationDbContext context, IHubContext<NotificacaoHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [BindProperty]
    public Contacto Contacto { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Contacto.DataEnvio = DateTime.UtcNow;
        _context.Contactos.Add(Contacto);
        await _context.SaveChangesAsync();

        // Send SignalR notification to admin
        await _hubContext.Clients.All.SendAsync("NovoContacto", Contacto.Nome, Contacto.Email, Contacto.Mensagem);

        TempData["Sucesso"] = "Mensagem enviada com sucesso! Entraremos em contacto brevemente.";

        return RedirectToPage();
    }
}

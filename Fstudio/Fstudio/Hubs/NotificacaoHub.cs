using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Fstudio.Hubs;

[Authorize(Roles = "Admin")]
public class NotificacaoHub : Hub
{
    public async Task EnviarNotificacao(string titulo, string mensagem)
    {
        await Clients.All.SendAsync("ReceberNotificacao", titulo, mensagem);
    }

    public async Task NovoContacto(string nome, string email, string mensagem)
    {
        await Clients.All.SendAsync("NovoContacto", nome, email, mensagem);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}

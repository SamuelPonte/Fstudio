// ============================================================================
// NotificacaoHub.cs
// Hub SignalR para comunicação em tempo real entre o servidor e os clientes
// Utilizado para enviar notificações instantâneas aos administradores
// ============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Fstudio.Hubs;

/// <summary>
/// Hub SignalR responsável pelas notificações em tempo real da aplicação.
/// Permite que o servidor envie mensagens instantâneas para os clientes ligados
/// sem que estes precisem de fazer polling (pedidos repetidos ao servidor).
///
/// Acesso restrito à role "Admin" — apenas administradores recebem notificações.
///
/// Eventos disponíveis (escutados pelo cliente JavaScript):
/// - "ReceberNotificacao" → notificação genérica com título e mensagem
/// - "NovoContacto"       → alerta quando um visitante envia uma mensagem de contacto
/// </summary>
[Authorize(Roles = "Admin")]
public class NotificacaoHub : Hub
{
    /// <summary>
    /// Envia uma notificação genérica para todos os administradores ligados.
    /// Pode ser invocado a partir do cliente JavaScript ou de um Controller via IHubContext.
    /// </summary>
    /// <param name="titulo">Título da notificação (ex: "Novo Testemunho")</param>
    /// <param name="mensagem">Corpo da notificação com detalhes</param>
    public async Task EnviarNotificacao(string titulo, string mensagem)
    {
        // Envia o evento "ReceberNotificacao" para todos os clientes ligados ao hub
        await Clients.All.SendAsync("ReceberNotificacao", titulo, mensagem);
    }

    /// <summary>
    /// Notifica todos os administradores ligados de que um novo contacto foi recebido.
    /// Este método é chamado pelo ContactosController quando alguém submete o formulário.
    /// </summary>
    /// <param name="nome">Nome da pessoa que enviou o contacto</param>
    /// <param name="email">Email da pessoa que enviou o contacto</param>
    /// <param name="mensagem">Excerto da mensagem enviada</param>
    public async Task NovoContacto(string nome, string email, string mensagem)
    {
        // Envia o evento "NovoContacto" para todos os admins — o JavaScript
        // no _AdminLayout atualiza o badge de notificações e mostra um toast
        await Clients.All.SendAsync("NovoContacto", nome, email, mensagem);
    }

    /// <summary>
    /// Chamado automaticamente pelo SignalR quando um cliente estabelece ligação ao hub.
    /// Pode ser usado para registar a ligação ou enviar dados iniciais ao cliente.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        // Chama a implementação base (obrigatório para o SignalR funcionar corretamente)
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Chamado automaticamente pelo SignalR quando um cliente perde a ligação ao hub.
    /// Pode ser usado para limpar recursos ou registar a desconexão.
    /// </summary>
    /// <param name="exception">Exceção que causou a desconexão (null se foi intencional)</param>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Chama a implementação base para garantir limpeza correta dos recursos
        await base.OnDisconnectedAsync(exception);
    }
}

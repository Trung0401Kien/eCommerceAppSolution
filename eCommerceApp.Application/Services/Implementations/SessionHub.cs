using Microsoft.AspNetCore.SignalR;

namespace eCommerceApp.Application.Services.Implementations
{
    public class SessionHub : Hub
    {
        public async Task NotifyLogout(string userId)
        {
            await Clients.User(userId).SendAsync("ReceiveLogoutNotification", "Your session has been logged out.");
        }
    }
}

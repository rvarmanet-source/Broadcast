using Broadcast.Models;

namespace Broadcast.Services
{
    public interface IBroadcastMessageSender
    {
        Task SendMessageAsync(BroadcastMessage message);
        Task SendMessagesAsync(IEnumerable<BroadcastMessage> messages);
    }
}

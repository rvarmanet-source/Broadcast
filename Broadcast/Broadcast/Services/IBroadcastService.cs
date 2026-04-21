using Broadcast.Models;

namespace Broadcast.Services
{
    public interface IBroadcastService
    {
        Task<IEnumerable<BroadcastMessage>> GetAllMessagesAsync();
        Task<IEnumerable<BroadcastMessage>> GetActiveMessagesAsync();
        Task<BroadcastMessage?> GetMessageByIdAsync(int id);
        Task<BroadcastMessage> CreateMessageAsync(BroadcastMessageViewModel model);
        Task<bool> UpdateMessageAsync(int id, BroadcastMessageViewModel model);
        Task<bool> DeleteMessageAsync(int id);
        Task<bool> SendMessageAsync(int id);
        Task<int> GetActiveMessageCountAsync();
        Task IncrementViewCountAsync(int id);
    }
}
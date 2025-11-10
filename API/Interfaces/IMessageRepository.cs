using System;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message?> GetMessage(int id);
    Task<PaginatedResult<MessageDTO>> GetMessagesForUser();

    Task<IReadOnlyList<MessageDTO>> GetMessageThread(int currentMemberId, int recipientId); 
    public Task<bool> SaveAllAsync();
}

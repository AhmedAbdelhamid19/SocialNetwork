using System;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessageRepository
{
    /// <summary>
    /// add a new message to the database
    /// </summary>
    /// <param name="message"></param>
    void AddMessage(Message message);
    /// <summary>
    /// delete a message from the database
    /// </summary>
    /// <param name="message"></param>
    void DeleteMessage(Message message);
    /// <summary>
    /// Get a message by its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Message?> GetMessage(int id);
    /// <summary>
    /// Get paginated messages for a member based on the message parameters
    /// </summary>
    /// <param name="messageParams"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    Task<PaginatedResult<MessageDTO>> GetMessagesForMember(MessageParams messageParams, int memberId);

    /// <summary>
    /// Get the message thread between the current member and the recipient.
    /// Also updates the DateRead property for all unread messages.
    /// </summary>
    /// <param name="currentMemberId"></param>
    /// <param name="recipientId"></param>
    /// <returns></returns>
    Task<IReadOnlyList<MessageDTO>> GetMessageThread(int currentMemberId, int recipientId); 
    public Task<bool> SaveAllAsync();
}

using System;
using API.Data;
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
    /// <summary>
    /// Add a new group to the database
    /// </summary>
    /// <param name="group"></param>
    void AddGroup(Group group);
    /// <summary>
    /// Efficiently delete a connection when user closes tab / disconnects.
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    Task RemoveConnection(string connectionId);
    /// <summary>
    /// Finds a single Connection row by its primary key (ConnectionId).
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    Task<Connection?> GetConnection(string connectionId);
    /// <summary>
    /// Get a group by name AND include all its connections
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    Task<Group?> GetMessageGroup(string groupName);
    /// <summary>
    /// Given a connectionId, find which group that connection belongs to.
    /// it fetches the group and also all connections inside it.
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    Task<Group?> GetGroupForConnection(string connectionId);
}
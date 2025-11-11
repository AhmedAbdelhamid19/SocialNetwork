using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessageRepository(AppDbContext context) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<PaginatedResult<MessageDTO>> GetMessagesForMember(MessageParams messageParams, int memberId)
    {
        var query = context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

        query = messageParams.Container switch
        {
            // inbox means that the messages that the logged in user received
            "Inbox" => query.Where(m => m.RecipientId == memberId && !m.RecipientDeleted),
            // outbox means that the messages that the logged in user sent
            "Outbox" => query.Where(m => m.SenderId == memberId && !m.SenderDeleted),
            // default case is unread messages 
            _ => query.Where(m => m.RecipientId == memberId && !m.RecipientDeleted && m.DateRead == null)
        };

        query = query.OrderByDescending(m => m.MessageSent);

        return await PaginationHelper.CreateAsync(query.Select(MessageExtensions.ToMessageDTOExpression()), messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IReadOnlyList<MessageDTO>> GetMessageThread(int currentMemberId, int recipientId)
    {
        // get theres required elements and update DateRead 
        // ExcuteUpdateAsync is more efficient than fetching entities and updating in memory by ForEachAsync
        // everything is done in the database, EF change tracker is not involved and don't know anything about these entities
        await context.Messages
            .Where(m => m.RecipientId == currentMemberId && m.DateRead == null && m.SenderId == recipientId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.DateRead, p => DateTime.UtcNow));
        // .ForEachAsync(m => m.DateRead = DateTime.UtcNow);

        // fetch all messages between the two members
        return await context.Messages
            .Where(m => (m.RecipientId == currentMemberId && m.SenderId == recipientId && !m.RecipientDeleted) ||
                        (m.RecipientId == recipientId && m.SenderId == currentMemberId && !m.SenderDeleted))
            .OrderBy(m => m.MessageSent)
            .Select(MessageExtensions.ToMessageDTOExpression())
            .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
using System;
using System.Linq.Expressions;
using API.DTOs;
using API.Entities;

namespace API.Extensions;

public static class MessageExtensions
{
    public static MessageDTO ToMessageDTO(this Message message)
    {
        return new MessageDTO
        {
            Id = message.Id,
            Content = message.Content,
            DateRead = message.DateRead,
            MessageSent = message.MessageSent,
            SenderId = message.SenderId,
            SenderDisplayName = message.Sender.DisplayName,
            SenderImageUrl = message.Sender.ImageUrl,
            RecipientId = message.RecipientId,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientImageUrl = message.Recipient.ImageUrl
        };
    }
    public static Expression<Func<Message, MessageDTO>> ToMessageDTOExpression()
    {
        return message => new MessageDTO
        {
            Id = message.Id,
            Content = message.Content,
            DateRead = message.DateRead,
            MessageSent = message.MessageSent,
            SenderId = message.SenderId,
            SenderDisplayName = message.Sender.DisplayName,
            SenderImageUrl = message.Sender.ImageUrl,
            RecipientId = message.RecipientId,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientImageUrl = message.Recipient.ImageUrl
        };
    } 
}


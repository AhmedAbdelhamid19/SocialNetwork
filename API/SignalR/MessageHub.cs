using System;
using System.Security.Claims;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IMessageRepository messageRepository, IMemberRepository memberRepository): Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpcontext = Context.GetHttpContext();
        int curUser = GetCurrentUser(), otherUser = GetOtherUser();
        var groupName = GetGroupName(curUser, otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var messages = await messageRepository.GetMessageThread(curUser, otherUser);
        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
    public async Task SendMessage(SendMessageDTO sendMessageDTO)
    {
        int senderId = GetCurrentUser(), recipientId = GetOtherUser();
        var sender = await memberRepository.GetMemberByIdAsync(senderId);
        var recipient = await memberRepository.GetMemberByIdAsync(recipientId);
        if (sender == null || recipient == null)
            throw new HubException("can't find sender or reciever");

        var message = new Entities.Message {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = sendMessageDTO.Content
        };

        messageRepository.AddMessage(message);

        if (await messageRepository.SaveAllAsync())
        {
            var groupName = GetGroupName(senderId, recipientId);
            await Clients.Group(groupName)
                .SendAsync("NewMessage",message.ToMessageDTO());
        }
    }
    public static string GetGroupName(int? curUser, int? otherUser)
    {
        return curUser< otherUser ? $"{curUser}-{otherUser}" : $"{otherUser}-{curUser}";
    }
    public int GetCurrentUser()
    {
        var memberId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new Exception("sender id not found, try to authenticate again");
        int curUser = int.Parse(memberId);
        return curUser;
    }
    public int GetOtherUser()
    {
        var httpcontext = Context.GetHttpContext();
        var otherUser = httpcontext?.Request.Query["otherUserId"].ToString()
            ?? throw new HubException("other user not found");
        return int.Parse(otherUser);
    }
}
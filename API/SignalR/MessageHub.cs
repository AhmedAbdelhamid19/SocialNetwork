using System;
using System.Security.Claims;
using API.Data;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IMessageRepository messageRepository,
    IMemberRepository memberRepository, IHubContext<PresenceHub> presenceHub): Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpcontext = Context.GetHttpContext();
        int curUser = GetCurrentUser(), otherUser = GetOtherUser();
        var groupName = GetGroupName(curUser, otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToGroup(groupName);

        var messages = await messageRepository.GetMessageThread(curUser, otherUser);
        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine(Context.ConnectionId);
        await messageRepository.RemoveConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
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
        
        var groupName = GetGroupName(senderId, recipientId);
        var group = await messageRepository.GetMessageGroup(groupName);
        bool isUserInGroup = group != null && group.Connections.Any(c => c.UserId == recipientId);
        if(isUserInGroup) {
            message.DateRead = DateTime.UtcNow;
        }
        messageRepository.AddMessage(message);

        if (await messageRepository.SaveAllAsync())
        {
            await Clients.Group(groupName)
                .SendAsync("NewMessage",message.ToMessageDTO());
            var connections = await PresenceTracker.GetConnection($"{recipientId}");
            if(connections != null && connections.Count > 0 && !isUserInGroup) 
            {
                await presenceHub.Clients.Clients(connections)
                    .SendAsync("newMessageReceived", message.ToMessageDTO());
            }
        }
    }
    private async Task<bool> AddToGroup(string groupName)
    {
        var group = await messageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, GetCurrentUser());
        if (group == null)
        {
            group = new Group(groupName);
            messageRepository.AddGroup(group);
        }
        group.Connections.Add(connection);
        return await messageRepository.SaveAllAsync();
    }
    public static string GetGroupName(int? curUser, int? otherUser)
    {
        return curUser< otherUser ? $"{curUser}-{otherUser}" : $"{otherUser}-{curUser}";
    }
    public int GetCurrentUser()
    {
        // you can use it now because in program.cs you added the token to the context.
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
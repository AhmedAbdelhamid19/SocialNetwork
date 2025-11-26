using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class PresenceHub(PresenceTracker presenceTracker): Hub
{
    public override async Task OnConnectedAsync()
    {
        await presenceTracker.UserConnected(GetUserId(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserOnline", GetUserId());

        var currentUsers = await presenceTracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await presenceTracker.UserDisconnected(GetUserId(), Context.ConnectionId); 
        await Clients.Others.SendAsync("UserOffline", GetUserId());

        var currentUsers = await presenceTracker.GetOnlineUsers();
        await Clients.Others.SendAsync("GetOnlineUsers", currentUsers);
    }
    private string GetUserId()
    {
        return Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new HubException("Cann't get member id");
    }
}

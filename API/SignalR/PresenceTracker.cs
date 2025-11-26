using System;
using System.Collections.Concurrent;

namespace API.SignalR;

public class PresenceTracker
{
    /*
        <userId, <connectionId, byte>>, here byte is useless.
        {"user1" : { "mobileApp" : 0, "webApp" : 0 }} due to multiple connections from multiple devices.
        it's thread safe, because signalR run on multiple threads, no race conditions, no crashes or problems
        it holds all online users and it's devices in memory
    */
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> OnlineUsers = new();

    // used when user start to connect to the hub
    public Task UserConnected(string userId, string connectionId)
    {
        // get connection if exist or create new one, use lambda as value please (create object after check)
        var connections = OnlineUsers.GetOrAdd(userId, _ => new ConcurrentDictionary<string,byte>());
        connections.TryAdd(connectionId, 0);

        return Task.CompletedTask;
    }

    public Task UserDisconnected(string userId, string connectionId)
    {
        // remember: out here make use user connection outside
        if(OnlineUsers.TryGetValue(userId, out var connections))
        {
            connections.TryRemove(connectionId, out _);
            if(connections.IsEmpty)
            {
                OnlineUsers.TryRemove(userId, out _);
            }
        }
        return Task.CompletedTask;
    }
    public Task<string[]> GetOnlineUsers()
    {
        return Task.FromResult(OnlineUsers.Keys.OrderBy(k=>k).ToArray());
    }
}

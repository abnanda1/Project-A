using DoodleThings.Controllers;
using DoodleThings.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
public class DrawingBoard : Hub
{
    // Map of connection id to connection id
    private static readonly ConcurrentDictionary<string, string> _games = new ConcurrentDictionary<string, string>();
    private static readonly UserInfoController _userInfoController = new UserInfoController();

    public override Task OnConnected()
    {
        Clients.Caller.Drawer = true;
        //    // Get User id from session cookie
    //    Cookie userNameCookie;

    //    if (!Context.RequestCookies.TryGetValue("userName", out userNameCookie))
    //    {
    //        throw new InvalidOperationException("User Name missing");
    //    }

    //    // Update the new user's values
    //    Clients.Caller.userName = userNameCookie.Value;
    //    Clients.Caller.Drawer = true;
    //    // Update the Game table with who is the drawer and who is the guesser
    //    _userInfoController.UpdateConnectionId(userNameCookie.Value, Context.ConnectionId);

    //    UserInfo player2 = _userInfoController.GetRandomAvailablePlayer(userNameCookie.Value);
    //    Clients.Client(player2.UserInfoId).Drawer = false;

        //if (player2 != null)
    //    {
    //        // We are assuming that the second user here is the drawer
    //        _games.TryAdd(Context.ConnectionId, player2.UserInfoId);
    //    }

    //    // send a message to both the clients so that the UI can be updated

        return base.OnConnected();
    }

    public Task BroadcastPoint(float x, float y)
    {
        string connectionId;
   
        if (_games.TryGetValue(Context.ConnectionId, out connectionId))
        {
            return Clients.Client(connectionId).drawPoint(x, y, Clients.Caller.color);
        }
        else
        {
            return null;
        }
    }

    public void EndGame()
    {
        Clients.Caller.showAlert("Game Ended");
        string connectionId;
        // Need too add logic to remove the mapping when the guesser leaves
        _games.TryRemove(Context.ConnectionId, out connectionId);
        // Update both the user's status
    }

    public Task BroadcastClear()
    {
        return Clients.Others.clear();
    }

    public override Task OnDisconnected()
    {
        EndGame();
        return base.OnDisconnected();
    }
}

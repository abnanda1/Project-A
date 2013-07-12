using DoodleThings.Controllers;
using DoodleThings.Models;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class DrawingBoard : Hub
{
    // Map of connection id to connection id
    private static readonly ConcurrentDictionary<string, string> _games = new ConcurrentDictionary<string, string>();
    private static readonly UserInfoController _userInfoController = new UserInfoController();
    private static readonly GameController _gameController = new GameController();

    public override Task OnConnected()
    {
        string currentPlayerName = Context.User.Identity.Name;

        // Set or update the player's values
        Clients.Caller.userName = currentPlayerName;
        _userInfoController.UpdateConnectionId(currentPlayerName, Context.ConnectionId);

        Game game = _gameController.AssignRandomAvailableGame(currentPlayerName);

        // Set game specific states
        Clients.Caller.GameId = game.GameId;
        string otherPlayerConnId = "";
      
        if (game.State == GameState.InPlay)
        {
            if (Clients.Caller.userName == game.DrawerUser.UserName)
            {
                Clients.Caller.Drawer = true;
                otherPlayerConnId = game.GuesserUser.ConnectionId;
                Clients.Client(otherPlayerConnId).Drawer = false;
                _games.TryAdd(Context.ConnectionId, otherPlayerConnId);
            }
            else
            {
                Clients.Caller.Drawer = false;
                otherPlayerConnId = game.DrawerUser.ConnectionId;
                Clients.Client(otherPlayerConnId).Drawer = true;
                _games.TryAdd(otherPlayerConnId, Context.ConnectionId);
            }
            Clients.Client(otherPlayerConnId).GameId = game.GameId;

            Clients.Caller.startGame();
            Clients.Client(otherPlayerConnId).startGame();
        }
        else
        {
            Clients.Caller.waitForPlayer();
        }
              
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
        // Need too add logic to remove the mapping when the guesser leaves
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

    public void UpdateState()
    {
    }
}

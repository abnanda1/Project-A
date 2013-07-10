using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
public class DrawingBoard : Hub
{
    public Task BroadcastPoint(float x, float y)
    {
        return Clients.Others.drawPoint(x, y, Clients.Caller.color);
    }
    public Task BroadcastClear()
    {
        return Clients.Others.clear();
    }
}

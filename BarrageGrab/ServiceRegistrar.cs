using BarrageGrab.GrabServices;
using BarrageGrab.Websocket;

namespace BarrageGrab
{
    internal static class ServiceRegistrar
    {
        internal static void BuildServices()
        {
            ApplicationRuntime.LocalWebSocketServer = new LocalWebSocketServer();
            ApplicationRuntime.LocalWebSocketServer.Start();
            ApplicationRuntime.BarrageGrabService = new DouyinBarrageGrabService();
        }
    }
}

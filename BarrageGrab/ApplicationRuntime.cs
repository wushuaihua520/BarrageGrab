using BarrageGrab.Entity.Enums;
using BarrageGrab.GrabServices;
using BarrageGrab.Websocket;

namespace BarrageGrab
{
    internal static class ApplicationRuntime
    {
        internal static MainWindow? MainWindow { get; set; }

        internal static LocalWebSocketServer? LocalWebSocketServer { get; set; }

        internal static IBarrageGrabService? BarrageGrabService { get; set; }

        internal static PlatformTypeEnum LivePlatform { get; set; } = PlatformTypeEnum.Douyin;

        internal static void Shutdown()
        {
            BarrageGrabService?.Stop();
            if (BarrageGrabService is IDisposable disposable)
            {
                disposable.Dispose();
            }

            BarrageGrabService = null;
            LocalWebSocketServer?.Dispose();
            LocalWebSocketServer = null;
            MainWindow = null;
        }
    }
}

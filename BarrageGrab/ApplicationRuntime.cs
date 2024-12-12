using BarrageGrab.Entity.Enums;
using BarrageGrab.GrabServices;
using BarrageGrab.Websocket;
using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab
{
    internal static class ApplicationRuntime
    {
        public static MainWindow? MainWindow { get; set; }

        /// <summary>
        /// 主窗体实例
        /// </summary>
        private static MainWindow? _mainWindow;
        internal static MainWindow MainForm
        {
            get
            {
                if (_mainWindow == null)
                {
                    _mainWindow = new MainWindow();
                }

                return _mainWindow;
            }
        }

        /// <summary>
        /// 本机WebSocket服务实例
        /// </summary>
        internal static LocalWebSocketServer? LocalWebSocketServer { get; set; }

        /// <summary>
        /// 弹幕抓取服务实例
        /// </summary>
        internal static IBarrageGrabService? BarrageGrabService;

        /// <summary>
        /// 直播的平台
        /// </summary>
        internal static PlatformTypeEnum? LivePlatform;


    }
}

using BarrageGrab.GrabServices;
using BarrageGrab.Websocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab
{
    /// <summary>
    /// 服务注册器
    /// </summary>
    internal static class ServiceRegistrar
    {
        internal static void BuildServices()
        {
            //本机websocket服务
            ApplicationRuntime.LocalWebSocketServer = new LocalWebSocketServer();
            ApplicationRuntime.LocalWebSocketServer.Start();

            //抖音弹幕抓取服务
            ApplicationRuntime.BarrageGrabService = new DouyinBarrageGrabService();
            //ApplicationRuntime.BarrageGrabService.Run(""); //其他地方调用

        }
    }
}

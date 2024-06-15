using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab
{
    /// <summary>
    /// global configs
    /// </summary>
    internal static class GlobalConfigs
    {
        /// <summary>
        /// 本地WebSocket服务器发布地址
        /// </summary>
        internal static string LocalWebSocketServer_Location { get; } = "ws://0.0.0.0:8888";


        /// <summary>
        /// 抖音直播间Url
        /// </summary>
        internal static string LiveUrl_Douyin { get; } = "https://live.douyin.com";

    }
}

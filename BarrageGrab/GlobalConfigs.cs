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


        internal static string Version { get; } = "v1.9.0";




        #region 签名服务

        /// <summary>
        /// 2H2G 2M带宽服务器，2024-06-25后，大家打赏的金额，将用作于服务器。
        /// 如需独立部署版本，请联系群主。
        /// </summary>
        internal static string SignApi_Domain { get; } = "http://129.204.171.194:8081"; //2H2G 2M 临时服务器
        internal static string SignApi_Url { get; } = "/Douyin/SignWss";

        /// <summary>
        /// 测试key，每分钟、每个key、每个独立IP，请求有限制，为：每分钟/key/IP 共10 times，且
        /// 如需申请独立key，请联系群主。
        /// </summary>
        internal static string SignApi_Key { get; } = "test-apikey-de9991ea-bf2b-454c-7982-adddfe0581ac-96c0642e-7d1d-87a7-08b2-eff81edae4d3";

        #endregion

    }
}

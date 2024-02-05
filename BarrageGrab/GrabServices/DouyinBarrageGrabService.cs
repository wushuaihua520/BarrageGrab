using BarrageGrab.Entity.Enums;
using BarrageGrab.Framework;
using BarrageGrab.Protobuf;
using Google.Protobuf;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BarrageGrab.GrabServices
{
    /// <summary>
    /// 抖音弹幕抓取
    /// </summary>
    internal class DouyinBarrageGrabService : IBarrageGrabService, IDisposable
    {
        #region 属性&字段

        /// <summary>
        /// User-Agent
        /// </summary>
        private string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";


        /// <summary>
        /// 直播id
        /// 若直播间Url为：https://live.douyin.com/751990192217
        /// 那么 751990192217 既为 liveid
        /// </summary>
        private string LiveId = string.Empty;


        private ClientWebSocket? clientWebSocket;



        #region Ttwid
        private string? _ttwid = string.Empty;
        private string? Ttwid
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_ttwid))
                {
                    _ttwid = GetTtwid();
                }

                return _ttwid;
            }
            set
            {
                this._ttwid = value;
            }
        }
        #endregion


        #region RoomId
        private string? _roomid = string.Empty;
        private string? RoomId
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_roomid))
                {
                    _roomid = GetRoomId();
                }

                return _roomid;
            }
            set
            {
                this._roomid = value;
            }
        }
        #endregion


        #region Wss

        private string _wss = string.Empty;
        private string Wss
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_wss))
                {
                    _wss = GetWss();
                }

                return _wss;
            }
            set
            {
                this._wss = value;
            }
        }
        #endregion



        /// <summary>
        /// 连接建立时触发
        /// </summary>
        public event EventHandler? OnOpen;

        /// <summary>
        /// 客户端接收服务端数据时触发
        /// </summary>
        public event EventHandler? OnMessage;

        /// <summary>
        /// 通信发生错误时触发
        /// </summary>
        public event EventHandler? OnError;

        /// <summary>
        /// 连接关闭时触发
        /// </summary>
        public event EventHandler? OnClose;


        #endregion










        public void Start(string liveId)
        {
            LiveId = liveId;

            this.ConnectWss();
        }

        public void Stop()
        {
            clientWebSocket?.Abort();
        }

        public void ReStart()
        {
            this.Stop();

            this.Start(LiveId);
        }



        private void ConnectWss()
        {
            clientWebSocket = new ClientWebSocket();
            clientWebSocket.Options.SetRequestHeader("cookie", $"ttwid={Ttwid}");
            clientWebSocket.Options.SetRequestHeader("user-agent", UserAgent);


            Task.Run(async () =>
            {
                try
                {
                    //连接
                    await clientWebSocket.ConnectAsync(new Uri(Wss), CancellationToken.None);

                    if (clientWebSocket.State != WebSocketState.Open && clientWebSocket.State != WebSocketState.Connecting)
                    {
                        throw new Exception("连接服务器失败");
                    }

                    OnOpen?.Invoke(clientWebSocket, EventArgs.Empty);


                    //缓冲写大一些，就不用while循环分多次取
                    byte[] buffer = new byte[1024 * 1000];

                    //监听Socket信息，接收连接的套接字发来的数据
                    WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    //如果没有关闭，就一直接收
                    while (!result.CloseStatus.HasValue)
                    {
                        #region 处理消息
                        //将接收到的数据写到缓冲里
                        var package = PushFrame.Parser.ParseFrom(new MemoryStream(buffer, 0, result.Count));
                        var response = Response.Parser.ParseFrom(Decompress(package.Payload.ToArray()));

                        #region if NeedAck
                        if (response.NeedAck)
                        {
                            var ack = new PushFrame()
                            {
                                LogId = package.LogId,
                                PayloadType = "ack",
                                Payload = ByteString.CopyFromUtf8(response.InternalExt)
                            };

                            await clientWebSocket.SendAsync(new ArraySegment<byte>(ack.ToByteString().ToArray()), WebSocketMessageType.Binary, true, CancellationToken.None);
                        }
                        #endregion

                        #region 处理消息数据（这里只是写个例子）
                        if (response.MessagesList != null && response.MessagesList.Count > 0)
                        {
                            foreach (var message in response.MessagesList)
                            {
                                switch (message.Method)
                                {
                                    #region WebcastMemberMessage
                                    case "WebcastMemberMessage":
                                        {
                                            //OnMessage?.Invoke(clientWebSocket, new RoomMessageEventArgs<MemberMessage>()
                                            //{
                                            //    Type = MessageTypeEnum.Member,
                                            //    Message = MemberMessage.Parser.ParseFrom(message.Payload)
                                            //});

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(new
                                            {
                                                Type = MessageTypeEnum.Member,
                                                Message = MemberMessage.Parser.ParseFrom(message.Payload)
                                            });

                                            break;
                                        }
                                    #endregion

                                    #region WebcastSocialMessage
                                    case "WebcastSocialMessage":
                                        {
                                            //OnMessage?.Invoke(clientWebSocket, new RoomMessageEventArgs<SocialMessage>()
                                            //{
                                            //    Type = MessageTypeEnum.Social,
                                            //    Message = SocialMessage.Parser.ParseFrom(message.Payload)
                                            //});

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(new
                                            {
                                                Type = MessageTypeEnum.Social,
                                                Message = SocialMessage.Parser.ParseFrom(message.Payload)
                                            });

                                            break;
                                        }
                                    #endregion

                                    #region WebcastChatMessage
                                    case "WebcastChatMessage":
                                        {
                                            //OnMessage?.Invoke(clientWebSocket, new RoomMessageEventArgs<ChatMessage>()
                                            //{
                                            //    Type = MessageTypeEnum.Chat,
                                            //    Message = ChatMessage.Parser.ParseFrom(message.Payload)
                                            //});

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(new
                                            {
                                                Type = MessageTypeEnum.Chat,
                                                Message = ChatMessage.Parser.ParseFrom(message.Payload)
                                            });

                                            break;
                                        }
                                    #endregion

                                    #region WebcastLikeMessage
                                    case "WebcastLikeMessage":
                                        {
                                            //OnMessage?.Invoke(clientWebSocket, new RoomMessageEventArgs<LikeMessage>()
                                            //{
                                            //    Type = MessageTypeEnum.Like,
                                            //    Message = LikeMessage.Parser.ParseFrom(message.Payload)
                                            //});

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(new
                                            {
                                                Type = MessageTypeEnum.Like,
                                                Message = LikeMessage.Parser.ParseFrom(message.Payload)
                                            });

                                            break;
                                        }
                                    #endregion

                                    #region WebcastGiftMessage
                                    case "WebcastGiftMessage":
                                        {
                                            //OnMessage?.Invoke(clientWebSocket, new RoomMessageEventArgs<GiftMessage>()
                                            //{
                                            //    Type = MessageTypeEnum.Gift,
                                            //    Message = GiftMessage.Parser.ParseFrom(message.Payload)
                                            //});

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(new
                                            {
                                                Type = MessageTypeEnum.Gift,
                                                Message = GiftMessage.Parser.ParseFrom(message.Payload)
                                            });

                                            break;
                                        }
                                    #endregion

                                    #region WebcastRoomUserSeqMessage
                                    case "WebcastRoomUserSeqMessage":
                                        {
                                            //OnMessage?.Invoke(clientWebSocket, new RoomMessageEventArgs<RoomUserSeqMessage>()
                                            //{
                                            //    Type = MessageTypeEnum.RoomUserSeq,
                                            //    Message = RoomUserSeqMessage.Parser.ParseFrom(message.Payload)
                                            //});

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(new
                                            {
                                                Type = MessageTypeEnum.RoomUserSeq,
                                                Message = RoomUserSeqMessage.Parser.ParseFrom(message.Payload)
                                            });

                                            break;
                                        }
                                    #endregion

                                    #region WebcastControlMessage
                                    case "WebcastControlMessage":
                                        {
                                            //OnMessage?.Invoke(clientWebSocket, new RoomMessageEventArgs<ControlMessage>()
                                            //{
                                            //    Type = MessageTypeEnum.Control,
                                            //    Message = ControlMessage.Parser.ParseFrom(message.Payload)
                                            //});

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(new
                                            {
                                                Type = MessageTypeEnum.Control,
                                                Message = ControlMessage.Parser.ParseFrom(message.Payload)
                                            });

                                            break;
                                        }
                                    #endregion

                                    #region WebcastFansclubMessage
                                    case "WebcastFansclubMessage":
                                        {
                                            //OnMessage?.Invoke(clientWebSocket, new RoomMessageEventArgs<FansclubMessage>()
                                            //{
                                            //    Type = MessageTypeEnum.Fansclub,
                                            //    Message = FansclubMessage.Parser.ParseFrom(message.Payload)
                                            //});

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(new
                                            {
                                                Type = MessageTypeEnum.Fansclub,
                                                Message = FansclubMessage.Parser.ParseFrom(message.Payload)
                                            });

                                            break;
                                        }
                                    #endregion

                                    default:
                                        break;
                                }
                            }
                        }
                        #endregion

                        #endregion


                        //继续保持监听
                        if (clientWebSocket.State == WebSocketState.Open)
                        {
                            result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        }
                    }
                }
                catch (Exception)
                {
                    OnError?.Invoke(clientWebSocket, EventArgs.Empty);

                    // do something


                }
            });




        }






        #region 方法

        #region private byte[] Decompress(byte[] zippedData)
        private byte[] Decompress(byte[] zippedData)
        {
            MemoryStream ms = new MemoryStream(zippedData);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();
        }
        #endregion



        #region GenerateMsToken
        static Random random = new Random();
        private string GenerateMsToken(int length = 107)
        {
            string baseStr = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789=_";

            StringBuilder str = new StringBuilder();

            int len = baseStr.Length;
            for (int i = 0; i < length; i++)
            {
                str.Append(baseStr[random.Next(0, len)]);
            }

            return str.ToString();
        }
        #endregion


        #region private string? GetTwid()
        private string? GetTtwid()
        {
            try
            {
                int defailedCount = 0;
                string? temp_ttwid = string.Empty;

                while (true)
                {
                    try
                    {
                        using (RestClient client = new RestClient(GlobalConfig.LiveUrl_Douyin))
                        {
                            RestRequest request = new RestRequest($"/{LiveId}", Method.Get);
                            request.AddHeader("User-Agent", UserAgent);
                            request.AddCookie("__ac_nonce", "0" + GenerateMsToken(20), "/", "live.douyin.com"); //__ac_nonce 可以是任意值

                            RestResponse response = client.Execute(request);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                if (response.Cookies != null && response.Cookies.Count > 0)
                                {
                                    temp_ttwid = response.Cookies.Where(cookie => "ttwid".Equals(cookie.Name)).FirstOrDefault()?.Value;
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(temp_ttwid))
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("空值，再来一次");
                        }
                    }
                    catch (Exception)
                    {
                        defailedCount++;
                        temp_ttwid = null;
                    }

                    //实在带不动了，算了
                    if (defailedCount > 5)
                    {
                        break;
                    }
                }

                return temp_ttwid;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion


        #region private string? GetRoomId()
        private string? GetRoomId()
        {
            try
            {
                using (RestClient client = new RestClient(GlobalConfig.LiveUrl_Douyin))
                {
                    RestRequest request = new RestRequest($"/{LiveId}", Method.Get);

                    request.AddHeader("User-Agent", UserAgent);
                    request.AddHeader("cookie", $"ttwid={Ttwid}&msToken={GenerateMsToken()}; __ac_nonce=0{GenerateMsToken(20)}");

                    RestResponse response = client.Execute(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //正则表达式，提取roomid
                        var reg = new Regex(@"roomId\\"":\\""(?<roomId>\d+)\\""", RegexOptions.IgnoreCase);
                        Match _match = reg.Match(response.Content ?? "");
                        if (_match.Success)
                        {
                            return _match.Groups["roomId"].Value;
                        }
                    }
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion


        #region private string? GetWss()
        /// <summary>
        /// GetWss
        /// </summary>
        /// <returns></returns>
        private string GetWss()
        {
            StringBuilder wss = new StringBuilder();
            wss.Append($"wss://webcast3-ws-web-lq.douyin.com/webcast/im/push/v2/?");
            wss.Append("app_name=douyin_web&version_code=180800&webcast_sdk_version=1.3.0&update_version_code=1.3.0");
            wss.Append("&compress=gzip");
            wss.Append($"&internal_ext=internal_src:dim|wss_push_room_id:{RoomId}|wss_push_did:{RoomId}");
            wss.Append("|dim_log_id:202302171547011A160A7BAA76660E13ED|fetch_time:1676620021641|seq:1|wss_info:0-1676");
            wss.Append("620021641-0-0|wrds_kvs:WebcastRoomStatsMessage-1676620020691146024_WebcastRoomRankMessage-167661");
            wss.Append("9972726895075_AudienceGiftSyncData-1676619980834317696_HighlightContainerSyncData-2&cursor=t-1676");
            wss.Append("620021641_r-1_d-1_u-1_h-1");
            wss.Append("&host=https://live.douyin.com&aid=6383&live_id=1");
            wss.Append("&did_rule=3&debug=false&endpoint=live_pc&support_wrds=1&");
            wss.Append($"im_path=/webcast/im/fetch/&user_unique_id={RoomId}&");
            wss.Append("device_platform=web&cookie_enabled=true&screen_width=1440&screen_height=900&browser_language=zh&");
            wss.Append("browser_platform=MacIntel&browser_name=Mozilla&");
            wss.Append("browser_version=5.0%20(Macintosh;%20Intel%20Mac%20OS%20X%2010_15_7)%20AppleWebKit/537.36%20(KHTML,%20");
            wss.Append("like%20Gecko)%20Chrome/110.0.0.0%20Safari/537.36&");
            wss.Append("browser_online=true&tz_name=Asia/Shanghai&identity=audience&");
            wss.Append($"room_id={RoomId}&heartbeatDuration=0&signature=00000000");

            return wss.ToString();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion





        #endregion
    }
}

using BarrageGrab.Entity.Enums;
using BarrageGrab.Entity.Models.Douyin;
using BarrageGrab.Framework;
using BarrageGrab.Entity.Protobuf.Douyin;
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
using BarrageGrab.Framework.Utils;
using BarrageGrab.Entity.Models;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using BarrageGrab.Framework.Helper;

namespace BarrageGrab.GrabServices
{
    /// <summary>
    /// Douyin barrage grab service
    /// </summary>
    internal class DouyinBarrageGrabService : IBarrageGrabService, IDisposable
    {
        #region Attributes & Fields

        /// <summary>
        /// User-Agent
        /// </summary>
        private string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";


        /// <summary>
        /// liveid
        /// if live url is https://live.douyin.com/751990192217
        /// so 751990192217 is the liveid
        /// </summary>
        private string LiveId = string.Empty;

        /// <summary>
        /// websocket client
        /// </summary>
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
        /// on open
        /// </summary>
        public event EventHandler? OnOpen;

        /// <summary>
        /// on message
        /// </summary>
        public event EventHandler? OnMessage;

        /// <summary>
        /// on error
        /// </summary>
        public event EventHandler? OnError;

        /// <summary>
        /// on close
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
            try
            {
                clientWebSocket?.Abort();
                clientWebSocket?.Dispose();
                clientWebSocket = null;
            }
            catch (Exception ex)
            {
                // do something
            }
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
                    //connect
                    await clientWebSocket.ConnectAsync(new Uri(Wss), CancellationToken.None);

                    if (clientWebSocket.State != WebSocketState.Open && clientWebSocket.State != WebSocketState.Connecting)
                    {
                        throw new Exception("连接服务器失败");
                    }

                    OnOpen?.Invoke(clientWebSocket, EventArgs.Empty);


                    #region 发送hb心跳
                    try
                    {
                        byte[] heartbeat = new byte[] { 0x3a, 0x02, 0x68, 0x62 };
                        await clientWebSocket.SendAsync(new ArraySegment<byte>(heartbeat), WebSocketMessageType.Binary, true, CancellationToken.None);

                        var heartbeatTimer = new System.Timers.Timer(10000);
                        heartbeatTimer.Enabled = true;
                        heartbeatTimer.Start();
                        heartbeatTimer.Elapsed += (sender, e) =>
                        {
                            clientWebSocket?.SendAsync(new ArraySegment<byte>(heartbeat), WebSocketMessageType.Binary, true, CancellationToken.None);
                        };
                    }
                    catch (Exception ex)
                    {
                        //do something
                    }
                    #endregion


                    //缓冲写大一些，就不用while循环分多次取
                    byte[] buffer = new byte[1024 * 10000];

                    //监听Socket信息，接收连接的套接字发来的数据
                    WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    //如果没有关闭，就一直接收
                    while (!result.CloseStatus.HasValue)
                    {
                        #region 处理消息
                        //将接收到的数据写到缓冲里
                        var package = PushFrame.Parser.ParseFrom(new MemoryStream(buffer, 0, result.Count));
                        var response = Response.Parser.ParseFrom(DecompressHelper.Decompress(package.Payload.ToArray()));

                        #region if NeedAck
                        if (response.NeedAck)
                        {
                            PushFrame ack = new PushFrame()
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
                                    #region WebcastMemberMessage 进入
                                    case "WebcastMemberMessage":
                                        {
                                            MemberMessage memberMsg = MemberMessage.Parser.ParseFrom(message.Payload);

                                            OpenBarrageMessage obm = new OpenBarrageMessage()
                                            {
                                                Type = MessageTypeEnum.Member,
                                                Data = new DouyinMsgMember()
                                                {
                                                    MsgId = (long)memberMsg.Common.MsgId,
                                                    Content = $"{memberMsg.User.NickName} 来了",
                                                    RoomId = (long)memberMsg.Common.RoomId,
                                                    MemberCount = (long)memberMsg.MemberCount,
                                                    User = GetUser(memberMsg.User)
                                                }
                                            };

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(JsonConvert.SerializeObject(obm));

                                            ApplicationRuntime.MainForm?.PrintConsole($"[进入]{memberMsg.User.NickName} 来了");

                                            break;
                                        }
                                    #endregion

                                    #region WebcastSocialMessage 关注 & 分享
                                    case "WebcastSocialMessage":
                                        {
                                            SocialMessage socialMessage = SocialMessage.Parser.ParseFrom(message.Payload);

                                            #region 分享
                                            if (socialMessage.Action == 3)
                                            {
                                                OpenBarrageMessage obm = new OpenBarrageMessage()
                                                {
                                                    Type = MessageTypeEnum.Share,
                                                    Data = new DouyinMsgShare()
                                                    {
                                                        MsgId = (long)socialMessage.Common.MsgId,
                                                        Content = $"{socialMessage.User.NickName} 分享了直播间到{socialMessage.ShareTarget}",
                                                        RoomId = (long)socialMessage.Common.RoomId,
                                                        //ShareType = socialMessage.ShareTarget,
                                                        User = GetUser(socialMessage.User)
                                                    }
                                                };

                                                ApplicationRuntime.LocalWebSocketServer?.Broadcast(JsonConvert.SerializeObject(obm));

                                                ApplicationRuntime.MainForm?.PrintConsole($"[分享]{socialMessage.User.NickName} 分享了直播间到{socialMessage.ShareTarget}");
                                            }
                                            #endregion

                                            #region 关注
                                            else
                                            {
                                                OpenBarrageMessage obm = new OpenBarrageMessage()
                                                {
                                                    Type = MessageTypeEnum.Social,
                                                    Data = new DouyinMsgSocial()
                                                    {
                                                        MsgId = (long)socialMessage.Common.MsgId,
                                                        Content = $"{socialMessage.User.NickName} 关注了主播",
                                                        RoomId = (long)socialMessage.Common.RoomId,
                                                        User = GetUser(socialMessage.User)
                                                    }
                                                };

                                                ApplicationRuntime.LocalWebSocketServer?.Broadcast(JsonConvert.SerializeObject(obm));

                                                ApplicationRuntime.MainForm?.PrintConsole($"[关注]{socialMessage.User.NickName} 关注了主播");
                                            }
                                            #endregion

                                            break;
                                        }
                                    #endregion

                                    #region WebcastChatMessage 弹幕
                                    case "WebcastChatMessage":
                                        {
                                            ChatMessage chatMessage = ChatMessage.Parser.ParseFrom(message.Payload);

                                            OpenBarrageMessage obm = new OpenBarrageMessage()
                                            {
                                                Type = MessageTypeEnum.Chat,
                                                Data = new DouyinMsgChat()
                                                {
                                                    MsgId = (long)chatMessage.Common.MsgId,
                                                    Content = chatMessage.Content,
                                                    RoomId = (long)chatMessage.Common.RoomId,
                                                    User = GetUser(chatMessage.User)
                                                }
                                            };

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(JsonConvert.SerializeObject(obm));

                                            ApplicationRuntime.MainForm?.PrintConsole($"[弹幕]{chatMessage.User.NickName} 说 {chatMessage.Content}");

                                            break;
                                        }
                                    #endregion

                                    #region WebcastLikeMessage 点赞
                                    case "WebcastLikeMessage":
                                        {
                                            LikeMessage likeMessage = LikeMessage.Parser.ParseFrom(message.Payload);

                                            OpenBarrageMessage obm = new OpenBarrageMessage()
                                            {
                                                Type = MessageTypeEnum.Like,
                                                Data = new DouyinMsgLike()
                                                {
                                                    MsgId = (long)likeMessage.Common.MsgId,
                                                    Count = (long)likeMessage.Count,
                                                    Total = (long)likeMessage.Total,
                                                    Content = $"{likeMessage.User.NickName} 为主播点了{likeMessage.Count.ToString()}个赞，总点赞{likeMessage.Total.ToString()}",
                                                    RoomId = (long)likeMessage.Common.RoomId,
                                                    User = GetUser(likeMessage.User)
                                                }
                                            };

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(JsonConvert.SerializeObject(obm));

                                            ApplicationRuntime.MainForm?.PrintConsole($"[点赞]{likeMessage.User.NickName} 点了 {likeMessage.Count.ToString()} 个赞");

                                            break;
                                        }
                                    #endregion

                                    #region WebcastGiftMessage 礼物
                                    case "WebcastGiftMessage":
                                        {
                                            GiftMessage giftMessage = GiftMessage.Parser.ParseFrom(message.Payload);

                                            OpenBarrageMessage obm = new OpenBarrageMessage()
                                            {
                                                Type = MessageTypeEnum.Gift,
                                                Data = new DouyinMsgGift()
                                                {
                                                    MsgId = (long)giftMessage.Common.MsgId,
                                                    GiftId = (long)giftMessage.GiftId,
                                                    GiftName = giftMessage.Gift.Name,
                                                    GiftCount = (long)giftMessage.RepeatCount,
                                                    DiamondCount = (int)giftMessage.Gift.DiamondCount,
                                                    Content = $"{giftMessage.User.NickName} 送出 {giftMessage.Gift.Name}{(giftMessage.Gift.Combo ? "(可连击)" : "")} x {giftMessage.RepeatCount}个", //，增量{count}个
                                                    RoomId = (long)giftMessage.Common.RoomId,
                                                    User = GetUser(giftMessage.User),
                                                    ToUser = GetUser(giftMessage.ToUser)
                                                }
                                            };

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(JsonConvert.SerializeObject(obm));

                                            ApplicationRuntime.MainForm?.PrintConsole($"[礼物]{giftMessage.User.NickName} 送出 {giftMessage.RepeatCount.ToString()} 个 {giftMessage.Gift.Name}");

                                            break;
                                        }
                                    #endregion

                                    #region WebcastRoomUserSeqMessage 统计
                                    case "WebcastRoomUserSeqMessage":
                                        {
                                            RoomUserSeqMessage roomUserSeqMessage = RoomUserSeqMessage.Parser.ParseFrom(message.Payload);

                                            OpenBarrageMessage obm = new OpenBarrageMessage()
                                            {
                                                Type = MessageTypeEnum.RoomUserSeq,
                                                Data = new DouyinMsgRoomUserSeq()
                                                {
                                                    MsgId = (long)roomUserSeqMessage.Common.MsgId,
                                                    OnlineUserCount = roomUserSeqMessage.Total,
                                                    TotalUserCount = roomUserSeqMessage.TotalUser,
                                                    TotalUserCountStr = roomUserSeqMessage.TotalPvForAnchor,
                                                    OnlineUserCountStr = roomUserSeqMessage.OnlineUserForAnchor,
                                                    Content = $"当前直播间人数 {roomUserSeqMessage.OnlineUserForAnchor}，累计直播间人数 {roomUserSeqMessage.TotalPvForAnchor}",
                                                    RoomId = (long)roomUserSeqMessage.Common.RoomId,
                                                    User = null
                                                }
                                            };

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(JsonConvert.SerializeObject(obm));

                                            ApplicationRuntime.MainForm?.PrintConsole($"[人气统计]当前直播间人数 {roomUserSeqMessage.OnlineUserForAnchor}，累计直播间人数 {roomUserSeqMessage.TotalPvForAnchor}");

                                            break;
                                        }
                                    #endregion

                                    #region WebcastControlMessage 直播间状态变更
                                    case "WebcastControlMessage":
                                        {
                                            ControlMessage controlMessage = ControlMessage.Parser.ParseFrom(message.Payload);

                                            OpenBarrageMessage obm = new OpenBarrageMessage()
                                            {
                                                Type = MessageTypeEnum.Control,
                                                Data = new DouyinMsgControl()
                                                {
                                                    MsgId = (long)controlMessage.Common.MsgId,
                                                    Content = controlMessage.Status == 3 ? "直播已结束" : "",
                                                    RoomId = (long)controlMessage.Common.RoomId,
                                                    User = null
                                                }
                                            };

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(JsonConvert.SerializeObject(obm));

                                            ApplicationRuntime.MainForm?.PrintConsole($"[系统]当前直播已结束");

                                            break;
                                        }
                                    #endregion

                                    #region WebcastFansclubMessage 粉丝团
                                    case "WebcastFansclubMessage":
                                        {
                                            FansclubMessage fansclubMessage = FansclubMessage.Parser.ParseFrom(message.Payload);

                                            DouyinMsgFansClub douyinMsgFansClub = new DouyinMsgFansClub()
                                            {
                                                MsgId = (long)fansclubMessage.CommonInfo.MsgId,
                                                Type = fansclubMessage.Type,
                                                Content = fansclubMessage.Content,
                                                RoomId = (long)fansclubMessage.CommonInfo.RoomId,
                                                User = GetUser(fansclubMessage.User)
                                            };

                                            if (douyinMsgFansClub.User != null && douyinMsgFansClub.User.FansClub != null)
                                            {
                                                douyinMsgFansClub.Level = douyinMsgFansClub.User.FansClub.Level;
                                            }

                                            OpenBarrageMessage obm = new OpenBarrageMessage()
                                            {
                                                Type = MessageTypeEnum.Fansclub,
                                                Data = douyinMsgFansClub
                                            };

                                            ApplicationRuntime.LocalWebSocketServer?.Broadcast(JsonConvert.SerializeObject(obm));

                                            ApplicationRuntime.MainForm?.PrintConsole($"[粉丝团]{fansclubMessage.User.NickName} 加入了主播粉丝团");

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


                        //keep receive
                        if (clientWebSocket.State == WebSocketState.Open)
                        {
                            result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(clientWebSocket, EventArgs.Empty);

                    // do something
                }
            });
        }






        #region 方法





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
                        using (RestClient client = new RestClient(GlobalConfigs.LiveUrl_Douyin))
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
                using (RestClient client = new RestClient(GlobalConfigs.LiveUrl_Douyin))
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
            wss.Append($"wss://webcast3-ws-web-lq.douyin.com/webcast/im/push/v2/");
            wss.Append($"?app_name=douyin_web");
            wss.Append($"&version_code=180800");
            wss.Append($"&webcast_sdk_version=1.3.0");
            wss.Append($"&update_version_code=1.3.0");
            wss.Append($"&compress=gzip");
            wss.Append($"&internal_ext=internal_src:dim|wss_push_room_id:{RoomId}|wss_push_did:{RoomId}|dim_log_id:202302171547011A160A7BAA76660E13ED|fetch_time:1676620021641|seq:1|wss_info:0-1676620021641-0-0|wrds_kvs:WebcastRoomStatsMessage-1676620020691146024_WebcastRoomRankMessage-1676619972726895075_AudienceGiftSyncData-1676619980834317696_HighlightContainerSyncData-2");
            wss.Append($"&cursor=t-1676620021641_r-1_d-1_u-1_h-1");
            wss.Append($"&host=https://live.douyin.com");
            wss.Append($"&aid=6383");
            wss.Append($"&live_id=1");
            wss.Append($"&did_rule=3");
            wss.Append($"&debug=false");
            wss.Append($"&endpoint=live_pc");
            wss.Append($"&support_wrds=1");
            wss.Append($"&im_path=/webcast/im/fetch/");
            wss.Append($"&user_unique_id={RoomId}");
            wss.Append($"&device_platform=web");
            wss.Append($"&cookie_enabled=true");
            wss.Append($"&screen_width=1440");
            wss.Append($"&screen_height=900");
            wss.Append($"&browser_language=zh");
            wss.Append($"&browser_platform=MacIntel");
            wss.Append($"&browser_name=Mozilla");
            wss.Append($"&browser_version=5.0%20(Macintosh;%20Intel%20Mac%20OS%20X%2010_15_7)%20AppleWebKit/537.36%20(KHTML,%20like%20Gecko)%20Chrome/110.0.0.0%20Safari/537.36");
            wss.Append($"&browser_online=true");
            wss.Append($"&tz_name=Asia/Shanghai");
            wss.Append($"&identity=audience");
            wss.Append($"&room_id={RoomId}");
            wss.Append($"&heartbeatDuration=0");

            //signature=00000000 wss并没有进行签名，稳定性肯定没签名过的稳定。如需签名及算法，请咨询群主
            wss.Append($"&signature=00000000");

            return wss.ToString();
        }

        public void Dispose()
        {

        }

        #endregion


        #region private DouyinUser? GetUser(User data)
        private DouyinUser? GetUser(User data)
        {
            if (data == null)
            {
                return null;
            }

            DouyinUser user = new DouyinUser()
            {
                DisplayId = data.DisplayId,
                ShortId = (long)data.ShortId,
                Gender = (int)data.Gender,
                Id = (long)data.Id,
                Level = (int)data.Level,
                PayLevel = (int)(data.PayGrade?.Level ?? -1),
                NickName = data.NickName ?? "用户" + data.DisplayId,
                Avatar = data.AvatarThumb?.UrlListList?.FirstOrDefault() ?? "",
                SecUid = data.SecUid,
                FollowerCount = (long)(data.FollowInfo?.FollowerCount ?? 0),
                FollowingCount = (long)(data.FollowInfo?.FollowingCount ?? 0),
                FollowStatus = (long)(data.FollowInfo?.FollowStatus ?? 0)
            };

            if (data.FansClub != null && data.FansClub.Data != null)
            {
                user.FansClub = new DouyinFansClub()
                {
                    ClubName = data.FansClub.Data.ClubName,
                    Level = data.FansClub.Data.Level
                };
            }

            return user;
        }
        #endregion


        #endregion
    }
}

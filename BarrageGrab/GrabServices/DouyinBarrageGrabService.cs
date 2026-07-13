using BarrageGrab.Entity.Enums;
using BarrageGrab.Entity.Models;
using BarrageGrab.Entity.Models.Douyin;
using BarrageGrab.Entity.Protobuf.Douyin;
using BarrageGrab.Entity.Requests;
using BarrageGrab.Framework.Helper;
using Google.Protobuf;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace BarrageGrab.GrabServices
{
    /// <summary>
    /// Douyin barrage grab service
    /// </summary>
    internal class DouyinBarrageGrabService : IBarrageGrabService, IDisposable
    {
        private const int ReceiveBufferSize = 64 * 1024;
        private static readonly byte[] HeartbeatPayload = [0x3a, 0x02, 0x68, 0x62];

        private readonly string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";

        private string liveId = string.Empty;
        private string? userUniqueId;
        private ClientWebSocket? clientWebSocket;
        private CancellationTokenSource? connectionCts;
        private System.Timers.Timer? heartbeatTimer;
        private bool disposed;

        private string? _ttwid;
        private string? _roomid;
        private string? _wss;

        public event EventHandler? OnOpen;
        public event EventHandler? OnMessage;
        public event EventHandler? OnError;
        public event EventHandler? OnClose;

        public void Start(string liveId)
        {
            Stop();

            this.liveId = ExtractLiveId(liveId);
            ResetConnectionState();
            connectionCts = new CancellationTokenSource();
            ConnectWss();
        }

        public void Stop()
        {
            connectionCts?.Cancel();
            connectionCts?.Dispose();
            connectionCts = null;

            heartbeatTimer?.Stop();
            heartbeatTimer?.Dispose();
            heartbeatTimer = null;

            try
            {
                if (clientWebSocket != null)
                {
                    if (clientWebSocket.State == WebSocketState.Open)
                    {
                        clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Stopped", CancellationToken.None)
                            .GetAwaiter().GetResult();
                    }

                    clientWebSocket.Dispose();
                }
            }
            catch
            {
                // ignore shutdown errors
            }
            finally
            {
                clientWebSocket = null;
            }
        }

        public void ReStart()
        {
            Start(liveId);
        }

        private void ResetConnectionState()
        {
            _ttwid = null;
            _roomid = null;
            _wss = null;
            userUniqueId = null;
        }

        private static string ExtractLiveId(string input)
        {
            input = input.Trim();
            if (Uri.TryCreate(input, UriKind.Absolute, out var uri))
            {
                var segment = uri.AbsolutePath.Trim('/');
                if (!string.IsNullOrEmpty(segment))
                {
                    return segment.Split('/').Last();
                }
            }

            return input;
        }

        private void ConnectWss()
        {
            clientWebSocket = new ClientWebSocket();
            clientWebSocket.Options.SetRequestHeader("cookie", $"ttwid={Ttwid}");
            clientWebSocket.Options.SetRequestHeader("user-agent", userAgent);

            var token = connectionCts?.Token ?? CancellationToken.None;

            _ = Task.Run(async () =>
            {
                try
                {
                    var wssUrl = Wss;
                    if (string.IsNullOrWhiteSpace(wssUrl))
                    {
                        throw new InvalidOperationException("获取WSS地址失败，请检查直播间ID或签名服务是否可用");
                    }

                    await clientWebSocket.ConnectAsync(new Uri(wssUrl), token).ConfigureAwait(false);

                    if (clientWebSocket.State != WebSocketState.Open)
                    {
                        throw new InvalidOperationException("连接服务器失败");
                    }

                    OnOpen?.Invoke(clientWebSocket, EventArgs.Empty);
                    StartHeartbeat(token);

                    var buffer = new byte[ReceiveBufferSize];
                    while (!token.IsCancellationRequested && clientWebSocket.State == WebSocketState.Open)
                    {
                        var payload = await ReceiveFullMessageAsync(clientWebSocket, buffer, token).ConfigureAwait(false);
                        if (payload.Length == 0)
                        {
                            break;
                        }

                        await ProcessPayloadAsync(payload).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    // expected during Stop()
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(clientWebSocket, EventArgs.Empty);
                    ApplicationRuntime.MainWindow?.PrintConsole($"[异常]{ex.Message}");
                }
                finally
                {
                    OnClose?.Invoke(clientWebSocket, EventArgs.Empty);
                }
            }, token);
        }

        private void StartHeartbeat(CancellationToken token)
        {
            heartbeatTimer?.Dispose();
            heartbeatTimer = new System.Timers.Timer(10000)
            {
                AutoReset = true,
                Enabled = true
            };

            heartbeatTimer.Elapsed += async (_, _) =>
            {
                if (token.IsCancellationRequested || clientWebSocket?.State != WebSocketState.Open)
                {
                    return;
                }

                try
                {
                    await clientWebSocket.SendAsync(
                        new ArraySegment<byte>(HeartbeatPayload),
                        WebSocketMessageType.Binary,
                        true,
                        token).ConfigureAwait(false);
                }
                catch
                {
                    // connection may already be closed
                }
            };

            _ = clientWebSocket!.SendAsync(
                new ArraySegment<byte>(HeartbeatPayload),
                WebSocketMessageType.Binary,
                true,
                token);
        }

        private static async Task<byte[]> ReceiveFullMessageAsync(ClientWebSocket webSocket, byte[] buffer, CancellationToken token)
        {
            using var messageStream = new MemoryStream();
            WebSocketReceiveResult result;

            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token).ConfigureAwait(false);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    return Array.Empty<byte>();
                }

                messageStream.Write(buffer, 0, result.Count);
            }
            while (!result.EndOfMessage);

            return messageStream.ToArray();
        }

        private async Task ProcessPayloadAsync(byte[] payload)
        {
            var package = PushFrame.Parser.ParseFrom(payload);
            var response = Response.Parser.ParseFrom(DecompressHelper.Decompress(package.Payload.ToByteArray()));

            if (response.NeedAck && clientWebSocket?.State == WebSocketState.Open)
            {
                var ack = new PushFrame
                {
                    LogId = package.LogId,
                    PayloadType = "ack",
                    Payload = ByteString.CopyFromUtf8(response.InternalExt)
                };

                await clientWebSocket.SendAsync(
                    new ArraySegment<byte>(ack.ToByteString().ToByteArray()),
                    WebSocketMessageType.Binary,
                    true,
                    connectionCts?.Token ?? CancellationToken.None).ConfigureAwait(false);
            }

            if (response.MessagesList == null || response.MessagesList.Count == 0)
            {
                return;
            }

            foreach (var message in response.MessagesList)
            {
                await HandleMessageAsync(message).ConfigureAwait(false);
            }
        }

        private async Task HandleMessageAsync(BarrageGrab.Entity.Protobuf.Douyin.Message message)
        {
            switch (message.Method)
            {
                case "WebcastMemberMessage":
                    {
                        var memberMsg = MemberMessage.Parser.ParseFrom(message.Payload);
                        await PublishMessageAsync(
                            new OpenBarrageMessage
                            {
                                Type = MessageTypeEnum.Member,
                                Data = new DouyinMsgMember
                                {
                                    MsgId = (long)memberMsg.Common.MsgId,
                                    Content = $"{memberMsg.User.NickName} 来了",
                                    RoomId = (long)memberMsg.Common.RoomId,
                                    MemberCount = (long)memberMsg.MemberCount,
                                    User = GetUser(memberMsg.User)
                                }
                            },
                            $"[进入]{memberMsg.User.NickName} 来了").ConfigureAwait(false);
                        break;
                    }

                case "WebcastSocialMessage":
                    {
                        var socialMessage = SocialMessage.Parser.ParseFrom(message.Payload);
                        if (socialMessage.Action == 3)
                        {
                            await PublishMessageAsync(
                                new OpenBarrageMessage
                                {
                                    Type = MessageTypeEnum.Share,
                                    Data = new DouyinMsgShare
                                    {
                                        MsgId = (long)socialMessage.Common.MsgId,
                                        Content = $"{socialMessage.User.NickName} 分享了直播间到{socialMessage.ShareTarget}",
                                        RoomId = (long)socialMessage.Common.RoomId,
                                        User = GetUser(socialMessage.User)
                                    }
                                },
                                $"[分享]{socialMessage.User.NickName} 分享了直播间到{socialMessage.ShareTarget}").ConfigureAwait(false);
                        }
                        else
                        {
                            await PublishMessageAsync(
                                new OpenBarrageMessage
                                {
                                    Type = MessageTypeEnum.Social,
                                    Data = new DouyinMsgSocial
                                    {
                                        MsgId = (long)socialMessage.Common.MsgId,
                                        Content = $"{socialMessage.User.NickName} 关注了主播",
                                        RoomId = (long)socialMessage.Common.RoomId,
                                        User = GetUser(socialMessage.User)
                                    }
                                },
                                $"[关注]{socialMessage.User.NickName} 关注了主播").ConfigureAwait(false);
                        }

                        break;
                    }

                case "WebcastChatMessage":
                    {
                        var chatMessage = ChatMessage.Parser.ParseFrom(message.Payload);
                        await PublishMessageAsync(
                            new OpenBarrageMessage
                            {
                                Type = MessageTypeEnum.Chat,
                                Data = new DouyinMsgChat
                                {
                                    MsgId = (long)chatMessage.Common.MsgId,
                                    Content = chatMessage.Content,
                                    RoomId = (long)chatMessage.Common.RoomId,
                                    User = GetUser(chatMessage.User)
                                }
                            },
                            $"[弹幕]{chatMessage.User.NickName} 说 {chatMessage.Content}").ConfigureAwait(false);
                        break;
                    }

                case "WebcastLikeMessage":
                    {
                        var likeMessage = LikeMessage.Parser.ParseFrom(message.Payload);
                        await PublishMessageAsync(
                            new OpenBarrageMessage
                            {
                                Type = MessageTypeEnum.Like,
                                Data = new DouyinMsgLike
                                {
                                    MsgId = (long)likeMessage.Common.MsgId,
                                    Count = (long)likeMessage.Count,
                                    Total = (long)likeMessage.Total,
                                    Content = $"{likeMessage.User.NickName} 为主播点了{likeMessage.Count}个赞，总点赞{likeMessage.Total}",
                                    RoomId = (long)likeMessage.Common.RoomId,
                                    User = GetUser(likeMessage.User)
                                }
                            },
                            $"[点赞]{likeMessage.User.NickName} 点了 {likeMessage.Count} 个赞").ConfigureAwait(false);
                        break;
                    }

                case "WebcastGiftMessage":
                    {
                        var giftMessage = GiftMessage.Parser.ParseFrom(message.Payload);
                        await PublishMessageAsync(
                            new OpenBarrageMessage
                            {
                                Type = MessageTypeEnum.Gift,
                                Data = new DouyinMsgGift
                                {
                                    MsgId = (long)giftMessage.Common.MsgId,
                                    GiftId = (long)giftMessage.GiftId,
                                    GiftName = giftMessage.Gift.Name,
                                    TotalCount = (long)giftMessage.TotalCount,
                                    RepeatCount = (long)giftMessage.RepeatCount,
                                    RepeatEnd = (int)giftMessage.RepeatEnd,
                                    ComboCount = (long)giftMessage.ComboCount,
                                    GroupCount = (long)giftMessage.GroupCount,
                                    DiamondCount = (int)giftMessage.Gift.DiamondCount,
                                    Content = $"{giftMessage.User.NickName} 送出 {giftMessage.Gift.Name}{(giftMessage.Gift.Combo ? "(可连击)" : "")} x {giftMessage.RepeatCount}个",
                                    RoomId = (long)giftMessage.Common.RoomId,
                                    User = GetUser(giftMessage.User),
                                    ToUser = GetUser(giftMessage.ToUser)
                                }
                            },
                            $"[礼物]{giftMessage.User.NickName} 送出 {giftMessage.RepeatCount} 个 {giftMessage.Gift.Name}").ConfigureAwait(false);
                        break;
                    }

                case "WebcastRoomUserSeqMessage":
                    {
                        var roomUserSeqMessage = RoomUserSeqMessage.Parser.ParseFrom(message.Payload);
                        await PublishMessageAsync(
                            new OpenBarrageMessage
                            {
                                Type = MessageTypeEnum.RoomUserSeq,
                                Data = new DouyinMsgRoomUserSeq
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
                            },
                            $"[人气统计]当前直播间人数 {roomUserSeqMessage.OnlineUserForAnchor}，累计直播间人数 {roomUserSeqMessage.TotalPvForAnchor}").ConfigureAwait(false);
                        break;
                    }

                case "WebcastControlMessage":
                    {
                        var controlMessage = ControlMessage.Parser.ParseFrom(message.Payload);
                        await PublishMessageAsync(
                            new OpenBarrageMessage
                            {
                                Type = MessageTypeEnum.Control,
                                Data = new DouyinMsgControl
                                {
                                    MsgId = (long)controlMessage.Common.MsgId,
                                    Content = controlMessage.Status == 3 ? "直播已结束" : string.Empty,
                                    RoomId = (long)controlMessage.Common.RoomId,
                                    User = null
                                }
                            },
                            "[系统]当前直播已结束").ConfigureAwait(false);
                        break;
                    }

                case "WebcastFansclubMessage":
                    {
                        var fansclubMessage = FansclubMessage.Parser.ParseFrom(message.Payload);
                        var fansClubMessage = new DouyinMsgFansClub
                        {
                            MsgId = (long)fansclubMessage.CommonInfo.MsgId,
                            Type = fansclubMessage.Type,
                            Content = fansclubMessage.Content,
                            RoomId = (long)fansclubMessage.CommonInfo.RoomId,
                            User = GetUser(fansclubMessage.User)
                        };

                        if (fansClubMessage.User?.FansClub != null)
                        {
                            fansClubMessage.Level = fansClubMessage.User.FansClub.Level;
                        }

                        await PublishMessageAsync(
                            new OpenBarrageMessage
                            {
                                Type = MessageTypeEnum.Fansclub,
                                Data = fansClubMessage
                            },
                            $"[粉丝团]{fansclubMessage.User.NickName} 加入了主播粉丝团").ConfigureAwait(false);
                        break;
                    }
            }
        }

        private static async Task PublishMessageAsync(OpenBarrageMessage message, string consoleMessage)
        {
            var payload = JsonConvert.SerializeObject(message);
            if (ApplicationRuntime.LocalWebSocketServer != null)
            {
                await ApplicationRuntime.LocalWebSocketServer.Broadcast(payload).ConfigureAwait(false);
            }

            ApplicationRuntime.MainWindow?.PrintConsole(consoleMessage);
        }

        private string GenerateMsToken(int length = 107)
        {
            const string baseStr = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789=_";
            var builder = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                builder.Append(baseStr[Random.Shared.Next(baseStr.Length)]);
            }

            return builder.ToString();
        }

        private string? Ttwid
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_ttwid))
                {
                    return _ttwid;
                }

                _ttwid = GetTtwid();
                return _ttwid;
            }
        }

        private string? RoomId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_roomid))
                {
                    return _roomid;
                }

                _roomid = GetRoomId();
                return _roomid;
            }
        }

        private string? Wss
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_wss))
                {
                    return _wss;
                }

                _wss = GetWss();
                return _wss;
            }
        }

        private string? GetTtwid()
        {
            var failedCount = 0;
            string? tempTtwid = null;

            while (failedCount <= 5)
            {
                try
                {
                    using var client = new RestClient(GlobalConfigs.LiveUrl_Douyin);
                    var request = new RestRequest($"/{liveId}", Method.Get);
                    request.AddHeader("User-Agent", userAgent);
                    request.AddCookie("__ac_nonce", "0" + GenerateMsToken(20), "/", "live.douyin.com");

                    var response = client.Execute(request);
                    if (response.StatusCode == HttpStatusCode.OK && response.Cookies?.Count > 0)
                    {
                        tempTtwid = response.Cookies.FirstOrDefault(cookie => cookie.Name == "ttwid")?.Value;
                    }

                    if (!string.IsNullOrWhiteSpace(tempTtwid))
                    {
                        return tempTtwid;
                    }
                }
                catch
                {
                    // retry
                }

                failedCount++;
            }

            return null;
        }

        private string? GetRoomId()
        {
            try
            {
                using var client = new RestClient(GlobalConfigs.LiveUrl_Douyin);
                var request = new RestRequest($"/{liveId}", Method.Get);
                request.AddHeader("User-Agent", userAgent);
                request.AddHeader("cookie", $"ttwid={Ttwid}&msToken={GenerateMsToken()}; __ac_nonce=0{GenerateMsToken(20)}");

                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                var content = response.Content ?? string.Empty;
                var userUniqueIdMatch = Regex.Match(content, @"user_unique_id\\"":\\""(?<userUniqueId>\d+)\\""", RegexOptions.IgnoreCase);
                if (userUniqueIdMatch.Success)
                {
                    userUniqueId = userUniqueIdMatch.Groups["userUniqueId"].Value;
                }

                var roomIdMatch = Regex.Match(content, @"roomId\\"":\\""(?<roomId>\d+)\\""", RegexOptions.IgnoreCase);
                if (roomIdMatch.Success)
                {
                    return roomIdMatch.Groups["roomId"].Value;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        private string? GetWss()
        {
            using var client = new RestClient(GlobalConfigs.SignApi_Domain);
            var request = new RestRequest(GlobalConfigs.SignApi_Url, Method.Post);
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "application/json;charset=UTF-8");

            request.AddJsonBody(new SignWssRequest
            {
                ApiKey = GlobalConfigs.SignApi_Key,
                BrowserName = "Mozilla",
                BrowserVersion = userAgent,
                RoomId = RoomId,
                UserUniqueId = userUniqueId
            });
            var response = client.Execute(request);
            if (response.Content == null)
            {
                return null;
            }

            var json = JsonNode.Parse(response.Content);
            if (json?["Code"]?.GetValue<int>() != 0)
            {
                var errorMessage = json?["Msg"]?.GetValue<string>() ?? "签名服务返回异常";
                throw new InvalidOperationException(errorMessage);
            }

            return json["Data"]?["WssUrl"]?.GetValue<string>();
        }

        private static DouyinUser? GetUser(User? data)
        {
            if (data == null)
            {
                return null;
            }

            var user = new DouyinUser
            {
                DisplayId = data.DisplayId,
                ShortId = (long)data.ShortId,
                Gender = (int)data.Gender,
                Id = (long)data.Id,
                Level = (int)data.Level,
                PayLevel = (int)(data.PayGrade?.Level ?? -1),
                NickName = data.NickName ?? "用户" + data.DisplayId,
                Avatar = data.AvatarThumb?.UrlListList?.FirstOrDefault() ?? string.Empty,
                SecUid = data.SecUid,
                FollowerCount = (long)(data.FollowInfo?.FollowerCount ?? 0),
                FollowingCount = (long)(data.FollowInfo?.FollowingCount ?? 0),
                FollowStatus = (long)(data.FollowInfo?.FollowStatus ?? 0)
            };

            if (data.FansClub?.Data != null)
            {
                user.FansClub = new DouyinFansClub
                {
                    ClubName = data.FansClub.Data.ClubName,
                    Level = data.FansClub.Data.Level
                };
            }

            return user;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            Stop();
            disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}

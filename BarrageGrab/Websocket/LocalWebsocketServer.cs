using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BarrageGrab.Websocket
{
    /// <summary>
    /// 本地WebSocket服务器类
    /// </summary>
    internal class LocalWebSocketServer : IDisposable
    {
        #region 属性&字段

        /// <summary>
        /// WebSocket实例
        /// </summary>
        private WebSocketServer? socketServer = null;

        /// <summary>
        /// 连接的客户端
        /// </summary>
        private Dictionary<string, IWebSocketConnection>? clientList;

        /// <summary>
        /// 要移除的客户端列表
        /// </summary>
        private List<string>? removeList;

        #endregion


        #region public void Run()
        public void Start()
        {
            if (socketServer == null)
            {
                socketServer = new WebSocketServer(GlobalConfig.LocalWebSocketServer_Location);
            }

            //异常重启
            socketServer.RestartAfterListenError = true;
            socketServer.Start(ListenWebSocketConnection);
        }
        #endregion

        #region public void ReBoot()
        public void ReStart()
        {
            if (socketServer != null)
            {
                socketServer.Dispose();
                socketServer = null;
            }

            this.Start();
        }
        #endregion


        #region private void ListenWebSocketConnection(IWebSocketConnection client)
        private void ListenWebSocketConnection(IWebSocketConnection client)
        {
            string clientId = client.ConnectionInfo.Id.ToString();


            #region OnOpen
            client.OnOpen = () =>
            {
                if (clientList == null || clientList.Count == 0)
                {
                    clientList = new Dictionary<string, IWebSocketConnection>();
                }

                if (!clientList.ContainsKey(clientId))
                {
                    clientList.Add(clientId, client);
                }
            };
            #endregion


            #region OnMessage
            client.OnMessage = (message) =>
            {
                //Broadcast(message);
            };
            #endregion


            #region OnClose
            client.OnClose = () =>
            {
                if (clientList != null && clientList.Count > 0)
                {
                    clientList.Remove(clientId);
                }
            };
            #endregion


            #region OnPing
            client.OnPing = (data) =>
            {

            };
            #endregion
        }
        #endregion


        #region public void Broadcast(string message)
        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="message"></param>
        public async Task Broadcast(object message)
        {
            //打印控制台（这句代码不应该写这里，我是为了测试方便，有空再挪）
            ApplicationRuntime.MainForm?.PrintConsole(JsonConvert.SerializeObject(message));

            //广播给所有连接的客户端
            if (clientList == null || clientList.Count == 0)
            {
                return;
            }

            removeList = new List<string>();

            foreach (var client in clientList)
            {
                if (client.Value.IsAvailable)
                {
                    await client.Value.Send(JsonConvert.SerializeObject(message));
                }
                else
                {
                    removeList.Add(client.Key);
                }
            }

            if (removeList != null && removeList.Count > 0)
            {
                removeList.ForEach(clientId =>
                {
                    clientList.Remove(clientId);
                });
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}

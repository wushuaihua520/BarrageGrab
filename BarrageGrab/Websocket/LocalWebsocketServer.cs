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
    /// local websocket server
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
            try
            {
                if (socketServer == null)
                {
                    socketServer = new WebSocketServer(GlobalConfigs.LocalWebSocketServer_Location);
                }

                //restart
                socketServer.RestartAfterListenError = true;
                socketServer.Start(ListenWebSocketConnection);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Local webSocket server fail to start：" + ex.Message);
            }
        }
        #endregion

        #region public void ReStart()
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
        /// Broadcast
        /// </summary>
        /// <param name="message"></param>
        public async Task Broadcast(string message)
        {
            //Broadcast to all clients
            if (clientList == null || clientList.Count == 0)
            {
                return;
            }

            removeList = new List<string>();

            foreach (var client in clientList)
            {
                if (client.Value.IsAvailable)
                {
                    await client.Value.Send(message);
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

        #endregion


        public void Dispose()
        {
            if (socketServer != null)
            {
                socketServer.Dispose();
                socketServer = null;
            }

            clientList = null;

            removeList = null;
        }
    }
}

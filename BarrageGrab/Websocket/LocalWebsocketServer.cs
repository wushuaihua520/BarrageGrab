using Fleck;
using System.Collections.Concurrent;

namespace BarrageGrab.Websocket
{
    /// <summary>
    /// local websocket server
    /// </summary>
    internal class LocalWebSocketServer : IDisposable
    {
        private WebSocketServer? socketServer;
        private readonly ConcurrentDictionary<string, IWebSocketConnection> clientList = new();

        public void Start()
        {
            try
            {
                if (socketServer == null)
                {
                    socketServer = new WebSocketServer(GlobalConfigs.LocalWebSocketServer_Location);
                }

                socketServer.RestartAfterListenError = true;
                socketServer.Start(ListenWebSocketConnection);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Local webSocket server fail to start：" + ex.Message);
            }
        }

        public void ReStart()
        {
            if (socketServer != null)
            {
                socketServer.Dispose();
                socketServer = null;
            }

            clientList.Clear();
            Start();
        }

        private void ListenWebSocketConnection(IWebSocketConnection client)
        {
            string clientId = client.ConnectionInfo.Id.ToString();

            client.OnOpen = () =>
            {
                clientList.TryAdd(clientId, client);
            };

            client.OnMessage = _ => { };

            client.OnClose = () =>
            {
                clientList.TryRemove(clientId, out _);
            };

            client.OnPing = _ => { };
        }

        public async Task Broadcast(string message)
        {
            if (clientList.IsEmpty)
            {
                return;
            }

            foreach (var entry in clientList)
            {
                var connection = entry.Value;
                if (connection.IsAvailable)
                {
                    try
                    {
                        await connection.Send(message).ConfigureAwait(false);
                    }
                    catch
                    {
                        clientList.TryRemove(entry.Key, out _);
                    }
                }
                else
                {
                    clientList.TryRemove(entry.Key, out _);
                }
            }
        }

        public void Dispose()
        {
            if (socketServer != null)
            {
                socketServer.Dispose();
                socketServer = null;
            }

            clientList.Clear();
        }
    }
}

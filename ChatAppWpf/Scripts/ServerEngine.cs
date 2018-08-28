using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ChatApp
{
    class ServerEngine
    {
        public event EventHandler<Message> OnMessageReceived;
        public event EventHandler<TcpClient> OnClientConnected;
        public event EventHandler<TcpClient> OnClientDisconnected;
        public event EventHandler<Exception> OnExceptionOccured;

        private static List<TcpClient> _clientObjects = new List<TcpClient>();
        private static List<NetworkStream> _clientStreams = new List<NetworkStream>(); //Pretty useless.

        private static ServerEngine _serverEngine;
        private static TcpListener _server;
        private static readonly object padlock = new object();

        internal Message MOTD = new Message("Hello there!", "Server");

        private ServerEngine()
        {

        }
        
        public static ServerEngine Server
        {
            get
            {
                lock (padlock)
                {
                    if (_serverEngine == null)
                    {
                        _serverEngine = new ServerEngine();
                    }
                    return _serverEngine;
                }
            }
        }

        public async Task StartAsync(int port)
        {
            _server = TcpListener.Create(port);
            _server.Start();
            while (true)
            {
                await AcceptPendingConnectionsAsync();
                await ReceiveMessageAsync();
                await Task.Delay(200);
            }
        }

        public async Task AcceptPendingConnectionsAsync()
        {
            while (_server.Pending())
            {
                var client = await _server.AcceptTcpClientAsync();
                _clientObjects.Add(client);
                OnClientConnected?.Invoke(this, client);
            }
        }

        public async Task GreetNewcomerAsync()
        {
            var motdBytes = MOTD.GetBytes();
            var client = _clientObjects.Last();
            await SendInfoPacketAsync(client, MOTD);
            await client.GetStream().WriteAsync(motdBytes, 0, motdBytes.Length);
        }

        public async Task BroadcastAsync(Message msg)
        {
            if (_clientObjects.Count > 0)
            {
                var msgBytes = Encoding.UTF8.GetBytes(msg.Content);
                List<TcpClient> badClients = new List<TcpClient>();
                foreach (var client in _clientObjects)
                {
                    try
                    {
                        await SendInfoPacketAsync(client, msg);
                        await client.GetStream().WriteAsync(msgBytes, 0, msgBytes.Length);
                    }
                    catch (Exception ex)
                    {
                        OnExceptionOccured?.Invoke(this, ex);
                        badClients.Add(client);
                    }
                }
                _clientObjects = _clientObjects.Except(badClients).ToList();
            }
        }

        public async Task SendInfoPacketAsync(TcpClient client, Message msg)
        {
            byte[] infoBytes = new byte[40]; //32 bytes for nickname and other 8 for additional stuff.
            Encoding.UTF8.GetBytes(msg.Author.UserName).CopyTo(infoBytes, 0);
            Encoding.UTF8.GetBytes(msg.Content.Length.ToString()).CopyTo(infoBytes, 32); //first few bites of the last 8 bytes is message length.
            await client.GetStream().WriteAsync(infoBytes, 0, 40);
        }

        public async Task ReceiveMessageAsync()
        {
            if (_clientObjects.Count > 0)
            {
                List<TcpClient> badClients = new List<TcpClient>();
                foreach (var client in _clientObjects)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        if (stream.DataAvailable)
                        {
                            string[] infoPacket = await ReceiveInfoPacketAsync(stream);
                            int messageLength = Convert.ToInt32(infoPacket[1]);
                            byte[] data = new byte[messageLength];
                            int receivedDataLength = await stream.ReadAsync(data, 0, data.Length);
                            Message receivedMessage = new Message(Encoding.UTF8.GetString(data, 0, receivedDataLength),
                                infoPacket[0]);
                            OnMessageReceived?.Invoke(this, receivedMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        OnExceptionOccured?.Invoke(this, ex);
                        badClients.Add(client);
                    }
                }

                _clientObjects = _clientObjects.Except(badClients).ToList();
                //foreach (NetworkStream stream in Streams)
                //{
                //    if (stream.DataAvailable)
                //    {
                //        string[] infoPacket = await ReceiveInfoPacket(stream);
                //        int messageLength = Convert.ToInt32(infoPacket[1]);
                //        byte[] data = new byte[messageLength];
                //        int ReceivedDataLength = stream.Read(data, 0, data.Length);
                //        Message ReceivedMessage = new Message(Encoding.UTF8.GetString(data, 0, ReceivedDataLength), infoPacket[0]);
                //        OnMessageReceived(ReceivedMessage);
                //        await Task.Delay(1000);
                //    }
                //}
            }
        }

        public async Task<string[]> ReceiveInfoPacketAsync(NetworkStream stream)
        {
            byte[] receivedBytes = new byte[40];
            await stream.ReadAsync(receivedBytes, 0, 40);
            string receivedMessage = Encoding.UTF8.GetString(receivedBytes);
            return new string[] { receivedMessage.Substring(0, 32).Trim(' ', '\0'), receivedMessage.Substring(32).Trim(' ', '\0') };
        }

        /*private async Task ReceiveHeartbeatAsync() //need another port for this, so not gonna use it for now.
        {
            //useful code here?
            List<TcpClient> badClients = new List<TcpClient>();
            foreach (var client in _clientObjects)
            {
                if (client.GetStream().DataAvailable)
                {
                    byte[] heartbeat = new byte[1];
                    await client.GetStream().ReadAsync(heartbeat, 0, 1);
                    if (heartbeat[0] != 1) badClients.Add(client);
                }
            }
            _clientObjects = _clientObjects.Except(badClients).ToList();
        }*/
    }
}

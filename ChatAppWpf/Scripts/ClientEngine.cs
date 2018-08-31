using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatApp
{
    class ClientEngine
    {
        public event EventHandler<Message> OnMessageReceived;
        public event EventHandler OnConnectedToServer;

        private static ClientEngine _clientEngine;
        private static TcpClient _serverObject;
        private static readonly object Padlock = new object();

        private ClientEngine()
        {

        }

        public static ClientEngine Client //Singleton pattern kek. padlock works like lock checker - if it's not in use, the code inside works just fine, (?) otherwise it waits till padlock is free
        {
            get
            {
                lock (Padlock)
                {
                    if(_serverObject == null)
                    {
                        _serverObject = new TcpClient(AddressFamily.InterNetwork);
                        _clientEngine = new ClientEngine();
                    }
                    return _clientEngine;
                }
            }
        }

        #region Task methods
        public async Task Connect(IPAddress address, int port) //tries to connect to the specified IP and port every 1000ms until it's connected. Probably, need to create a workaround for cases when there's no connection to the server.
        {
            while (!_serverObject.Connected)
            {
                await _serverObject.ConnectAsync(address, port);
                await Task.Delay(1000);
            }
            await ReceiveMessageAsync(); //Prolly, needed to receive a greeting message? Is it even neccessary?
            OnConnectedToServer?.Invoke(this, EventArgs.Empty);
        }

        public async Task SendMessageAsync(Message msg)
        {
            byte[] msgBytes = Encoding.UTF8.GetBytes(msg.Content);
            await SendInfoPacketAsync(msg);
            await _serverObject.GetStream().WriteAsync(msgBytes, 0, msgBytes.Length);
        }

        public async Task ReceiveMessageAsync()
        {
            NetworkStream networkStream = _serverObject.GetStream();
            while (true)
            {
                if (networkStream.DataAvailable)
                {
                    string[] infoPacket = await ReceiveInfoPacketAsync(networkStream);
                    int messageLength = Convert.ToInt32(infoPacket[1]);
                    byte[] data = new byte[messageLength];
                    int receivedDataLength = await networkStream.ReadAsync(data, 0, data.Length);
                    Message receivedMessage = new Message(Encoding.UTF8.GetString(data, 0, receivedDataLength), infoPacket[0]);
                    OnMessageReceived?.Invoke(this, receivedMessage);
                }
                await Task.Delay(500);
            }
        }

        private async Task SendInfoPacketAsync(Message msg)
        {
            byte[] infoBytes = new byte[40]; //32 bytes for nickname and other 8 for additional stuff.
            Encoding.UTF8.GetBytes(msg.Author.UserName).CopyTo(infoBytes, 0);
            Encoding.UTF8.GetBytes(msg.Content.Length.ToString()).CopyTo(infoBytes, 32); //first of last 8 bytes is message length.
            await _serverObject.GetStream().WriteAsync(infoBytes, 0, 40);
        }

        private async Task<string[]> ReceiveInfoPacketAsync(NetworkStream stream)
        {
            byte[] receivedBytes = new byte[40];
            await stream.ReadAsync(receivedBytes, 0, 40);
            string receivedMessage = Encoding.UTF8.GetString(receivedBytes);
            return new string[] { receivedMessage.Substring(0, 32).Trim(' ', '\0'), receivedMessage.Substring(32).Trim(' ', '\0') };
        }
        #endregion
    }
}

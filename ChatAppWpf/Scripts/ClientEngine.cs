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
        public event Func<Message, Task> OnMessageReceived;
        public event Func<Task> OnConnectedToServer;

        internal TcpClient Server;

        public ClientEngine()
        {
            Server = new TcpClient(AddressFamily.InterNetwork);
        }

        #region Task methods
        public async Task Connect(IPAddress address)
        {
            while (!Server.Connected)
            {
                Console.WriteLine("Connecting to the server...");
                await Server.ConnectAsync(address, 5813);
                await Task.Delay(1000);
            }
            if (OnConnectedToServer != null)
            {
                await OnConnectedToServer();
            }
            Console.Clear();
        }

        public async Task SendMessageAsync(Message msg)
        {
            byte[] msgBytes = Encoding.UTF8.GetBytes(msg.Content);
            await SendInfoPacketAsync(msg);
            await Server.GetStream().WriteAsync(msgBytes, 0, msgBytes.Length);
        }

        public async Task ReceiveMessageAsync()
        {
            NetworkStream NetStream = Server.GetStream();
            while (true)
            {
                if (NetStream.DataAvailable)
                {
                    string[] infoPacket = await ReceiveInfoPacketAsync(NetStream);
                    byte[] data = new byte[Convert.ToInt32(infoPacket[1])];
                    await NetStream.ReadAsync(data, 0, data.Length);
                    Message ReceivedMessage = new Message(Encoding.UTF8.GetString(data, 0, data.Length), infoPacket[0]);
                    await OnMessageReceived(ReceivedMessage);
                }
            }
        }

        public async Task SendInfoPacketAsync(Message msg)
        {
            byte[] infoBytes = new byte[40]; //32 bytes for nickname and other 8 for additional stuff.
            Encoding.UTF8.GetBytes(msg.Sender.UserName).CopyTo(infoBytes, 0);
            Encoding.UTF8.GetBytes(msg.Content.Length.ToString()).CopyTo(infoBytes, 32); //first of last 8 bytes is message length.
            await Server.GetStream().WriteAsync(infoBytes, 0, 40);
        }

        public async Task<string[]> ReceiveInfoPacketAsync(NetworkStream stream)
        {
            byte[] receivedBytes = new byte[40];
            await stream.ReadAsync(receivedBytes, 0, 40);
            string receivedMessage = Encoding.UTF8.GetString(receivedBytes);
            return new string[] { receivedMessage.Substring(0, 32).Trim(' ', '\0'), receivedMessage.Substring(32).Trim(' ', '\0') };
        }

        public async Task DisplayMessage(Message msg)
        {
            if (msg.Sender.UserName != Console.Title)
            {
                Console.WriteLine($"{msg.Sender.UserName}: {msg.Content}");
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WofEngine.NetworkCommand;

namespace WofEngine.Service
{
    class SocketService
    {
        public delegate void TcpDataReceivedDelegate(string data);
        public TcpDataReceivedDelegate OnDataReceivedCallback;
        public bool IsReady { get; private set; } = false;
        private TcpClient TcpConnection;
        private Thread NetworkThread;
        private string ServerAddress;
        private int ServerPort;


        public SocketService(string address, int port)
        {
            ServerAddress = address;
            ServerPort = port;
            NetworkThread = new Thread(new ThreadStart(NetworkHandler));
            NetworkThread.Start();
        }

        private void NetworkHandler() {
            TcpConnection = new TcpClient(ServerAddress, ServerPort);
            Byte[] buffer = new byte[1024];
            IsReady = true;
            while (TcpConnection.Connected)
            {
                NetworkStream stream = TcpConnection.GetStream();
                int length = 0;
                while ((length = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    byte[] receivedBytes = new byte[length];
                    Array.Copy(buffer, 0, receivedBytes, 0, length);
                    string tcpLastResponse = Encoding.UTF8.GetString(receivedBytes);
                    OnTcpDataReceived(tcpLastResponse);
                }
            }
        }

        private void OnTcpDataReceived(string tcpLastResponse)
        {
            // TODO: We may receive multiple packets here in which case JSON string will not be valid
            // Extract packets and deserialize separately, then invoke callback for each one
            if (OnDataReceivedCallback != null)
                OnDataReceivedCallback(tcpLastResponse);
        }

        public void SendCommand(GenericNetworkCommand command)
        {
            NetworkStream stream = TcpConnection.GetStream();

            if (!stream.CanWrite)
            {
                return;
            }

            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(command.ToJson());
            writer.Flush();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WofEngine.NetworkCommand;
using WofEngine.NetworkPacket;

namespace WofEngine.Service
{
    class SocketService
    {
        public delegate void TcpDataReceivedDelegate(string data);
        public delegate void UdpDataReceivedDelegate(string data);
        public TcpDataReceivedDelegate OnDataReceivedCallback;
        public UdpDataReceivedDelegate OnPacketReceived;

        public bool IsReady { get; private set; } = false;
        private TcpClient TcpConnection;
        private UdpClient UdpConnection;
        private Thread TcpNetworkThread;
        private Thread UdpNetworkThread;
        private string ServerAddress;
        private int ServerPort;
        private IPEndPoint Endpoint;
        private bool ClientAlive = true;

        public SocketService(string address, int port)
        {
            ServerAddress = address;
            ServerPort = port;
            Endpoint = new IPEndPoint(IPAddress.Parse(ServerAddress), ServerPort + 1);
            UdpConnection = new UdpClient(ServerAddress, ServerPort + 1);
            UdpConnection.Connect(Endpoint);
            TcpNetworkThread = new Thread(new ThreadStart(TcpHandler));
            TcpNetworkThread.Start();
            UdpNetworkThread = new Thread(new ThreadStart(UdpHandler));
            UdpNetworkThread.Start();
        }

        private void UdpHandler()
        {
            while (ClientAlive)
            {
                byte[] data = UdpConnection.Receive(ref Endpoint);
                string json = Encoding.UTF8.GetString(data);
                if (OnPacketReceived != null)
                    OnPacketReceived(json);
            }
        }

        private void TcpHandler() {
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

        public void SendPacket(GenericNetworkPacket packet)
        {
            byte[] data = packet.ToBytes();
            UdpConnection.Send(data, data.Length);
        }

        public void Disconnect()
        {
            TcpConnection.Close();
            ClientAlive = false;
            UdpConnection.Close();
        }
    }
}

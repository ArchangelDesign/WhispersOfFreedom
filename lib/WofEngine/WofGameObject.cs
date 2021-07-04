using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WofEngine.Entity;
using WofEngine.Exceptions;
using WofEngine.NetworkCommand;
using WofEngine.NetworkPacket;
using WofEngine.Service;

namespace WofEngine
{
    public enum WOF_STATE
    {
        LOGIN_SCREEN = 0,
        LOGINIG_IN = 1,
        LOBBY = 2,
        LOGIN_FAILED = 3,
    }

    public class WofGameObject
    {
        public static readonly WofGameObject Game = new WofGameObject();

        private readonly RestService Rest = new RestService();
        public WOF_STATE State { get; private set; } = WOF_STATE.LOGIN_SCREEN;

        public delegate void StateChangedDelegate(WOF_STATE state);
        public delegate void CommandReceivedDelegate(GenericNetworkCommand cmd);
        public delegate void PacketReceivedDelegate(GenericNetworkPacket packet);

        public StateChangedDelegate OnStateChanged;
        public CommandReceivedDelegate OnCommandReceived;
        public PacketReceivedDelegate OnPacketReceived;

        private SocketService SocketService;
        

        public void EnterServer(string username, string password)
        {
            SetNewState(WOF_STATE.LOGINIG_IN);
            EnterServerRequest request = new EnterServerRequest() { Username = username, Password = password };
            new Thread(new ParameterizedThreadStart(EnterServer)).Start(request);
        }

        public void InitializeTcpConnection()
        {
            // TODO: log errors, report connection problems
            if (SocketService != null)
                return;
            if (Rest.SessionToken == null)
                return;
            SocketService = new SocketService("127.0.0.1", 8081);
            SocketService.OnDataReceivedCallback += SocketDataReceived;
            SocketService.OnPacketReceived += PacketReceived;
            // wait for connection to be established
            int waitPeriod = 0;
            while (!SocketService.IsReady && waitPeriod < 5)
            {
                Thread.Sleep(1000);
                waitPeriod++;
            }
            if (!SocketService.IsReady)
                throw new TcpConnectionError();
            SocketService.SendCommand(new IdentificationCommand(Rest.SessionToken));
            SocketService.SendPacket(new IdentificationPacket(Rest.SessionToken));
        }

        private void PacketReceived(string data)
        {
            GenericNetworkPacket packet = GenericNetworkPacket.FromJson(data);
            if (OnPacketReceived != null)
                OnPacketReceived(packet);
        }

        private void SocketDataReceived(string data)
        {
            try
            {
                GenericNetworkCommand cmd = GenericNetworkCommand.FromJson(data);
                if (OnCommandReceived != null)
                    OnCommandReceived(cmd);
                switch (cmd.Command.ToLower())
                {
                    case "welcome":

                        break;
                    case "ping":
                        break;
                }
            }
            catch (Exception) { }
        }

        public void InitializeUdpConnection()
        {

        }


        private void EnterServer(object request)
        {
            try
            {
                var response = Rest.Send((EnterServerRequest)request);
                Rest.SetSessionToken(EnterServerResponse.FromAbstractResponse(response).SessionToken);
                SetNewState(WOF_STATE.LOBBY);
            }
            catch (WebException e)
            {
                SetNewState(WOF_STATE.LOGIN_FAILED);
            }
        }

        public void Exit()
        {
            SocketService.Disconnect();
        }

        private void SetNewState(WOF_STATE newState)
        {
            State = newState;
            if (OnStateChanged != null)
                OnStateChanged(newState);
        }


    }
}

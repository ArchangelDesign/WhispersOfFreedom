using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WofEngine.Entity;
using WofEngine.Exceptions;
using WofEngine.NetworkCommand;
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

        public delegate void OnStateChanged(WOF_STATE state);

        public OnStateChanged StateChangedCallback;

        private SocketService TcpSocketService;
        
        public void EnterServer(string username, string password)
        {
            SetNewState(WOF_STATE.LOGINIG_IN);
            EnterServerRequest request = new EnterServerRequest() { Username = username, Password = password };
            new Thread(new ParameterizedThreadStart(EnterServer)).Start(request);
        }

        public void InitializeTcpConnection() {
            // TODO: log errors, report connection problems
            if (TcpSocketService != null)
                return;
            if (Rest.SessionToken == null)
                return;
            TcpSocketService = new SocketService("127.0.0.1", 8081);
            TcpSocketService.OnDataReceivedCallback += SocketDataReceived;
            // wait for connection to be established
            int waitPeriod = 0;
            while (!TcpSocketService.IsReady && waitPeriod < 5)
            {
                Thread.Sleep(1000);
                waitPeriod++;
            }
            if (!TcpSocketService.IsReady)
                throw new TcpConnectionError();
            TcpSocketService.SendCommand(new IdentificationCommand(Rest.SessionToken));
        }

        private void SocketDataReceived(string data)
        {
            // throw new NotImplementedException();
        }

        public void InitializeUdpConnection() { }


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


        private void SetNewState(WOF_STATE newState)
        {
            State = newState;
            if (StateChangedCallback != null)
                StateChangedCallback(newState);
        }
    }
}

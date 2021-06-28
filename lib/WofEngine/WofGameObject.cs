using System;
using System.Net;
using WofEngine.Entity;
using WofEngine.Exceptions;
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
        private readonly RestService Rest = new RestService();
        public WOF_STATE State { get; private set; } = WOF_STATE.LOGIN_SCREEN;
        
        public void EnterServer(string username, string password)
        {
            // TODO: do not block this thread
            State = WOF_STATE.LOGINIG_IN;
            EnterServerRequest request = new EnterServerRequest() { Username = username, Password = password };
            try
            {
                var response = Rest.Send(request);
                Rest.SetSessionToken(EnterServerResponse.FromAbstractResponse(response).SessionToken);
            } catch (WebException e)
            {
                State = WOF_STATE.LOGIN_FAILED;
                throw new EnteringServerFailed(e.Message);
            }
            State = WOF_STATE.LOBBY;
        }
    }
}

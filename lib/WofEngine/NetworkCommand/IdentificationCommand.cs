using System;
using System.Collections.Generic;
using System.Text;

namespace WofEngine.NetworkCommand
{
    public class IdentificationCommand : GenericNetworkCommand
    {
        public IdentificationCommand(string sessionToken)
        {
            ClientId = sessionToken;
            Command = "identification";
        }
    }
}

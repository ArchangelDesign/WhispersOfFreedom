using System;
using System.Collections.Generic;
using System.Text;

namespace WofEngine.NetworkPacket
{
    public class IdentificationPacket : GenericNetworkPacket
    {
        public IdentificationPacket(string sessionToken)
        {
            Command = "identification";
            ClientId = sessionToken;
        }
    }
}

using crypcy.shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crypcy.stun
{

    // Not used
    public class MessageServerHandler : IPeerItemHandler
    {
        public void HandlePeerItem(PeerItem input)
        {
            Program.ProcessItem(input, System.Net.Sockets.ProtocolType.Tcp, null, null);
        }
    }
}

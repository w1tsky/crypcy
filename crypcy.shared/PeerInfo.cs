using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace crypcy.shared
{
    public enum ConnectionTypes { Unknown, LAN, WAN }
    public class PeerInfo : IPeerItem
    {
        public string Name { get; set; }
        public long ID { get; set; }
        public IPEndPoint ExternalEndpoint { get; set; }
        public IPEndPoint InternalEndpoint { get; set; }
        public ConnectionTypes ConnectionType { get; set; }
        public PeerItemType peerItemType { get; set; }
        

        public List<IPAddress> InternalAddresses = new List<IPAddress>(); 

        [NonSerialized]
        public TcpClient tcp;
        [NonSerialized] 
        public bool Initialized;

        public bool Update(PeerInfo peer)
        {
            peerItemType = PeerItemType.PeerInfo;

            if (ID == peer.ID)
            {
                foreach (PropertyInfo P in peer.GetType().GetProperties())
                    if (P.GetValue(peer) != null)
                        P.SetValue(this, P.GetValue(peer));

                if (peer.InternalAddresses.Count > 0)
                {
                    InternalAddresses.Clear();
                    InternalAddresses.AddRange(peer.InternalAddresses);
                }
            }

            return (ID == peer.ID);
        }

        public PeerInfo Simplified()
        {
            return new  PeerInfo()
            {
                Name = this.Name,
                ID = this.ID,
                InternalEndpoint = this.InternalEndpoint,
                ExternalEndpoint = this.ExternalEndpoint,
                peerItemType = PeerItemType.PeerInfo                   
            };
        }
    }

}

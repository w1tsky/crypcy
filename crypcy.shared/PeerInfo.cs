using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json.Serialization;

namespace crypcy.shared
{
    public enum ConnectionTypes { Unknown, LAN, WAN }
    [Serializable]
    public class PeerInfo : PeerItem
    {
        public string Name { get; set; }
        [JsonConverter(typeof(IPEndPointConverter))]
        public IPEndPoint ExternalEndpoint { get; set; }
        [JsonConverter(typeof(IPEndPointConverter))]
        public IPEndPoint InternalEndpoint { get; set; }
        public ConnectionTypes ConnectionType { get; set; }
        public List<IPAddress> InternalAddresses = new List<IPAddress>(); 

        [NonSerialized]
        public TcpClient PeerTCP;
        [NonSerialized] 
        public bool Initialized;

        public bool Update(PeerInfo peer) 
        {
            PeerItemType = PeerItemType.PeerInfo;

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
                ID = this.ID,
                PeerItemType = PeerItemType.PeerInfo,  
                Name = this.Name,
                InternalEndpoint = this.InternalEndpoint,
                ExternalEndpoint = this.ExternalEndpoint            
            };
        }
    }
}

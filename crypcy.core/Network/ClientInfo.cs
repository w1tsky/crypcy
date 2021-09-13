using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;


public enum ConnectionTypes { Unknown, LAN, WAN }

[Serializable]
public class ClientInfo
{
    public string Name { get; set; }
    public long ID { get; set; }
    public ConnectionTypes ConnectionType { get; set; }
    public List<IPAddress> InternalAddresses = new List<IPAddress>();        

    public bool Update(ClientInfo ci)
    {
        
        if (ID == ci.ID)
        {
            foreach (PropertyInfo p in ci.GetType().GetProperties())
                if (p.GetValue(ci) != null)
                    p.SetValue(this, p.GetValue(ci));

            if (ci.InternalAddresses.Count > 0)
            {
                InternalAddresses.Clear();
                InternalAddresses.AddRange(ci.InternalAddresses);
            }
        }

        return (ID == ci.ID);
    }


    public ClientInfo Simplified()
    {
        return new ClientInfo()
        {
            Name = this.Name,
            ID = this.ID,              
        };
    }
}    

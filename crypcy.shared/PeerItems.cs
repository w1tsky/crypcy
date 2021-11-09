using System;
using System.Text.Json.Serialization;

namespace crypcy.shared
{
    public enum NotificationsTypes { ServerShutdown, Disconnected }

    [Serializable]
    public class Notification : PeerItem
    {

        public NotificationsTypes Type { get; set; }
        public object Tag { get; set; }

        public Notification(NotificationsTypes _Type, object _Tag)
        {
            PeerItemType = PeerItemType.Notification;
            Type = _Type;
            Tag = _Tag;
        }
    }

    [Serializable]
    public class Message : PeerItem
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Content { get; set; }

        public Message(string from, string to, string content)
        {
            PeerItemType = PeerItemType.Message;
            From = from;
            To = to;
            Content = content;
        }
    }

    [Serializable]
    public class Req : PeerItem
    {
        public long RecipientID { get; set; }
        public long SenderID { get; set; }

        // invistigate ctor properties set

        [JsonConstructor]
        public Req(long senderID, long recipientID)
        {
            PeerItemType = PeerItemType.Req;
            ID = senderID;
            RecipientID = recipientID;
            SenderID = senderID;
        }

    }

    [Serializable]
    public class Ack : PeerItem
    {
        public long RecipientID { get; set; }
        public long SenderID { get; set; }
        public bool Responce { get; set; }


        public Ack(long senderID)
        {
            PeerItemType = PeerItemType.Ack;
            SenderID = senderID;
            ID = senderID;
        }
    }

    [Serializable]
    public class KeepAlive : PeerItem
    {
        public KeepAlive()
        {
            PeerItemType = PeerItemType.KeepAlive;
        }
    }


}
using System;

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
        public long RecipientID { get; set; }

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
        public Req(long Sender_ID, long Recipient_ID)
        {
            ID = Sender_ID;
            PeerItemType = PeerItemType.Req;
            RecipientID = Recipient_ID;
        }
    }

    [Serializable]
    public class Ack : PeerItem
    {
        public long RecipientID { get; set; }
        public bool Responce { get; set; }

        public Ack(long Sender_ID)
        {
            ID = Sender_ID;
            PeerItemType = PeerItemType.Ack;
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
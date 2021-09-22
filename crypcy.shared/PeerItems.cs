using System;

namespace crypcy.shared
{
    [Serializable]
    public class Message : IPeerItem
    {
        public long ID { get; set; }

        public string From { get; set; }
        public string To { get; set; }
        public string Content { get; set; }
        public long RecipientID { get; set; }    
        public PeerItemType peerItemType { get; set; }    

        public Message(string from, string to, string content)
        {
            From = from;
            To = to;
            Content = content;
            peerItemType = PeerItemType.Message;
        }
    }

    [Serializable]
    public class Req : IPeerItem
    {
        public long ID { get; set; }
        public long RecipientID { get; set; }       
        public PeerItemType peerItemType { get; set; }
        public Req(long Sender_ID, long Recipient_ID)
        {
            ID = Sender_ID;
            RecipientID = Recipient_ID;
            peerItemType = PeerItemType.Req;
        }
    }  
}
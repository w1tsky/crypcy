using System;

namespace crypcy.shared
{
    [Serializable]
    public class Message : IPeer
    {
        public long ID { get; set; }

        public string From { get; set; }
        public string To { get; set; }
        public string Content { get; set; }
        public long RecipientID { get; set; }        

        public Message(string from, string to, string content)
        {
            From = from;
            To = to;
            Content = content;
        }
    }

    [Serializable]
    public class Req : IPeer
    {
        public long ID { get; set; }
        public long RecipientID { get; set; }       

        public Req(long Sender_ID, long Recipient_ID)
        {
            ID = Sender_ID;
            RecipientID = Recipient_ID;
        }
    }  
}
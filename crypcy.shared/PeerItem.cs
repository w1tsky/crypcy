namespace crypcy.shared
{
    public enum PeerItemType
    { 
        PeerInfo,
        Message,
        Req,
        Notification
    }
    public class PeerItem
    {
        public long ID { get; set; } 
        public PeerItemType PeerItemType { get; set; }
    }

}
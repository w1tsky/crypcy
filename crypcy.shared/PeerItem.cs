namespace crypcy.shared
{
    public enum PeerItemType
    { 
        PeerInfo,
        Message,
        Req,
        Notification
    }
    public abstract class PeerItem
    {
        public long ID { get; set; } 
        public PeerItemType PeerItemType { get; set; }
    }

}
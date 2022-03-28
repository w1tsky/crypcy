namespace crypcy.shared
{
    public enum PeerItemType
    {
        PeerInfo,
        Message,
        Req,
        Notification,
        Ack,
        KeepAlive
    }

    public class PeerHeader
    {
        public uint Length { get; set; }

    }
    public class PeerItem
    {
        public long ID { get; set; }
        public PeerItemType PeerItemType { get; set; }
    }

}
namespace crypcy.shared
{

    public enum PeerItemType
    { 
        PeerInfo,
        Message,
        Req
    }
    public interface IPeerItem
    {
        long ID { get; set; } 
        PeerItemType peerItemType { get; set; }
    }

}
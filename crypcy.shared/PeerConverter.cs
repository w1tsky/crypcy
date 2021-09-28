using System.Text;
using System.Text.Json;

namespace crypcy.shared
{
    public static class PeerConverter
    {
        public static byte[] PeerToByteArray(this PeerItem peerItem)
        {

            return JsonSerializer.SerializeToUtf8Bytes(peerItem);
        }

        public static PeerItem ByteArrayToPeer(this byte[] bytes)
        {
            string jsonStr = Encoding.UTF8.GetString(bytes);
            System.Console.WriteLine(jsonStr);
            PeerItem item = JsonSerializer.Deserialize<PeerItem>(jsonStr);

            switch(item.PeerItemType)
            {
                case PeerItemType.PeerInfo:
                    return JsonSerializer.Deserialize<PeerInfo>(jsonStr);
                case PeerItemType.Message:
                    return JsonSerializer.Deserialize<Message>(jsonStr);
                case PeerItemType.Req:
                    return JsonSerializer.Deserialize<Req>(jsonStr);
                case PeerItemType.Notification:
                    return JsonSerializer.Deserialize<Notification>(jsonStr);
                case PeerItemType.Ack:
                    return JsonSerializer.Deserialize<Ack>(jsonStr);
                case PeerItemType.KeepAlive:
                    return JsonSerializer.Deserialize<KeepAlive>(jsonStr);
                default:
                    return JsonSerializer.Deserialize<PeerItem>(jsonStr);
            }
        }
    }
}
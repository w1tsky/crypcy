using System.Text;
using System.Text.Json;

namespace crypcy.shared
{
    public static class PeerConverter
    {
        public static byte[] PeerToByteArray(this IPeerItem peerInfo)
        {

            return JsonSerializer.SerializeToUtf8Bytes(peerInfo);
        }

        public static IPeerItem ByteArrayToPeer(this byte[] bytes)
        {
            string jsonStr = Encoding.UTF8.GetString(bytes);
            IPeerItem item = JsonSerializer.Deserialize<IPeerItem>(jsonStr);

            if(item.peerItemType == PeerItemType.PeerInfo)
            {
                return JsonSerializer.Deserialize<PeerInfo>(jsonStr);
            }
            else if(item.peerItemType == PeerItemType.Message)
            {
                return JsonSerializer.Deserialize<Message>(jsonStr);
            }
            else if(item.peerItemType == PeerItemType.Req)
            {
                return JsonSerializer.Deserialize<Req>(jsonStr);
            }
            else
            {
                return JsonSerializer.Deserialize<IPeerItem>(jsonStr);
            }
        }
    }
}
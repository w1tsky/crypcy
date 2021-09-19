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
            return JsonSerializer.Deserialize<IPeerItem>(jsonStr);        
        }
    }
}
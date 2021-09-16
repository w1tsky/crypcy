using System.Text;
using System.Text.Json;

namespace crypcy.shared
{
    public static class PeerConverter
    {
        public static byte[] PeerToByteArray(this IPeer peerInfo)
        {
            byte[] data = JsonSerializer.SerializeToUtf8Bytes(peerInfo);
            return data;
        }
        
        public static IPeer ByteArrayToPeer(this byte[] bytes)
        {
            string jsonStr = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<PeerInfo>(jsonStr);
        }
    }
}
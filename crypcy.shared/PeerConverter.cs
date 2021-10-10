using System;
using System.Text;
using System.Text.Json;

namespace crypcy.shared
{
    public static class PeerConverter
    {
        public static byte[] PeerToByteArray(this PeerItem peerItem)
        {

            switch (peerItem.PeerItemType)
            {
                case PeerItemType.PeerInfo:
                    return JsonSerializer.SerializeToUtf8Bytes<PeerInfo>((PeerInfo)peerItem);
                case PeerItemType.Message:
                    return JsonSerializer.SerializeToUtf8Bytes<Message>((Message)peerItem);
                case PeerItemType.Req:
                    return JsonSerializer.SerializeToUtf8Bytes<Req>((Req)peerItem);
                case PeerItemType.Notification:
                    return JsonSerializer.SerializeToUtf8Bytes<Notification>((Notification)peerItem);
                case PeerItemType.Ack:
                    return JsonSerializer.SerializeToUtf8Bytes<Ack>((Ack)peerItem);
                case PeerItemType.KeepAlive:
                    return JsonSerializer.SerializeToUtf8Bytes<KeepAlive>((KeepAlive)peerItem);
                default:
                    return JsonSerializer.SerializeToUtf8Bytes(peerItem);
            }

        }

        public static PeerItem ByteArrayToPeer(this byte[] bytes, int bytesCount)
        {

            string jsonStr = Encoding.UTF8.GetString(bytes, 0, bytesCount);
            PeerItem item = JsonSerializer.Deserialize<PeerItem>(jsonStr);

            switch (item.PeerItemType)
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
                    return item;
            }
        }
    }
}
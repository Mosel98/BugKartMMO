using Network.IO;
using System.IO;

namespace Network.Messages
{
    public class SceneLoadedMessage : AMessageBase
    {
        public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
        {
            base.Deserialize(_senderID, _data, _receivedBytes);
        }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.SCENE_LOADED);
                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            NetworkManager.Instance.SendAllObjectsToClient(SenderID);
        }
    }
}
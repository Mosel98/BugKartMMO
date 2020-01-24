using System.IO;
using System.Linq;
using Network.IO;

namespace Network.Messages
{
    public class UpdateNetworkBehaviourMessage : AMessageBase
    {
        public NetworkIdentity NetID { get; set; }
        public uint ComponentID { get; set; }
        public byte[] Bytes { get; set; }

        public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
        {
            base.Deserialize(_senderID, _data, _receivedBytes);
            using (MemoryStream ms = new MemoryStream(_data, 0, _receivedBytes))
            {
                using (NetworkReader nr = new NetworkReader(ms))
                {
                    nr.ReadUInt16();
                    NetID = nr.ReadNetworkIdentity();
                    ComponentID = nr.ReadUInt32();
                    int length = nr.ReadInt32();
                    Bytes = nr.ReadBytes(length);
                }
            }
        }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((ushort)EMessageType.NETWORK_BEHAVIOUR_UPDATE);
                    nw.Write(NetID);
                    nw.Write(ComponentID);
                    nw.Write(Bytes.Length);
                    nw.Write(Bytes);
                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            NetworkBehaviour[] networkBehaviours = NetID.GetComponents<NetworkBehaviour>();
            networkBehaviours.First(o => o.ComponentID == ComponentID).Deserialize(Bytes);
        }
    }
}
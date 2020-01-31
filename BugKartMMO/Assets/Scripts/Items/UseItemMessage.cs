using Network.IO;
using System.IO;
using UnityEngine;

namespace Network.Messages
{
    public class UseItemMessage : AMessageBase
    {
        public GameObject Player { get; set; }
        public float Item { get; set; }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.USE_ITEM);
                    nw.Write(Player);
                    nw.Write(Item);

                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
        {
            base.Deserialize(_senderID, _data, _receivedBytes);
            using (MemoryStream ms = new MemoryStream(_data, 0, _receivedBytes))
            {
                using (NetworkReader nr = new NetworkReader(ms))
                {
                    nr.ReadInt16();
                    Player = nr.ReadGameObject();
                    Item = nr.ReadSingle();
                }
            }
        }

        public override void Use()
        {
            Debug.Log("Item use request for " + Player);
            if (Player is object)
            {
                NetworkItemHandeling NIM = Player.GetComponent<NetworkItemHandeling>();
                if (NIM is object)
                {
                    NIM.UseItem(Player, Item);
                }
                else
                {
                    Debug.LogWarning("NetworkItemHandeling was not found! " + Player);
                }
            }
            else
            {
                Debug.LogWarning("Object was not found!");
            }
        }
    }
}


using Network.IO;
using System.IO;
using UnityEngine;

// Mario
namespace Network.Messages
{
    public class CollisionCheckMessage : AMessageBase
    {
        public GameObject Player { get; set; }
        public GameObject ItemBox { get; set; }
        public string Tag { get; set; }
        public float Speed { get; set; }
        public float Accel { get; set; }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.COLLISION_CHECK);
                    nw.Write(Player);
                    nw.Write(ItemBox);
                    nw.Write(Tag);
                    nw.Write(Speed);
                    nw.Write(Accel);

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
                    ItemBox = nr.ReadGameObject();
                    Tag = nr.ReadString();
                    Speed = nr.ReadSingle();
                    Accel = nr.ReadSingle();
                }
            }
        }

        public override void Use()
        {
            Debug.Log("Received CheckCollisionMessage for " + Player);
            if (Player is object)
            {
                NetworkItemHandeling NIM = Player.GetComponent<NetworkItemHandeling>();
                if (NIM is object)
                {                   
                    if (Tag == "ItemBox")
                    {
                        NIM.ItemBoxCheck(Player, ItemBox);
                    }
                    else
                    {
                        NIM.CollisionCheck(Player, ItemBox, Tag, Speed, Accel);
                    }
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

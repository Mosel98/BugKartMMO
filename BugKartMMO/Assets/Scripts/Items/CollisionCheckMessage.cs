using Network.IO;
using System.IO;
using UnityEngine;

// Mario
namespace Network.Messages
{
    public class CollisionCheckMessage : AMessageBase
    {
        public GameObject GameObject { get; set; }
        public string Tag { get; set; }
        public float Speed { get; set; }
        public float Accel { get; set; }

        public CollisionCheckMessage()
        {

        }

        public CollisionCheckMessage(GameObject _go, string _tag)
        {
            GameObject = _go;
            Tag = _tag;
        }

        public CollisionCheckMessage(GameObject _go, string _tag, float _speed, float _accel)
        {
            GameObject = _go;
            Tag = _tag;
            Speed = _speed;
            Accel = _accel;
        }

        public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
        {
            base.Deserialize(_senderID, _data, _receivedBytes);
            using (MemoryStream ms = new MemoryStream(_data, 0, _receivedBytes))
            {
                using (NetworkReader nr = new NetworkReader(ms))
                {
                    nr.ReadInt16();
                    GameObject = nr.ReadGameObject();
                    Tag = nr.ReadString();
                    Speed = (float)nr.ReadDouble();
                    Accel = (float)nr.ReadDouble();
                }
            }
        }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.COLLISION_CHECK);
                    nw.Write(GameObject);
                    nw.Write(Tag);
                    nw.Write(Speed);
                    nw.Write(Accel);

                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            Debug.Log("Received CheckCollisionMessage for " + GameObject, GameObject);
            if (GameObject is object)
            {
                NetworkItemHandeling NIM = GameObject.GetComponent<NetworkItemHandeling>();
                if (NIM is object)
                {
                    if (Tag == "ItemBox")
                    {
                        NIM.ItemBoxCheck(GameObject);
                    }
                    else
                    {
                        NIM.CollisionCheck(GameObject, Tag, Speed, Accel);
                    }
                }
                else
                {
                    Debug.LogWarning("NetworkItemHandeling was not found! " + GameObject, GameObject);
                }
            }
            else
            {
                Debug.LogWarning("Object was not found!");
            }
        }
    }
}

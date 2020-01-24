using Network.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Network.Messages
{
    public class UseItemMessage : AMessageBase
    {
        public GameObject GameObject { get; set; }
        public float Item { get; set; }

        public UseItemMessage()
        {

        }

        public UseItemMessage(GameObject _go, float _item)
        {
            GameObject = _go;
            Item = _item;
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
                    Item = (float) nr.ReadDouble();
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
                    nw.Write(Item);

                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            Debug.Log("Item use request for " + GameObject, GameObject);
            if (GameObject is object)
            {
                NetworkItemHandeling NIM = GameObject.GetComponent<NetworkItemHandeling>();
                if (NIM is object)
                {
                    NIM.UseItem(GameObject, Item);
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


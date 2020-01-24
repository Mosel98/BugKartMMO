using Network.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Network.Messages
{
    public class ShootItemMessage : AMessageBase
    {
        public GameObject ItemPrefab { get; set; }
        public int PlayerID { get; set; }
        public KeyCode KeyCode { get; set; }

        public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
        {
            base.Deserialize(_senderID, _data, _receivedBytes);
            using (MemoryStream ms = new MemoryStream(_data, 0, _receivedBytes))
            {
                using (NetworkReader nr = new NetworkReader(ms))
                {
                    nr.ReadInt16();
                    ItemPrefab = nr.ReadPrefab();
                    PlayerID = nr.ReadInt32();
                    KeyCode = (KeyCode)nr.ReadInt16();
                }
            }
        }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.SHOOT_ITEM);
                    nw.Write(ItemPrefab);
                    nw.Write(PlayerID);
                    nw.Write((short)KeyCode);
                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            // Validierung
            GameObject go = NetworkManager.Instantiate(ItemPrefab);
            Item item = go.GetComponent<Item>();
            item.OwnerID = PlayerID;

            NetworkManager.Instance.SpawnGameObject(go);
        }
    }
}
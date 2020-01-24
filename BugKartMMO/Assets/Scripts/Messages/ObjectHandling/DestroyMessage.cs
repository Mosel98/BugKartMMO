using Network.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Network.Messages
{
    public class DestroyMessage : AMessageBase
    {
        public GameObject SpawnedObject { get; set; }

        public DestroyMessage()
        {

        }

        public DestroyMessage(GameObject _go)
        {
            SpawnedObject = _go;
        }

        public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
        {
            using (MemoryStream ms = new MemoryStream(_data, 0, _receivedBytes))
            {
                using (NetworkReader nr = new NetworkReader(ms))
                {
                    nr.ReadInt16();
                    SpawnedObject = nr.ReadGameObject();
                }
            }
        }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.DESTROY_OBJECT);
                    nw.Write(SpawnedObject);
                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            NetworkManager.Instance.RemoveIdentities(SpawnedObject.GetComponent<NetworkIdentity>());
            NetworkManager.Destroy(SpawnedObject);
        }
    }
}
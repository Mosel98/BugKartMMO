using Network.IO;
using System.IO;
using UnityEngine;

// Mario
namespace Network.Messages
{
    public class UpdateVariableMessage : AMessageBase
    {
        public GameObject GameObject { get; set; }
        public float Speed { get; set; }
        public float Accel { get; set; }
        public float Item { get; set; }

        public UpdateVariableMessage()
        {

        }

        public UpdateVariableMessage(GameObject _go, float _item)
        {
            GameObject = _go;
            Item = _item;
        }

        public UpdateVariableMessage(GameObject _go, float _speed, float _accel)
        {
            GameObject = _go;
            Speed = _speed;
            Accel = _accel;
            Item = -1;              
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
                    Speed = (float)nr.ReadDouble();
                    Accel = (float)nr.ReadDouble();
                    Item = (float)nr.ReadDouble();
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
                    nw.Write(Speed);
                    nw.Write(Accel);
                    nw.Write(Item);

                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            Debug.Log("Update Variables for " + GameObject, GameObject);
            if (GameObject is object)
            {
                PlayerController playerController = GameObject.GetComponent<PlayerController>();
                if (playerController is object)
                {
                    if(Item == -1)
                    {
                        playerController.UpdateVariable(Speed, Accel);
                    }
                    else
                    {
                        playerController.UpdateItem(Item);
                    }
                }
                else
                {
                    Debug.LogWarning("PlayerController was not found! " + GameObject, GameObject);
                }
            }
            else
            {
                Debug.LogWarning("Object was not found!");
            }
        }
    }
}

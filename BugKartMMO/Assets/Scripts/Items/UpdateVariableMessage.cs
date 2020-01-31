using Network.IO;
using System.IO;
using UnityEngine;

// Mario
namespace Network.Messages
{
    public class UpdateVariableMessage : AMessageBase
    {
        public GameObject Player { get; set; }
        public float Speed { get; set; }
        public float Accel { get; set; }
        public float Item { get; set; }

        public UpdateVariableMessage()
        {
            Item = -1;
        }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.COLLISION_CHECK);
                    nw.Write(Player);
                    nw.Write(Speed);
                    nw.Write(Accel);
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
                    Speed = nr.ReadSingle();
                    Accel = nr.ReadSingle();
                    Item = nr.ReadSingle();
                }
            }
        }

        public override void Use()
        {
            Debug.Log("Update Variables for " + Player);
            if (Player is object)
            {
                PlayerController playerController = Player.GetComponent<PlayerController>();
                if (playerController is object)
                {
                    if(Item == -1)
                    {
                       // playerController.UpdateVariable(Speed, Accel);
                        playerController.m_Acceleration = Accel;
                        playerController.m_Speed = Speed;
                        playerController.SetIsDirty();
                    }
                    else
                    {
                        playerController.UpdateItem(Item);
                    }
                }
                else
                {
                    Debug.LogWarning("PlayerController was not found! " + Player);
                }
            }
            else
            {
                Debug.LogWarning("Object was not found!");
            }
        }
    }
}
